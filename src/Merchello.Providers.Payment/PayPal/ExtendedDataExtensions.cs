namespace Merchello.Providers.Payment.PayPal
{
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.PayPal.Models;

    using Newtonsoft.Json;


    /// <summary>
	/// Extended data utility extensions
	/// </summary>
	public static class PayPalExtendedDataExtensions
	{
		/// <summary>
		/// Saves the processor settings to an extended data collection
		/// </summary>
		/// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
		/// <param name="providerSettings">The <see cref="PayPalProviderSettings"/> to be serialized and saved</param>
		public static void SaveProcessorSettings(this ExtendedDataCollection extendedData, PayPalProviderSettings providerSettings)
		{
			var settingsJson = JsonConvert.SerializeObject(providerSettings);

			extendedData.SetValue(Constants.PayPal.ExtendedDataKeys.ProcessorSettings, settingsJson);
		}

		/// <summary>
		/// Get the processor settings from the extended data collection
		/// </summary>
		/// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
		/// <returns>The deserialized <see cref="PayPalProviderSettings"/></returns>
		public static PayPalProviderSettings GetProcessorSettings(this ExtendedDataCollection extendedData)
		{
			if (!extendedData.ContainsKey(Constants.PayPal.ExtendedDataKeys.ProcessorSettings)) return new PayPalProviderSettings();

			return
				JsonConvert.DeserializeObject<PayPalProviderSettings>(
					extendedData.GetValue(Constants.PayPal.ExtendedDataKeys.ProcessorSettings));
		}

	}
}
