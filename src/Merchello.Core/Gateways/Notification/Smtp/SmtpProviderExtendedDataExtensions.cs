using System.Data;
using Merchello.Core.Models;
using Newtonsoft.Json;

namespace Merchello.Core.Gateways.Notification.Smtp
{
    /// <summary>
    /// Extension methods for Smtp Gateway Provider Extended Data Collection
    /// </summary>
    public static class SmtpProviderExtendedDataExtensions
    {
        /// <summary>
        /// Utility to get SMTP Gateway Provider settings from the <see cref="ExtendedDataCollection"/>
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        public static SmtpNotificationGatewayProviderSettings GetSmtpProviderSettings(this ExtendedDataCollection extendedData)
        {

            if(!extendedData.ContainsKey(Constants.ExtendedDataKeys.SmtpProviderSettings)) return new SmtpNotificationGatewayProviderSettings();

            return
                JsonConvert.DeserializeObject<SmtpNotificationGatewayProviderSettings>(
                    extendedData.GetValue(Constants.ExtendedDataKeys.SmtpProviderSettings));

        }

        /// <summary>
        /// Saves <see cref="SmtpNotificationGatewayProviderSettings"/> to an ExtendedDataCollection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <param name="settings">The <see cref="SmtpNotificationGatewayProviderSettings"/> to save</param>
        public static void SaveSmtpProviderSettings(this ExtendedDataCollection extendedData, SmtpNotificationGatewayProviderSettings settings)
        {
            var settingsJson = JsonConvert.SerializeObject(settings);

            extendedData.SetValue(Constants.ExtendedDataKeys.SmtpProviderSettings, settingsJson);
        }

    }
}