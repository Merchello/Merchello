namespace Merchello.Providers.Payment.PayPal.Services
{
    using Merchello.Providers.Payment.PayPal.Models;

    /// <summary>
    /// The PayPal API payment service.
    /// </summary>
    internal class PayPalApiPaymentService : PayPalApiServiceBase, IPayPalApiPaymentService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiPaymentService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PayPalApiPaymentService(PayPalProviderSettings settings)
            : base(settings)
        {
        }
    }
}