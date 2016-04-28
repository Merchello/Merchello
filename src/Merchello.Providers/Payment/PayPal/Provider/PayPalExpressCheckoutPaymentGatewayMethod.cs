namespace Merchello.Providers.Payment.PayPal.Provider
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Merchello.Providers.Payment.PayPal.Services;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Merchello.Core.Models.TypeFields;
    using Merchello.Providers.Exceptions;

    using Umbraco.Core;

    /// <summary>
    /// A payment method for facilitating PayPal Express Checkouts.
    /// </summary>
    [GatewayMethodUi("PayPal.ExpressCheckout")]
    [GatewayMethodEditor("PayPal Express Checkout Method Editor", "PayPal Express Checkout", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    public class PayPalExpressCheckoutPaymentGatewayMethod : RedirectPaymentMethodBase
    {
        /// <summary>
        /// The <see cref="IPayPalApiService"/>.
        /// </summary>
        private readonly IPayPalApiService _paypalApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalExpressCheckoutPaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="paypalApiService">
        /// The <see cref="IPayPalApiService"/>.
        /// </param>
        public PayPalExpressCheckoutPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IPayPalApiService paypalApiService)
            : base(gatewayProviderService, paymentMethod)
        {
            Mandate.ParameterNotNull(paypalApiService, "payPalApiService");
            this._paypalApiService = paypalApiService;
        }

        /// <summary>
        /// Performs the AuthorizePayment operation.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="args">
        /// The <see cref="ProcessorArgumentCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        /// <remarks>
        /// For the ExpressCheckout there is not technically an "Authorize" but we use this to start the checkout process and to 
        /// mark intent to pay before redirecting the customer to PayPal.  e.g.  This method is called after the customer has
        /// clicked the Pay button, we then save the invoice and "Authorize" a payment setting the invoice status to Unpaid before redirecting.
        /// IN this way, we have both an Invoice and a Payment (denoting the redirect).  When the customer completes the purchase on PayPal sites
        /// the payment will be used to perform a capture and the invoice status will be changed to Paid.  In the event the customer cancels,
        /// the invoice will remain unpaid.  Events will be included in the controller handling the response to allow developers to delete invoices
        /// that are canceled if they choose.
        /// </remarks>
        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.Redirect, invoice.Total, PaymentMethod.Key);
            payment.CustomerKey = invoice.CustomerKey;
            payment.PaymentMethodName = PaymentMethod.Name;
            payment.ReferenceNumber = PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();
            payment.Collected = false;
            payment.Authorized = true;

            GatewayProviderService.Save(payment);

            // In this case, we want to do our own Apply Payment operation as the amount has not been collected -
            // so we create an applied payment with a 0 amount.  Once the payment has been "collected", another Applied Payment record will
            // be created showing the full amount and the invoice status will be set to Paid.
            GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, string.Format("To show promise of a {0} payment via PayPal Express Checkout", PaymentMethod.Name), 0);

            // Now we want to get things setup for the ExpressCheckout
            var apiResponse = this._paypalApiService.ExpressCheckout.SetExpressCheckout(invoice, payment);

            // if the ACK was success return a success IPaymentResult
            if (apiResponse.Ack != null && apiResponse.Ack == AckCodeType.SUCCESS)
            {
                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false, apiResponse.RedirectUrl);
            }

            // In the case of a failure, package up the exception so we can bubble it up.
            var ex = new PayPalApiException("PayPal Checkout Express initial response ACK was not Success");
            if (apiResponse.ErrorTypes.Any()) ex.ErrorTypes = apiResponse.ErrorTypes;

            return new PaymentResult(Attempt<IPayment>.Fail(payment, ex), invoice, false);
        }

        /// <summary>
        /// Performs the capture payment operation.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            // We need to determine if the entire amount authorized has been collected before marking
            // the payment collected.
            var appliedPayments = GatewayProviderService.GetAppliedPaymentsByPaymentKey(payment.Key);
            var applied = appliedPayments.Sum(x => x.Amount);

            payment.Collected = (amount + applied) == payment.Amount;
            payment.Authorized = true;

            GatewayProviderService.Save(payment);

            GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "PayPal ExpressCheckout SUCCESS payment", amount);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, CalculateTotalOwed(invoice).CompareTo(amount) <= 0);
        }

        protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Does the actual work of voiding a payment
        /// </summary>
        /// <param name="invoice">The invoice to which the payment is associated</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            foreach (var applied in payment.AppliedPayments())
            {
                applied.TransactionType = AppliedPaymentType.Void;
                applied.Amount = 0;
                applied.Description += " - **Void**";
                GatewayProviderService.Save(applied);
            }

            payment.Voided = true;
            GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
        }
    }
}