namespace Merchello.Providers.Payment.PayPal
{
    using System.Web;

    using Merchello.Core.Events;
    using Merchello.Providers.Payment.PayPal.Models;

    using Umbraco.Core.Events;

    /// <summary>
	/// The PayPal payment processor
	/// </summary>
	public class PayPalPaymentProcessor
	{
        /// <summary>
        /// The _settings.
        /// </summary>
        private readonly PayPalProviderSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalPaymentProcessor"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PayPalPaymentProcessor(PayPalProviderSettings settings)
        {
            this._settings = settings;
        }

        /// <summary>
        /// Occurs before applying decimal places to <see cref="CurrencyCodeTypeDecimal"/>.
        /// </summary>
        public static event TypedEventHandler<PayPalPaymentProcessor, ObjectEventArgs<CurrencyCodeTypeDecimal>> ApplyingCurrencyDecimalPlaces;  

        /// <summary>
        /// Get the absolute base URL for this website
        /// </summary>
        /// <returns>
        /// The root URL for the current website
        /// </returns>
        private static string GetWebsiteUrl()
		{
			var url = HttpContext.Current.Request.Url;
			var baseUrl = string.Format("{0}://{1}{2}", url.Scheme, url.Host, url.IsDefaultPort ? string.Empty : ":" + url.Port);
			return baseUrl;
		}
	}
}
