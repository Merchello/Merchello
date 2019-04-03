namespace Merchello.Providers.Payment.PayPal
{
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.PayPal.Models;
    using Newtonsoft.Json;

    public static class PayPalExtensions
    {
        /// <summary>
        /// Gets the <see cref="PayPalProviderSettings"/>.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalProviderSettings"/>.
        /// </returns>
        public static PayPalProviderSettings GetPayPalProviderSettings(this ExtendedDataCollection extendedData)
        {
            PayPalProviderSettings settings;
            if (extendedData.ContainsKey(Constants.PayPal.ExtendedDataKeys.ProviderSettings))
            {
                var json = extendedData.GetValue(Constants.PayPal.ExtendedDataKeys.ProviderSettings);
                settings = JsonConvert.DeserializeObject<PayPalProviderSettings>(json);
            }
            else
            {
                settings = new PayPalProviderSettings();
            }

            return settings;
        }
    }
}
