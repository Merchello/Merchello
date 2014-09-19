using System;
using AutoMapper;

namespace Merchello.Plugin.Payments.Braintree
{
    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree.Models;

    using Newtonsoft.Json;

    /// <summary>
    /// Utility extensions that assist in mapping and serializing/de-serializing models
    /// </summary>
    public static class MappingExtensions
    {
        #region BraintreeProviderSettings and BraintreeGateway

        /// <summary>
        /// Utility extension the deserializes BraintreeProviderSettings from the ExtendedDataCollection
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// The <see cref="BraintreeProviderSettings"/>.
        /// </returns>
        public static BraintreeProviderSettings GetBrainTreeProviderSettings(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.BraintreeProviderSettings)
                       ? JsonConvert.DeserializeObject<BraintreeProviderSettings>(
                           extendedData.GetValue(Constants.ExtendedDataKeys.BraintreeProviderSettings))
                       : new BraintreeProviderSettings();
        }

        /// <summary>
        /// Utility extension to quickly serialize <see cref="BraintreeProviderSettings"/> to an extended data value
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="settings">
        /// The <see cref="BraintreeProviderSettings"/>
        /// </param>
        public static void SaveProviderSettings(this ExtendedDataCollection extendedData, BraintreeProviderSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings);
            extendedData.SetValue(Constants.ExtendedDataKeys.BraintreeProviderSettings, json);
        }

        /// <summary>
        /// Maps a <see cref="BraintreeProviderSettings"/> as a <see cref="BraintreeGateway"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="BraintreeGateway"/>.
        /// </returns>
        public static BraintreeGateway AsBraintreeGateway(this BraintreeProviderSettings settings)
        {
            return Mapper.Map<BraintreeGateway>(settings);
        }

        #endregion

        #region DescriptorRequest

        /// <summary>
        /// Maps a 
        /// </summary>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="DescriptorRequest"/>.
        /// </returns>
        public static DescriptorRequest AsDescriptorRequest(this MerchantDescriptor descriptor)
        {
            return Mapper.Map<DescriptorRequest>(descriptor);
        }

        /// <summary>
        /// Provides and indication if any values have been set in <see cref="MerchantDescriptor"/>
        /// </summary>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasValues(this MerchantDescriptor descriptor)
        {
            var values = new[] { descriptor.Name, descriptor.Url, descriptor.Phone };
            var all = string.Join(" ", values).Trim();

            return !string.IsNullOrEmpty(all);
        }

        /// <summary>
        /// Provides and indication if any values have been set in <see cref="DescriptorRequest"/>
        /// </summary>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasValues(this DescriptorRequest descriptor)
        {
            var values = new[] { descriptor.Name, descriptor.Url, descriptor.Phone };
            var all = string.Join(" ", values).Trim();

            return !string.IsNullOrEmpty(all);
        }

        #endregion
    }
}