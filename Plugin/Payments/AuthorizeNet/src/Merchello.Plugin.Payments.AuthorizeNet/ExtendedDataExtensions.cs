using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.AuthorizeNet.Models;

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
            var settingsXml = SerializationHelper.SerializeToXml(processorSettings);

            extendedData.SetValue(Constants.ExtendedDataKeys.ProcessorSettings, settingsXml);
        }

        /// <summary>
        /// Get teh processor settings from the extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The deserialized <see cref="AuthorizeNetProcessorSettings"/></returns>
        public static AuthorizeNetProcessorSettings GetProcessorSettings(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.ProcessorSettings)) return null;

            var attempt = SerializationHelper.DeserializeXml<AuthorizeNetProcessorSettings>(extendedData.GetValue(Constants.ExtendedDataKeys.ProcessorSettings));

            return attempt.Success ? attempt.Result : null;
        }
    }
}