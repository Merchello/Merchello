namespace Merchello.Plugin.Payments.Braintree.Api
{
    using global::Braintree;

    using Merchello.Core;

    /// <summary>
    /// Represents a BraintreeSubscriptionApiProvider.
    /// </summary>
    internal class BraintreeSubscriptionApiProvider : BraintreeApiProviderBase, IBraintreeSubscriptionApiProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeSubscriptionApiProvider"/> class.
        /// </summary>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        public BraintreeSubscriptionApiProvider(BraintreeGateway braintreeGateway)
            : this(Core.MerchelloContext.Current, braintreeGateway)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeSubscriptionApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        internal BraintreeSubscriptionApiProvider(IMerchelloContext merchelloContext, BraintreeGateway braintreeGateway)
            : base(merchelloContext, braintreeGateway)
        {
        }
    }
}