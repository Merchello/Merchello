namespace Merchello.Providers.Payment.Braintree.Services
{
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the <see cref="BraintreeApiService"/>.
    /// </summary>
    public interface IBraintreeApiService : IService
    {
        /// <summary>
        /// Gets the <see cref="BraintreeProviderSettings"/>.
        /// </summary>
        BraintreeProviderSettings BraintreeProviderSettings { get; }

        /// <summary>
        /// Gets the <see cref="IBraintreeCustomerApiService"/>.
        /// </summary>
        IBraintreeCustomerApiService Customer { get; }

        /// <summary>
        /// Gets the <see cref="IBraintreePaymentMethodApiService"/>.
        /// </summary>
        IBraintreePaymentMethodApiService PaymentMethod { get; }

        /// <summary>
        /// Gets the <see cref="IBraintreeSubscriptionApiService"/>.
        /// </summary>
        IBraintreeSubscriptionApiService Subscription { get; }

        /// <summary>
        /// Gets the <see cref="IBraintreeTransactionApiService"/>.
        /// </summary>
        IBraintreeTransactionApiService Transaction { get; }

        /// <summary>
        /// Gets the <see cref="IBraintreeWebhooksApiService"/>
        /// </summary>
        IBraintreeWebhooksApiService Webhook { get; }
    }
}