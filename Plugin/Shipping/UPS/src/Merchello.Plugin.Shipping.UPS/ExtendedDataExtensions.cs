using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Plugin.Shipping.UPS.Models;
using Newtonsoft.Json;

namespace Merchello.Plugin.Shipping.UPS
{
    public static class ExtendedDataExtensions
    {
        /// <summary>
        /// Saves the processor settings to an extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <param name="processorSettings">The <see cref="UPSProcessorSettings"/> to be serialized and saved</param>
        public static void SaveProcessorSettings(this ExtendedDataCollection extendedData, UPSProcessorSettings processorSettings)
        {
            var settingsJson = JsonConvert.SerializeObject(processorSettings);
        
            extendedData.SetValue(Constants.ExtendedDataKeys.ProcessorSettings, settingsJson);
        }
        
        /// <summary>
        /// Get teh processor settings from the extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The deserialized <see cref="UPSProcessorSettings"/></returns>
        public static UPSProcessorSettings GetProcessorSettings(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.ProcessorSettings)) return new UPSProcessorSettings();
        
            return
                JsonConvert.DeserializeObject<UPSProcessorSettings>(
                    extendedData.GetValue(Constants.ExtendedDataKeys.ProcessorSettings));
        }
    }
}
