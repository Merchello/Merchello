namespace Merchello.Providers.Payment.Braintree
{
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.Braintree.Models;
    using Newtonsoft.Json;

    public static class BraintreeExtensions
    {
        /// <summary>
        /// Gets the Braintree provider settings from the ExtendedDataCollection
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// The <see cref="BraintreeProviderSettings"/>.
        /// </returns>
        public static BraintreeProviderSettings GetBrainTreeProviderSettings(this ExtendedDataCollection extendedData)
        {
            BraintreeProviderSettings settings;
            if (extendedData.ContainsKey(Constants.Braintree.ExtendedDataKeys.ProviderSettings))
            {
                var json = extendedData.GetValue(Constants.Braintree.ExtendedDataKeys.ProviderSettings);
                settings = JsonConvert.DeserializeObject<BraintreeProviderSettings>(json);
            }
            else
            {
                settings = new BraintreeProviderSettings();
            }

            return settings;
        }
    }
}
