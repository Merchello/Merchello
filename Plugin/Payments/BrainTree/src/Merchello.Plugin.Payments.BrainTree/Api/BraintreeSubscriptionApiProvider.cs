namespace Merchello.Plugin.Payments.Braintree.Api
{
    using global::Braintree;

    using Merchello.Core;
    using Merchello.Plugin.Payments.Braintree.Models;

    /// <summary>
    /// Represents a BraintreeSubscriptionApiProvider.
    /// </summary>
    internal class BraintreeSubscriptionApiProvider : BraintreeApiProviderBase, IBraintreeSubscriptionApiProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeSubscriptionApiProvider"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeSubscriptionApiProvider(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeSubscriptionApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        internal BraintreeSubscriptionApiProvider(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : base(merchelloContext, settings)
        {
        }
    }
}