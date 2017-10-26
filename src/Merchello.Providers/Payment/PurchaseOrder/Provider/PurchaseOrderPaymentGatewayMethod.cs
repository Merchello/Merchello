namespace Merchello.Providers.Payment.PurchaseOrder.Provider
{
    using System;
    using System.Linq;
    using Core;
    using Core.Gateways;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Services;
    using Umbraco.Core;

    /// <summary>
    /// Represents an PurchaseOrder Payment Method
    /// </summary>
    [GatewayMethodUi("PurchaseOrder.PurchaseOrder")]
    [PaymentGatewayMethod("Purchase Order Method Editors",
        "",
        "",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.voidpayment.html",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.refundpayment.html",
        "")]
    public class PurchaseOrderPaymentGatewayMethod : PaymentGatewayMethodBase, IPurchaseOrderPaymentGatewayMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrderPaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        public PurchaseOrderPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod)
            : base(gatewayProviderService, paymentMethod)
        {
        }

        /// <summary>
        /// Does the actual work of creating and processing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment.</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            // Check if an amount is passed
            var authorizeAmount = invoice.Total;
            if (args.ContainsKey("authorizePaymentAmount"))
            {
                authorizeAmount = Convert.ToDecimal(args["authorizePaymentAmount"]);
            }

            var po = args.AsPurchaseOrderFormData();

            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.PurchaseOrder, authorizeAmount, PaymentMethod.Key);
            payment.CustomerKey = invoice.CustomerKey;
            payment.PaymentMethodName = PaymentMethod.Name;
            payment.ReferenceNumber = PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();
            payment.Collected = false;
            payment.Authorized = true;

            if (string.IsNullOrEmpty(po.PurchaseOrderNumber))
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Error Purchase Order Number is empty")), invoice, false);
            }

            invoice.PoNumber = po.PurchaseOrderNumber;
            MerchelloContext.Current.Services.InvoiceService.Save(invoice);

            GatewayProviderService.Save(payment);

            // In this case, we want to do our own Apply Payment operation as the amount has not been collected -
            // so we create an applied payment with a 0 amount.  Once the payment has been "collected", another Applied Payment record will
            // be created showing the full amount and the invoice status will be set to Paid.
            GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, string.Format("To show promise of a {0} payment", PaymentMethod.Name), 0);

            //// If this were using a service we might want to store some of the transaction data in the ExtendedData for record
            ////payment.ExtendData

            return new PaymentResult(Attempt.Succeed(payment), invoice, false);
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
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.PurchaseOrder, amount, PaymentMethod.Key);
            payment.CustomerKey = invoice.CustomerKey;
            payment.PaymentMethodName = PaymentMethod.Name;
            payment.ReferenceNumber = PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();
            payment.Collected = true;
            payment.Authorized = true;

            var po = args.AsPurchaseOrderFormData();

            if (string.IsNullOrEmpty(po.PurchaseOrderNumber))
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Error Purchase Order Number is empty")), invoice, false);
            }

            invoice.PoNumber = po.PurchaseOrderNumber;
            MerchelloContext.Current.Services.InvoiceService.Save(invoice);

            GatewayProviderService.Save(payment);

            GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Cash payment", amount);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, CalculateTotalOwed(invoice).CompareTo(amount) <= 0);
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
            // We need to determine if the entire amount authorized has been collected before marking
            // the payment collected.
            var appliedPayments = GatewayProviderService.GetAppliedPaymentsByPaymentKey(payment.Key);
            var applied = appliedPayments.Sum(x => x.Amount);

            var newTotalPaymentAmount = amount + applied;

            // There could be an adjustment, and the capture amount could be more than the payment amount
            if (newTotalPaymentAmount > payment.Amount)
            {
                // We are capturing more money so update payment total
                payment.Amount = newTotalPaymentAmount;
            }

            payment.Collected = newTotalPaymentAmount == payment.Amount;
            payment.Authorized = true;

            GatewayProviderService.Save(payment);

            GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Purchase Order Payment", amount);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, CalculateTotalOwed(invoice).CompareTo(amount) <= 0);
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
            foreach (var applied in payment.AppliedPayments())
            {
                applied.TransactionType = AppliedPaymentType.Refund;
                applied.Amount = 0;
                applied.Description += " - Refunded";
                GatewayProviderService.Save(applied);
            }

            payment.Amount = payment.Amount - amount;

            if (payment.Amount != 0)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "To show partial payment remaining after refund", payment.Amount);
            }

            GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
        }
    }
}