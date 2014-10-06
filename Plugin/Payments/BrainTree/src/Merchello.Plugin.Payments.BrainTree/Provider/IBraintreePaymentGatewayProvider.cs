namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using Services;

    /// <summary>
    /// Marker interface for the BraintreePaymentGatewayProvider.
    /// </summary>
    public interface IBraintreePaymentGatewayProvider
    {
        /// <summary>
        /// Gets the <see cref="IBraintreeApiService"/>.
        /// </summary>
        IBraintreeApiService BraintreeApiService { get; }
    }
}