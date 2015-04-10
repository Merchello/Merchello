using Merchello.Core.Models;
using Merchello.Plugin.Payments.AuthorizeNet.Models;
using Newtonsoft.Json;

namespace Merchello.Plugin.Payments.AuthorizeNet
{
    /// <summary>
    /// Extended data utiltity extensions
    /// </summary>
    public static class ExtendedDataExtensions
    {
        /// <summary>
        /// Saves the processor settings to an extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <param name="processorSettings">The <see cref="AuthorizeNetProcessorSettings"/> to be serialized and saved</param>
        public static void SaveProcessorSettings(this ExtendedDataCollection extendedData, AuthorizeNetProcessorSettings processorSettings)
        {
            var settingsJson = JsonConvert.SerializeObject(processorSettings);

            extendedData.SetValue(Constants.ExtendedDataKeys.ProcessorSettings, settingsJson);
        }

        /// <summary>
        /// Get the processor settings from the extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The deserialized <see cref="AuthorizeNetProcessorSettings"/></returns>
        public static AuthorizeNetProcessorSettings GetProcessorSettings(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.ProcessorSettings)) return new AuthorizeNetProcessorSettings();

            return
                JsonConvert.DeserializeObject<AuthorizeNetProcessorSettings>(
                    extendedData.GetValue(Constants.ExtendedDataKeys.ProcessorSettings));
        }
    }
}