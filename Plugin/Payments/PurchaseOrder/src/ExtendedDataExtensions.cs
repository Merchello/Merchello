using Merchello.Core.Models;
using Merchello.Plugin.Payments.PurchaseOrder.Models;
using Newtonsoft.Json;

namespace Merchello.Plugin.Payments.PurchaseOrder
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
        /// <param name="processorSettings">The <see cref="PurchaseOrderProcessorSettings"/> to be serialized and saved</param>
        public static void SaveProcessorSettings(this ExtendedDataCollection extendedData, PurchaseOrderProcessorSettings processorSettings)
        {
            var settingsJson = JsonConvert.SerializeObject(processorSettings);

            extendedData.SetValue(Constants.ExtendedDataKeys.ProcessorSettings, settingsJson);
        }

        /// <summary>
        /// Get teh processor settings from the extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The deserialized <see cref="PurchaseOrderProcessorSettings"/></returns>
        public static PurchaseOrderProcessorSettings GetProcessorSettings(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.ProcessorSettings)) return new PurchaseOrderProcessorSettings();

            return
                JsonConvert.DeserializeObject<PurchaseOrderProcessorSettings>(
                    extendedData.GetValue(Constants.ExtendedDataKeys.ProcessorSettings));
        }
    }
}