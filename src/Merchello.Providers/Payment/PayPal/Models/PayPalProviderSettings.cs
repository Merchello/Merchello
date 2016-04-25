namespace Merchello.Providers.Payment.PayPal.Models
{
    using Merchello.Providers.Payment.Models;
    using Merchello.Providers.Payment.PayPal;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// PayPal Provider settings.
    /// </summary>
    public class PayPalProviderSettings : IPaymentProviderSettings
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalProviderSettings"/> class.
        /// </summary>
        public PayPalProviderSettings()
        {
            this.Mode = PayPalMode.Sandbox;
            ClientId = string.Empty;
            ClientSecret = string.Empty;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the site is in live mode.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PayPalMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; }
	}
}
