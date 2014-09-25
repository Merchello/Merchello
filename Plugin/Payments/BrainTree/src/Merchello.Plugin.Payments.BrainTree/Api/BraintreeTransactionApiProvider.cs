namespace Merchello.Plugin.Payments.Braintree.Api
{
    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree.Models;

    /// <summary>
    /// Represents the <see cref="BraintreeTransactionApiProvider"/>.
    /// </summary>
    internal class BraintreeTransactionApiProvider : BraintreeApiProviderBase, IBraintreeTransactionApiProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeTransactionApiProvider"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeTransactionApiProvider(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeTransactionApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        internal BraintreeTransactionApiProvider(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
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
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        public IPaymentResult Sale(IInvoice invoice, string paymentMethodNonce = "", ICustomer customer = null)
        {
            return Sale(invoice, paymentMethodNonce, customer, null);
        }

        public IPaymentResult Sale(IInvoice invoice, string paymentMethodNonce, ICustomer customer, IAddress billingAddress)
        {
            throw new System.NotImplementedException();
        }

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
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        public IPaymentResult Sale(IInvoice invoice, string paymentMethodNonce, ICustomer customer, IAddress billingAddress = null, IAddress shippingAddress = null)
        {
            throw new System.NotImplementedException();
        }
    }
}