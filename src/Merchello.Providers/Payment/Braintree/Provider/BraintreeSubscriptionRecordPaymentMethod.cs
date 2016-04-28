namespace Merchello.Providers.Payment.Braintree.Provider
{
    using System.Linq;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.Braintree.Services;

    using Newtonsoft.Json;

    using Umbraco.Core;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// The braintree subscription record payment method.
    /// </summary>
    [GatewayMethodUi("BrainTree.SubscriptionRecordTransaction")]
    [GatewayMethodEditor("BrainTree Payment Method Editor", "~/App_Plugins/MerchelloProviders/views/dialogs/braintee.paymentmethod.addedit.html")]
    [PaymentGatewayMethod("Braintree Payment Gateway Method Editors", false)]
    public class BraintreeSubscriptionRecordPaymentMethod : PaymentGatewayMethodBase, IBraintreeSubscriptionRecordPaymentGatewayMethod
    {
        /// <summary>
        /// The <see cref="IBraintreeApiService"/>.
        /// </summary>
        private readonly IBraintreeApiService _braintreeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeSubscriptionRecordPaymentMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="braintreeApiService">The <see cref="IBraintreeApiService"/></param>
        public BraintreeSubscriptionRecordPaymentMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService)
            : base(gatewayProviderService, paymentMethod)
        {
            Mandate.ParameterNotNull(braintreeApiService, "braintreeApiService");

            this._braintreeApiService = braintreeApiService;
        }

        /// <summary>
        /// The perform authorize payment.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            var payment = this.GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, invoice.Total, this.PaymentMethod.Key);
            payment.CustomerKey = invoice.CustomerKey;
            payment.PaymentMethodName = this.PaymentMethod.Name;
            payment.ReferenceNumber = this.PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();
            payment.Collected = false;
            payment.Authorized = true;

            this.GatewayProviderService.Save(payment);

            // In this case, we want to do our own Apply Payment operation as the amount has not been collected -
            // so we create an applied payment with a 0 amount.  Once the payment has been "collected", another Applied Payment record will
            // be created showing the full amount and the invoice status will be set to Paid.
            this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, string.Format("To show promise of a {0} payment", this.PaymentMethod.Name), 0);

            return new PaymentResult(Attempt.Succeed(payment), invoice, false);
        }

        /// <summary>
        /// The perform authorize capture payment.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
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
        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            var payment = this.GatewayProviderService.CreatePayment(PaymentMethodType.Cash, amount, this.PaymentMethod.Key);
            payment.CustomerKey = invoice.CustomerKey;
            payment.PaymentMethodName = this.PaymentMethod.Name + " " + this.PaymentMethod.PaymentCode;
            payment.ReferenceNumber = this.PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();
            payment.Collected = true;
            payment.Authorized = true;

            string transaction;
            if (args.TryGetValue(Constants.Braintree.ExtendedDataKeys.BraintreeTransaction, out transaction))
            {
                payment.ExtendedData.SetValue(Constants.Braintree.ExtendedDataKeys.BraintreeTransaction, transaction);
            }

            this.GatewayProviderService.Save(payment);

            this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Braintree subscription payment", amount);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, this.CalculateTotalOwed(invoice).CompareTo(amount) <= 0);
        }

        /// <summary>
        /// The perform capture payment.
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
            payment.Collected = true;
            payment.Authorized = true;

            string transaction;
            if (args.TryGetValue(Constants.Braintree.ExtendedDataKeys.BraintreeTransaction, out transaction))
            {
                payment.ExtendedData.SetValue(Constants.Braintree.ExtendedDataKeys.BraintreeTransaction, transaction);
            }

            this.GatewayProviderService.Save(payment);

            this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Braintree subscription payment", amount);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, this.CalculateTotalOwed(invoice).CompareTo(amount) <= 0);
        }

        /// <summary>
        /// The perform refund payment.
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
        protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            foreach (var applied in payment.AppliedPayments())
            {
                applied.TransactionType = AppliedPaymentType.Refund;
                applied.Amount = 0;
                applied.Description += " - Refunded";
                this.GatewayProviderService.Save(applied);
            }

            payment.Amount = payment.Amount - amount;

            if (payment.Amount != 0)
            {
                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "To show partial payment remaining after refund", payment.Amount);
            }

            this.GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
        }

        /// <summary>
        /// The perform void payment.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            string transactionString;
            if (args.TryGetValue(Constants.Braintree.ExtendedDataKeys.BraintreeTransaction, out transactionString))
            {
                var transaction = JsonConvert.DeserializeObject<Transaction>(transactionString);
                var result = this._braintreeApiService.Transaction.Refund(transaction.Id);

                if (!result.IsSuccess()) return new PaymentResult(Attempt<IPayment>.Fail(payment), invoice, false);  

                foreach (var applied in payment.AppliedPayments())
                {
                    applied.TransactionType = AppliedPaymentType.Void;
                    applied.Amount = 0;
                    applied.Description += " - **Void**";
                    this.GatewayProviderService.Save(applied);
                }

                payment.Voided = true;
                this.GatewayProviderService.Save(payment);

                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
            }
            
            return new PaymentResult(Attempt<IPayment>.Fail(payment), invoice, false);
        }
    }
}