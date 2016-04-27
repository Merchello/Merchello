namespace Merchello.Providers.Payment.PayPal.Services
{
    /// <summary>
    /// Defines a PayPal API Service.
    /// </summary>
    public interface IPayPalApiService
    {
        /// <summary>
        /// Gets the <see cref="IPayPalApiPaymentService"/>.
        /// </summary>
        IPayPalApiPaymentService ApiPayment { get; }

        /// <summary>
        /// Gets the <see cref="IPayPalExpressCheckoutService"/>.
        /// </summary>
        IPayPalExpressCheckoutService ExpressCheckout { get; } 
    }
}