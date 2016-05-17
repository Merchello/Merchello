namespace Merchello.Tests.PaymentProviders.PayPal.TestHelpers
{
    using System;

    using Merchello.Providers.Payment.PayPal.Models;

    using Newtonsoft.Json;

    /// <summary>
    /// The PayPal test helper.
    /// </summary>
    public class TestHelper
    {
        /// <summary>
        /// Gets PayPalProviderSettings.
        /// </summary>
        /// <returns>
        /// The <see cref="PayPalProviderSettings"/>.
        /// </returns>
        public static PayPalProviderSettings GetPayPalProviderSettings()
        {
            var jsonFile = string.Format("{0}\\{1}", Environment.CurrentDirectory, "..\\..\\PayPal\\extendedData.json");
            string json = System.IO.File.ReadAllText(jsonFile);
            return JsonConvert.DeserializeObject<PayPalProviderSettings>(json);
        }
    }
}