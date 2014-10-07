namespace Merchello.Plugin.Payments.Braintree.Services
{
    /// <summary>
    /// Defines the <see cref="BraintreeApiService"/>.
    /// </summary>
    public interface IBraintreeApiService
    {
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
    }
}