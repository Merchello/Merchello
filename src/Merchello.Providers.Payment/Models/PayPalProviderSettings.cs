namespace Merchello.Providers.Payment.Models
{
    using Merchello.Providers.Payment.PayPal;

    /// <summary>
    /// PayPal Provider settings.
    /// </summary>
    public class PayPalProviderSettings : IPaymentProviderSettings
	{
        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public string AccountId { get; set; }

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
        /// Gets or sets a value indicating whether the site is in live mode.
        /// </summary>
        public bool LiveMode { get; set; }

        /// <summary>
        /// Gets or sets the article by sku path.
        /// </summary>
        /// <remarks>
        /// TODO what is this for?
        /// </remarks>
        public string ArticleBySkuPath { get; set; }

        /// <summary>
        /// Gets or sets the return url.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the cancel url.
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// Gets the api version.
        /// </summary>
        public string ApiVersion
        {
            get { return PayPalPaymentProcessor.ApiVersion; }
        }
	}
}
