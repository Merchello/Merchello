namespace Merchello.Providers.Payment.Braintree.Services
{
    using global::Braintree;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.Braintree.Models;

    using Umbraco.Core;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the BraintreeTransactionApiProvider.
    /// </summary>
    public interface IBraintreeTransactionApiService : IService
    {
        /// <summary>
        /// Performs a Braintree sales transaction.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="amount">
        /// The amount
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="option">
        /// The transaction option.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        Result<Transaction> Sale(IInvoice invoice, decimal amount, string paymentMethodNonce, ICustomer customer = null, TransactionOption option = TransactionOption.SubmitForSettlement, string email = "");

        /// <summary>
        /// The sale.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="amount">
        /// The amount
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
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        Result<Transaction> Sale(IInvoice invoice, decimal amount, string paymentMethodNonce, ICustomer customer, IAddress billingAddress, TransactionOption option = TransactionOption.SubmitForSettlement);

        /// <summary>
        /// Performs a Braintree sales transaction
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="amount">
        /// The amount
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
        /// The transaction option.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        Result<Transaction> Sale(IInvoice invoice, decimal amount, string paymentMethodNonce, ICustomer customer, IAddress billingAddress, IAddress shippingAddress, TransactionOption option = TransactionOption.SubmitForSettlement);

        /// <summary>
        /// Performs a Braintree Transaction using a vaulted credit card.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="amount">
        /// The amount
        /// </param>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        Result<Transaction> VaultSale(IInvoice invoice, decimal amount, string paymentMethodToken, TransactionOption option = TransactionOption.SubmitForSettlement);

        /// <summary>
        /// Performs a Braintree submit for settlement transaction
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        Result<Transaction> SubmitForSettlement(string transactionId);

        /// <summary>
        /// Performs a Braintree submit for settlement transaction with a specified amount
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <param name="amount">The amount of the transaction to be captured</param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        Result<Transaction> SubmitForSettlement(string transactionId, decimal amount);
            
        /// <summary>
        /// Performs a total refund.
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <returns>
        /// The <see cref="Result{Transaction}"/>.
        /// </returns>
        Result<Transaction> Refund(string transactionId);

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
        Result<Transaction> Refund(string transactionId, decimal amount);

        /// <summary>
        /// Find transaction
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{Transaction}"/>.
        /// </returns>
        Attempt<Transaction> Find(string transactionId);
    }
}