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
        IPayPalApiPaymentService Payment { get; }
    }
}