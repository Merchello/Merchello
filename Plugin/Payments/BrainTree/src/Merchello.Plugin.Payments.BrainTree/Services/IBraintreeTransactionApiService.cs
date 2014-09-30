namespace Merchello.Plugin.Payments.Braintree.Services
{
    using global::Braintree;
    using Core.Gateways.Payment;
    using Core.Models;
    using Models;

    /// <summary>
    /// Defines the BraintreeTransactionApiProvider.
    /// </summary>
    public interface IBraintreeTransactionApiService
    {
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
        /// The transaction option.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        Result<Transaction> Sale(IInvoice invoice, string paymentMethodNonce, ICustomer customer = null, TransactionOption option = TransactionOption.SubmitForSettlement);

        /// <summary>
        /// The sale.
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
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        Result<Transaction> Sale(IInvoice invoice, string paymentMethodNonce, ICustomer customer, IAddress billingAddress, TransactionOption option = TransactionOption.SubmitForSettlement);

        /// <summary>
        /// Performs a Braintree sales transaction
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
        /// The transaction option.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        Result<Transaction> Sale(IInvoice invoice, string paymentMethodNonce, ICustomer customer, IAddress billingAddress, IAddress shippingAddress, TransactionOption option = TransactionOption.SubmitForSettlement);

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
    }
}