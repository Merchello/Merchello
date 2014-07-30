using Merchello.Core.Models;
using Merchello.Plugin.Shipping.USPS.Models;
using Newtonsoft.Json;

namespace Merchello.Plugin.Shipping.USPS
{
    public static class ExtendedDataExtensions
    {
        /// <summary>
        /// Saves the processor settings to an extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <param name="processorSettings">The <see cref="UspsProcessorSettings"/> to be serialized and saved</param>
        public static void SaveProcessorSettings(this ExtendedDataCollection extendedData, UspsProcessorSettings processorSettings)
        {
            var settingsJson = JsonConvert.SerializeObject(processorSettings);

            extendedData.SetValue(Constants.ExtendedDataKeys.ProcessorSettings, settingsJson);
        }

        /// <summary>
        /// Get teh processor settings from the extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The deserialized <see cref="UspsProcessorSettings"/></returns>
        public static UspsProcessorSettings GetProcessorSettings(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.ProcessorSettings)) return new UspsProcessorSettings();

            return
                JsonConvert.DeserializeObject<UspsProcessorSettings>(
                    extendedData.GetValue(Constants.ExtendedDataKeys.ProcessorSettings));
        }
    }
}
