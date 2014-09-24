namespace Merchello.Plugin.Payments.Braintree.Api
{
    /// <summary>
    /// Defines the <see cref="BraintreeApiProvider"/>.
    /// </summary>
    internal interface IBraintreeApiProvider
    {
        /// <summary>
        /// Gets the <see cref="IBraintreeCustomerApiProvider"/>.
        /// </summary>
        IBraintreeCustomerApiProvider Customer { get; }

        /// <summary>
        /// Gets the <see cref="IBraintreePaymentMethodApiProvider"/>.
        /// </summary>
        IBraintreePaymentMethodApiProvider PaymentMethod { get; }

        /// <summary>
        /// Gets the <see cref="IBraintreeSubscriptionApiProvider"/>.
        /// </summary>
        IBraintreeSubscriptionApiProvider Subscription { get; }

        /// <summary>
        /// Gets the <see cref="IBraintreeTransactionApiProvider"/>.
        /// </summary>
        IBraintreeTransactionApiProvider Transaction { get; }
    }
}