using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.Stripe.Models;
using Umbraco.Core;

namespace Merchello.Plugin.Payments.Stripe.Provider
{
    /// <summary>
    /// Represents an Stripe Payment Method
    /// </summary>
    [GatewayMethodEditor("Stripe Payment Method Editor", "~/App_Plugins/Merchello.Stripe/paymentmethod.html")]
    public class StripePaymentGatewayMethod : PaymentGatewayMethodBase, IStripePaymentGatewayMethod
    {
        private readonly StripePaymentProcessor _processor;

        public StripePaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, ExtendedDataCollection providerExtendedData) 
            : base(gatewayProviderService, paymentMethod)
        {
            _processor = new StripePaymentProcessor(providerExtendedData.GetProcessorSettings());
        }

        /// <summary>
        /// Does the actual work of creating and processing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment.</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            return ProcessPayment(invoice, TransactionMode.Authorize, invoice.Total, args);
        }

        /// <summary>
        /// Does the actual work of authorizing and capturing a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="amount">The amount to capture</param>
        /// <param name="args">Any arguments required to process the payment.</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            return ProcessPayment(invoice, TransactionMode.AuthorizeAndCapture, amount, args);
        }


        private IPaymentResult ProcessPayment(IInvoice invoice, TransactionMode transactionMode, decimal amount, ProcessorArgumentCollection args)
        {
            var cc = args.AsCreditCardFormData();

            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, invoice.Total, PaymentMethod.Key);
            payment.CustomerKey = invoice.CustomerKey;
            payment.Authorized = false;
            payment.Collected = false;
            payment.PaymentMethodName = string.Format("{0} Stripe Credit Card", cc.CreditCardType);
            payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.CcLastFour, cc.CardNumber.Substring(cc.CardNumber.Length - 4, 4).EncryptWithMachineKey());

            
            var result = _processor.ProcessPayment(invoice, payment, transactionMode, amount, cc);

            GatewayProviderService.Save(payment);

            if (!result.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, result.Payment.Exception.Message, 0);
            }
            else
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit,
                    //payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.AuthorizationTransactionResult) +
                    (transactionMode == TransactionMode.AuthorizeAndCapture ? string.Empty : " to show record of Authorization"),
                    transactionMode == TransactionMode.AuthorizeAndCapture ? invoice.Total : 0);
            }

            return result;
        }

        /// <summary>
        /// Does the actual work capturing a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="payment">The previously Authorize payment to be captured</param>
        /// <param name="amount">The amount to capture</param>
        /// <param name="args">Any arguments required to process the payment.</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            var result = _processor.PriorAuthorizeCapturePayment(invoice, payment);

            GatewayProviderService.Save(payment);

            if (!result.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied,
                    result.Payment.Exception.Message, 0);
            }
            else
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit,
                    payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.CaptureTransactionResult), amount);
            }

            return result;
        }

        /// <summary>
        /// Does the actual work of refunding a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="payment">The previously Authorize payment to be captured</param>
        /// <param name="amount">The amount to be refunded</param>
        /// <param name="args">Any arguments required to process the payment.</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            var result = _processor.RefundPayment(invoice, payment, amount);

            if (!result.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied,
                   result.Payment.Exception.Message, 0);
                return result;
            }

            // use the overloaded AppliedPayments method here for testing if we don't have
            // a MerchelloContext
            foreach (var applied in payment.AppliedPayments(GatewayProviderService))
            {
                applied.TransactionType = AppliedPaymentType.Refund;
                applied.Amount = 0;
                applied.Description += " - Refunded";
                GatewayProviderService.Save(applied);
            }

            payment.Amount = payment.Amount - amount;

            if (payment.Amount != 0)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit,
                    "To show partial payment remaining after refund", payment.Amount);
            }

            GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
        }

        /// <summary>
        /// Does the actual work of voiding a payment
        /// </summary>
        /// <param name="invoice">The invoice to which the payment is associated</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            var result = _processor.VoidPayment(invoice, payment);

            if (!result.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied,
                   result.Payment.Exception.Message, 0);
                return result;
            }

            // use the overloaded AppliedPayments method here for testing if we don't have
            // a MerchelloContext
            foreach (var applied in payment.AppliedPayments(GatewayProviderService))
            {
                applied.TransactionType = AppliedPaymentType.Refund;
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