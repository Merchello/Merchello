namespace Merchello.Plugin.Payments.Epay.Models
{
    /// <summary>
    /// The Epay processor settings.
    /// </summary>
    public class EpayProcessorSettings
    {
        /// <summary>
        /// Gets or sets the merchant number.
        /// </summary>
        public string MerchantNumber { get; set; }

        /// <summary>
        /// Gets or sets the API password.
        /// </summary>
        public string Password { get; set; }
    }
}