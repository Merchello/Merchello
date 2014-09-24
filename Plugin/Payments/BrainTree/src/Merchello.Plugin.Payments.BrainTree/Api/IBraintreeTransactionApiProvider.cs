namespace Merchello.Plugin.Payments.Braintree.Api
{
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;

    /// <summary>
    /// Defines the BraintreeTransactionApiProvider.
    /// </summary>
    public interface IBraintreeTransactionApiProvider
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
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        IPaymentResult Sale(IInvoice invoice, string paymentMethodNonce = "", ICustomer customer = null);
    }
}