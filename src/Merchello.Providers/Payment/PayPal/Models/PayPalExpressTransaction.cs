namespace Merchello.Providers.Payment.PayPal.Models
{
    /// <summary>
    /// A model for saving PayPal Express Checkout Transaction data.
    /// </summary>
    public class PayPalExpressTransaction
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the payer id.
        /// </summary>
        public string PayerId { get; set; } 
    }
}