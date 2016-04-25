namespace Merchello.Providers.Payment.PayPal.Services
{
    
    using Merchello.Providers.Payment.PayPal.Models;

    using global::PayPal;

    using Umbraco.Core;

    /// <summary>
    /// Represents a PayPalApiService.
    /// </summary>
    public class PayPalApiService
    {
        /// <summary>
        /// The <see cref="PayPalProviderSettings"/>.
        /// </summary>
        private readonly PayPalProviderSettings _settings;

        /// <summary>
        /// The <see cref="APIContext"/>.
        /// </summary>
        private APIContext _apiContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PayPalApiService(PayPalProviderSettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");
            _settings = settings;
        }


        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns>
        /// The access token.
        /// </returns>
        private string GetAccessToken()
        {
            return string.Empty;
        }
    }
}