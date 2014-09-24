namespace Merchello.Plugin.Payments.Braintree.Api
{
    using global::Braintree;

    using Merchello.Core;

    /// <summary>
    /// Represents the BraintreePaymentMethodApiProvider.
    /// </summary>
    internal class BraintreePaymentMethodApiProvider : BraintreeApiProviderBase, IBraintreePaymentMethodApiProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiProvider"/> class.
        /// </summary>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        public BraintreePaymentMethodApiProvider(BraintreeGateway braintreeGateway)
            : this(Core.MerchelloContext.Current, braintreeGateway)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        public BraintreePaymentMethodApiProvider(IMerchelloContext merchelloContext, BraintreeGateway braintreeGateway)
            : base(merchelloContext, braintreeGateway)
        {
        }
    }
}