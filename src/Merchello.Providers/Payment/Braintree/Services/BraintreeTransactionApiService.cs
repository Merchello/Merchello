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

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeTransactionApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        internal BraintreeTransactionApiService(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : base(merchelloContext, settings)
        {
        }

        /// <summary>
        /// Performs a Braintree sales transaction.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        public Result<Transaction> Sale(IInvoice invoice, string paymentMethodNonce = "", ICustomer customer = null, TransactionOption option = TransactionOption.SubmitForSettlement, string email = "")
        {
            var request = this.RequestFactory.CreateTransactionRequest(invoice, paymentMethodNonce, customer, option);
            if (customer == null && !string.IsNullOrEmpty(email))
            {
                request.Customer = new CustomerRequest() { Email = email };
            }

            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Transaction attempt ({0}) for Invoice {1}", option.ToString(), invoice.PrefixedInvoiceNumber()));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Sale(request));

            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Performs a Braintree sales transaction.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="option">
        /// The transaction option.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        public Result<Transaction> Sale(IInvoice invoice, string paymentMethodNonce, ICustomer customer, IAddress billingAddress, TransactionOption option = TransactionOption.SubmitForSettlement)
        {
            return this.Sale(invoice, paymentMethodNonce, customer, billingAddress, null, option);
        }

        /// <summary>
        /// Performs a Braintree sales transaction.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="shippingAddress">
        /// The shipping address.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        public Result<Transaction> Sale(IInvoice invoice, string paymentMethodNonce, ICustomer customer, IAddress billingAddress, IAddress shippingAddress, TransactionOption option = TransactionOption.SubmitForSettlement)
        {
            var request = this.RequestFactory.CreateTransactionRequest(invoice, paymentMethodNonce, customer, option);

            if (billingAddress != null) request.BillingAddress = this.RequestFactory.CreateAddressRequest(billingAddress);
            if (shippingAddress != null) request.ShippingAddress = this.RequestFactory.CreateAddressRequest(shippingAddress);

            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Transaction attempt ({0}) for Invoice {1}", option.ToString(), invoice.PrefixedInvoiceNumber()));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Sale(request));

            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Performs a Braintree Transaction using a vaulted credit card.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        public Result<Transaction> VaultSale(
            IInvoice invoice,
            string paymentMethodToken,
            TransactionOption option = TransactionOption.SubmitForSettlement)
        {
            var request = this.RequestFactory.CreateVaultTransactionRequest(invoice, paymentMethodToken, option);

            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Vault Transaction attempt ({0}) for Invoice {1}", option.ToString(), invoice.PrefixedInvoiceNumber()));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Sale(request));

            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Performs a Braintree submit for settlement transaction
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        public Result<Transaction> SubmitForSettlement(string transactionId)
        {
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Transaction {0} submit for settlement attempt", transactionId));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.SubmitForSettlement(transactionId));
            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Performs a Braintree submit for settlement transaction with a specified amount
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <param name="amount">
        /// The amount of the transaction to be captured
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        public Result<Transaction> SubmitForSettlement(string transactionId, decimal amount)
        {
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Transaction {0} submit for settlement attempt, amount {1}", transactionId, amount));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.SubmitForSettlement(transactionId, amount));
            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Performs a total refund.
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        public Result<Transaction> Refund(string transactionId)
        {
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Refund attempt for transaction {0}", transactionId));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Refund(transactionId));
            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Performs a partial refund.
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        public Result<Transaction> Refund(string transactionId, decimal amount)
        {
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Refund attempt for transaction {0} for amount {1}", transactionId, amount));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Refund(transactionId, amount));
            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Find transaction
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{Transaction}"/>.
        /// </returns>
        public Attempt<Transaction> Find(string transactionId)
        {
            return this.TryGetApiResult(() => this.BraintreeGateway.Transaction.Find(transactionId));
        }
    }
}