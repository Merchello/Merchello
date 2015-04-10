namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Plugin.Payments.Braintree.Exceptions;
    using Merchello.Plugin.Payments.Braintree.Models;
    using Merchello.Plugin.Payments.Braintree.Services;

    using Umbraco.Core;

    /// <summary>
    /// The braintree payment gateway method base.
    /// </summary>
    public abstract class BraintreePaymentGatewayMethodBase : PaymentGatewayMethodBase, IBraintreeVaultTransactionPaymentGatewayMethod
    {
        /// <summary>
        /// The <see cref="IBraintreeApiService"/>.
        /// </summary>
        private readonly IBraintreeApiService _braintreeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentGatewayMethodBase"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="braintreeApiService">
        /// The braintree api service.
        /// </param>
        protected BraintreePaymentGatewayMethodBase(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService)
            : base(gatewayProviderService, paymentMethod)
        {
            Mandate.ParameterNotNull(braintreeApiService, "braintreeApiService");

            _braintreeApiService = braintreeApiService;
        }

        /// <summary>
        /// Gets the braintree api service.
        /// </summary>
        protected IBraintreeApiService BraintreeApiService
        {
            get
            {
                return _braintreeApiService;
            }
        }

        /// <summary>
        /// Performs the actual work of capturing the payment.
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
            var transaction = payment.ExtendedData.GetBraintreeTransaction();

            if (transaction == null)
            {
                var error = new NullReferenceException("Braintree transaction could not be found and/or deserialized from payment extended data collection");
                return new PaymentResult(Attempt<IPayment>.Fail(payment, error), invoice, false);
            }

            var attempt = BraintreeApiService.Transaction.SubmitForSettlement(transaction.Id);

            if (!attempt.IsSuccess())
            {
                var error = new BraintreeApiException(attempt.Errors, attempt.Message);
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, error.Message, 0);
                return new PaymentResult(Attempt<IPayment>.Fail(payment, error), invoice, false);
            }

            payment.Collected = true;
            this.GatewayProviderService.Save(payment);
            this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, attempt.Message, amount);
            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
        }

        /// <summary>
        /// Performs the actual work of performing the refund.
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
            var transaction = payment.ExtendedData.GetBraintreeTransaction();

            if (transaction == null)
            {
                var error = new NullReferenceException("Braintree transaction could not be found and/or deserialized from payment extended data collection");
                return new PaymentResult(Attempt<IPayment>.Fail(payment, error), invoice, false);
            }

            var attempt = BraintreeApiService.Transaction.Refund(transaction.Id, amount);

            if (!attempt.IsSuccess())
            {
                var error = new BraintreeApiException(attempt.Errors, attempt.Message);
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Refund, error.Message, 0);
                return new PaymentResult(Attempt<IPayment>.Fail(payment, error), invoice, false);
            }

            payment.Amount = payment.Amount - amount;

            if (payment.Amount != 0)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "To show partial payment remaining after refund", payment.Amount);
            }

            GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
        }

        /// <summary>
        /// Performs the actual work of voiding a payment in Merchello ONLY
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
        /// <remarks>
        /// This merely VOIDs the payment in Merchello and does nothing against the Braintree API as Braintree does not do a VOID
        /// </remarks>
        protected override IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            payment.Voided = true;
            payment.Amount = 0;

            foreach (var applied in payment.AppliedPayments(GatewayProviderService))
            {
                applied.TransactionType = AppliedPaymentType.Void;
                applied.Amount = 0;
                applied.Description += " - Voided";
                GatewayProviderService.Save(applied);
            }

            GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
        }

        /// <summary>
        /// Processes a payment against the Braintree API using the BraintreeApiService.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="token">
        /// The payment method nonce.
        /// </param>
        /// <param name="email">The email</param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        /// <remarks>
        /// This converts the <see cref="Result{Transaction}"/> into Merchello's <see cref="IPaymentResult"/>
        /// </remarks>
        protected abstract IPaymentResult ProcessPayment(IInvoice invoice, TransactionOption option, decimal amount, string token, string email = "");
        
    }
}