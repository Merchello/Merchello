namespace Merchello.Plugin.Payments.Braintree.Api
{
    using Merchello.Core;
    using Merchello.Plugin.Payments.Braintree.Models;

    /// <summary>
    /// Represents the BraintreePaymentMethodApiProvider.
    /// </summary>
    internal class BraintreePaymentMethodApiProvider : BraintreeApiProviderBase, IBraintreePaymentMethodApiProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiProvider"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreePaymentMethodApiProvider(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentMethodApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreePaymentMethodApiProvider(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : base(merchelloContext, settings)
        {
        }
    }
}