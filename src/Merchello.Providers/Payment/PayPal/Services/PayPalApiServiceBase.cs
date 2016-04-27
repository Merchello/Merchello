namespace Merchello.Providers.Payment.PayPal.Services
{
    using System;
    using System.Net;

    using Merchello.Providers.Payment.PayPal.Models;

    using global::PayPal;

    using Merchello.Core.Events;
    using Merchello.Core.Logging;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// A base class of <see cref="IPayPalApiServiceBase"/>s.
    /// </summary>
    public class PayPalApiServiceBase : IPayPalApiServiceBase
    {
        /// <summary>
        /// The <see cref="PayPalProviderSettings"/>.
        /// </summary>
        private readonly PayPalProviderSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiServiceBase"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        protected PayPalApiServiceBase(PayPalProviderSettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");
            _settings = settings;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        internal PayPalProviderSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns>
        /// The access token.
        /// </returns>
        protected APIContext GetApiContext()
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit = 9999;

                var attempt = _settings.GetApiSdkConfig();

                if (!attempt.Success) throw attempt.Exception;

                var accessToken = new OAuthTokenCredential(_settings.ClientId, _settings.ClientSecret, attempt.Result).GetAccessToken();

                return new APIContext(accessToken);
            }
            catch (Exception ex)
            {
                var logData = GetLoggerData();
                MultiLogHelper.Error<PayPalApiServiceBase>("Failed to create PayPal APIContext", ex, logData);
                throw;
            }

        }

        /// <summary>
        /// Gets the extended logger data.
        /// </summary>
        /// <returns>
        /// The <see cref="IExtendedLoggerData"/>.
        /// </returns>
        protected IExtendedLoggerData GetLoggerData()
        {
            var logData = MultiLogger.GetBaseLoggingData();

            logData.AddCategory("PayPal");

            return logData;
        }
    }
}