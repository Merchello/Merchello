namespace Merchello.Plugin.Taxation.Taxjar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Plugin.Taxation.Taxjar.Models;

    using Newtonsoft.Json;

    public static class MappingExtensions
    {
        #region ExtendedData

        /// <summary>
        /// Serializes the <see cref="TaxJarProviderSettings"/> and saves them in the extend data collection.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public static void SaveProviderSettings(this ExtendedDataCollection extendedData, TaxJarProviderSettings settings)
        {
            extendedData.SetValue(TaxJarProviderSettings.ExtendedDataKey, JsonConvert.SerializeObject(settings));
        }

        /// <summary>
        /// Deserializes tax provider settings from the gateway provider's extended data collection
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// The <see cref="TaxJarProviderSettings"/>.
        /// </returns>
        public static TaxJarProviderSettings GetTaxJarProviderSettings(this ExtendedDataCollection extendedData)
        {
            return JsonConvert.DeserializeObject<TaxJarProviderSettings>(extendedData.GetValue(TaxJarProviderSettings.ExtendedDataKey));
        }

        #endregion
    }
}
