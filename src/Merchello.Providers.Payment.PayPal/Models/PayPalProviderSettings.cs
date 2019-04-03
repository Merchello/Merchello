﻿namespace Merchello.Providers.Payment.PayPal.Models
{
    using Merchello.Providers.Models;
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
            ApiUsername = string.Empty;
            ApiPassword = string.Empty;
            ApiSignature = string.Empty;
            ApplicationId = string.Empty;
            DeleteInvoiceOnCancel = false;
            AddressOverride = 0;
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

        /// <summary>
        /// Gets or sets the API username.
        /// </summary>
        public string ApiUsername { get; set; }

        /// <summary>
        /// Gets or sets the API password.
        /// </summary>
        public string ApiPassword { get; set; }

        /// <summary>
        /// Gets or sets the API signature.
        /// </summary>
        public string ApiSignature { get; set; }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the success url.
        /// </summary>
        public string SuccessUrl { get; set; }

        /// <summary>
        /// Gets or sets the retry url.
        /// </summary>
        public string RetryUrl { get; set; }

        /// <summary>
        /// Gets or sets the cancel url.
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to delete the invoice on cancel.
        /// </summary>
        public bool DeleteInvoiceOnCancel { get; set; }

        /// <summary>
        /// Gets or sets the address override.
        /// </summary>
        /// <seealso cref="https://developer.paypal.com/webapps/developer/docs/classic/api/merchant/SetExpressCheckout_API_Operation_NVP/"/>
        public int AddressOverride { get; set; }
	}
}
