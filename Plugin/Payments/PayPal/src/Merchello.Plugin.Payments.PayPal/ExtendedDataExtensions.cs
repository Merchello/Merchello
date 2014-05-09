using Merchello.Core.Models;
using Merchello.Plugin.Payments.PayPal.Models;
using Newtonsoft.Json;

namespace Merchello.Plugin.Payments.PayPal
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
		/// <param name="processorSettings">The <see cref="PayPalProcessorSettings"/> to be serialized and saved</param>
		public static void SaveProcessorSettings(this ExtendedDataCollection extendedData, PayPalProcessorSettings processorSettings)
		{
			var settingsJson = JsonConvert.SerializeObject(processorSettings);

			extendedData.SetValue(Constants.ExtendedDataKeys.ProcessorSettings, settingsJson);
		}

		/// <summary>
		/// Get teh processor settings from the extended data collection
		/// </summary>
		/// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
		/// <returns>The deserialized <see cref="PayPalProcessorSettings"/></returns>
		public static PayPalProcessorSettings GetProcessorSettings(this ExtendedDataCollection extendedData)
		{
			if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.ProcessorSettings)) return new PayPalProcessorSettings();

			return
				JsonConvert.DeserializeObject<PayPalProcessorSettings>(
					extendedData.GetValue(Constants.ExtendedDataKeys.ProcessorSettings));
		}
	}
}
