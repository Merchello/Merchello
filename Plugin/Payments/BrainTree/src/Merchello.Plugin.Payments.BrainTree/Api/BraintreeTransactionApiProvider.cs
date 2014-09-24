namespace Merchello.Plugin.Payments.Braintree.Api
{
    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;

    /// <summary>
    /// Represents the <see cref="BraintreeTransactionApiProvider"/>.
    /// </summary>
    internal class BraintreeTransactionApiProvider : BraintreeApiProviderBase, IBraintreeTransactionApiProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeTransactionApiProvider"/> class.
        /// </summary>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        public BraintreeTransactionApiProvider(BraintreeGateway braintreeGateway)
            : this(Core.MerchelloContext.Current, braintreeGateway)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeTransactionApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        internal BraintreeTransactionApiProvider(IMerchelloContext merchelloContext, BraintreeGateway braintreeGateway)
            : base(merchelloContext, braintreeGateway)
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
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        public IPaymentResult Sale(IInvoice invoice, string paymentMethodNonce = "", ICustomer customer = null)
        {
            throw new System.NotImplementedException();
        }
    }
}