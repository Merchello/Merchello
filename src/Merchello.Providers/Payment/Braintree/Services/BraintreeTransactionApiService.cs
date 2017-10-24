namespace Merchello.Providers.Payment.Braintree.Services
{
    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the <see cref="BraintreeTransactionApiService"/>.
    /// </summary>
    internal class BraintreeTransactionApiService : BraintreeApiServiceBase, IBraintreeTransactionApiService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeTransactionApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeTransactionApiService(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {
        }

        /// <inheritdoc />
        internal BraintreeTransactionApiService(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : base(merchelloContext, settings)
        {
        }

        /// <inheritdoc />
        public Result<Transaction> Sale(IInvoice invoice, decimal amount, string paymentMethodNonce = "", ICustomer customer = null, TransactionOption option = TransactionOption.SubmitForSettlement, string email = "", string merchantAccountId = "")
        {
            var request = this.RequestFactory.CreateTransactionRequest(invoice, amount, paymentMethodNonce, customer, option, merchantAccountId);
            if (customer == null && !string.IsNullOrEmpty(email))
            {
                request.Customer = new CustomerRequest() { Email = email };
            }

            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Transaction attempt ({0}) for Invoice {1}", option.ToString(), invoice.PrefixedInvoiceNumber()));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Sale(request));

            return attempt.Success ? attempt.Result : null;
        }

        /// <inheritdoc />
        public Result<Transaction> Sale(IInvoice invoice, decimal amount, string paymentMethodNonce, ICustomer customer, IAddress billingAddress, TransactionOption option = TransactionOption.SubmitForSettlement, string merchantAccountId = "")
        {
            return this.Sale(invoice, amount, paymentMethodNonce, customer, billingAddress, null, option, merchantAccountId);
        }

        /// <inheritdoc />
        public Result<Transaction> Sale(IInvoice invoice, decimal amount, string paymentMethodNonce, ICustomer customer, IAddress billingAddress, IAddress shippingAddress, TransactionOption option = TransactionOption.SubmitForSettlement, string merchantAccountId = "")
        {
            var request = this.RequestFactory.CreateTransactionRequest(invoice, amount, paymentMethodNonce, customer, option, merchantAccountId);

            if (billingAddress != null) request.BillingAddress = this.RequestFactory.CreateAddressRequest(billingAddress);
            if (shippingAddress != null) request.ShippingAddress = this.RequestFactory.CreateAddressRequest(shippingAddress);

            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Transaction attempt ({0}) for Invoice {1}", option.ToString(), invoice.PrefixedInvoiceNumber()));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Sale(request));

            return attempt.Success ? attempt.Result : null;
        }

        /// <inheritdoc />
        public Result<Transaction> VaultSale(
            IInvoice invoice, decimal amount,
            string paymentMethodToken,
            TransactionOption option = TransactionOption.SubmitForSettlement, string merchantAccountId = "")
        {
            var request = this.RequestFactory.CreateVaultTransactionRequest(invoice, amount, paymentMethodToken, option, merchantAccountId);

            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Vault Transaction attempt ({0}) for Invoice {1}", option.ToString(), invoice.PrefixedInvoiceNumber()));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Sale(request));

            return attempt.Success ? attempt.Result : null;
        }

        /// <inheritdoc />
        public Result<Transaction> SubmitForSettlement(string transactionId)
        {
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Transaction {0} submit for settlement attempt", transactionId));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.SubmitForSettlement(transactionId));
            return attempt.Success ? attempt.Result : null;
        }

        /// <inheritdoc />
        public Result<Transaction> SubmitForSettlement(string transactionId, decimal amount)
        {
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Transaction {0} submit for settlement attempt, amount {1}", transactionId, amount));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.SubmitForSettlement(transactionId, amount));
            return attempt.Success ? attempt.Result : null;
        }

        /// <inheritdoc />
        public Result<Transaction> Refund(string transactionId)
        {
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Refund attempt for transaction {0}", transactionId));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Refund(transactionId));
            return attempt.Success ? attempt.Result : null;
        }

        /// <inheritdoc />
        public Result<Transaction> Refund(string transactionId, decimal amount)
        {
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Refund attempt for transaction {0} for amount {1}", transactionId, amount));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Refund(transactionId, amount));
            return attempt.Success ? attempt.Result : null;
        }

        /// <inheritdoc />
        public Attempt<Transaction> Find(string transactionId)
        {
            return this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Find(transactionId));
        }
    }
}