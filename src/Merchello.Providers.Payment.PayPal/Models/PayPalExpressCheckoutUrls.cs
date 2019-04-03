namespace Merchello.Providers.Payment.PayPal.Models
{
    /// <summary>
    /// A model for exposing PayPal API return in cancel URLs in an event so that they can be overridden.
    /// </summary>
    public class PayPalExpressCheckoutUrls
    {
        /// <summary>
        /// Gets or sets the return URL.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the cancel URL.
        /// </summary>
        public string CancelUrl { get; set; } 
    }
}