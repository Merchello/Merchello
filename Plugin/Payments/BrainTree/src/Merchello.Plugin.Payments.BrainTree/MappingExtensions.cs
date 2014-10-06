namespace Merchello.Plugin.Payments.Braintree
{
    using AutoMapper;
    using global::Braintree;
    using Core.Gateways.Payment;
    using Core.Models;
    using Models;
    using Newtonsoft.Json;
    using Umbraco.Core.Logging;

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

        #region ExtendedData

        /// <summary>
        /// Serializes and saves a <see cref="Transaction"/> in extended data
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        public static void SetBraintreeTransaction(this ExtendedDataCollection extendedData, Transaction transaction)
        {
            extendedData.SetValue(Constants.ExtendedDataKeys.BraintreeTransaction, JsonConvert.SerializeObject(transaction));
        }

        /// <summary>
        /// Deserializes and returns a <see cref="Transaction"/> from the extended data collection
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// The <see cref="Transaction"/>.
        /// </returns>
        public static Transaction GetBraintreeTransaction(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.BraintreeTransaction)
                ? JsonConvert.DeserializeObject<Transaction>(
                    extendedData.GetValue(Constants.ExtendedDataKeys.BraintreeTransaction))
                : null;
        }

        #endregion

        #region ProcessorArguments

        /// <summary>
        /// Gets the payment method nonce from the <see cref="ProcessorArgumentCollection"/>
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The payment method nonce.
        /// </returns>
        public static string GetPaymentMethodNonce(this ProcessorArgumentCollection args)
        {
            if (args.ContainsKey(Constants.ProcessorArguments.PaymentMethodNonce)) return args[Constants.ProcessorArguments.PaymentMethodNonce];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Nonce not found in process argument collection");

            return string.Empty;
        }

#endregion
    }
}