namespace Merchello.Providers.Payment.Braintree
{
    using System.Diagnostics.CodeAnalysis;

    using AutoMapper;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.Braintree.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Umbraco.Core.Logging;

    using Constants = Merchello.Providers.Payment.Constants;

    /// <summary>
    /// Utility extensions that assist in mapping and serializing/de-serializing models
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    public static class MappingExtensions
    {

        #region Address

        /// <summary>
        /// Converts a braintree address to a <see cref="IAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress ToAddress(this global::Braintree.Address address, AddressType addressType)
        {
            return new Core.Models.Address()
                       {
                           Name = string.Format("{0} {1}", address.FirstName, address.LastName),
                           Address1 = address.StreetAddress,
                           Locality = address.Locality,
                           Region = address.Region,
                           PostalCode = address.PostalCode,
                           CountryCode = address.CountryCodeAlpha2,
                           AddressType = addressType
                       };
        }

        #endregion

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
            BraintreeProviderSettings settings;
            if (extendedData.ContainsKey(Constants.Braintree.ExtendedDataKeys.BraintreeProviderSettings))
            {
                var json = extendedData.GetValue(Constants.Braintree.ExtendedDataKeys.BraintreeProviderSettings);
                settings = JsonConvert.DeserializeObject<BraintreeProviderSettings>(json);
            }
            else
            {
                settings = new BraintreeProviderSettings();
            }
            return settings;
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
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(settings, Formatting.None, jsonSerializerSettings);
            extendedData.SetValue(Constants.Braintree.ExtendedDataKeys.BraintreeProviderSettings, json);
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
            return new BraintreeGateway(settings.Environment.AsEnvironment(), settings.MerchantId, settings.PublicKey, settings.PrivateKey);
        }

        /// <summary>
        /// Maps an <see cref="EnvironmentType"/> to an <see cref="Environment"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Environment"/>.
        /// </returns>
        private static Environment AsEnvironment(this EnvironmentType type)
        {
            switch (type)
            {
                case EnvironmentType.Development:
                    return Environment.DEVELOPMENT;

                case EnvironmentType.Production:
                    return Environment.PRODUCTION;

                case EnvironmentType.Qa:
                    return Environment.QA;

                default:
                    return Environment.SANDBOX;
            }
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
            var mapped = Mapper.Map<DescriptorRequest>(descriptor);

            mapped.Name = mapped.Name.Replace(" ", "*");

            if (mapped.Name.Length < 22)
            {
                mapped.Name = mapped.Name.PadRight(22, '*');
            }
            else
            {
                mapped.Name = mapped.Name.Substring(0, 21);
            }
            return mapped;
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
            extendedData.SetValue(Constants.Braintree.ExtendedDataKeys.BraintreeTransaction, JsonConvert.SerializeObject(transaction));
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
        public static TransactionReference GetBraintreeTransaction(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.Braintree.ExtendedDataKeys.BraintreeTransaction)
                ? JsonConvert.DeserializeObject<TransactionReference>(
                    extendedData.GetValue(Constants.Braintree.ExtendedDataKeys.BraintreeTransaction))
                : null;
        }

        #endregion

        #region ProcessorArguments

        /// <summary>
        /// Sets the payment method nonce.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        public static void SetPaymentMethodNonce(this ProcessorArgumentCollection args, string paymentMethodNonce)
        {
            args.Add(Constants.Braintree.ProcessorArguments.PaymentMethodNonce, paymentMethodNonce);
        }

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
            if (args.ContainsKey(Constants.Braintree.ProcessorArguments.PaymentMethodNonce)) return args[Constants.Braintree.ProcessorArguments.PaymentMethodNonce];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Nonce not found in process argument collection");

            return string.Empty;
        }

        /// <summary>
        /// The set payment method token.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        public static void SetPaymentMethodToken(this ProcessorArgumentCollection args, string paymentMethodToken)
        {
            args.Add(Constants.Braintree.ProcessorArguments.PaymentMethodToken, paymentMethodToken);
        }

        /// <summary>
        /// The get payment method token.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPaymentMethodToken(this ProcessorArgumentCollection args)
        {
            if (args.ContainsKey(Constants.Braintree.ProcessorArguments.PaymentMethodToken)) return args[Constants.Braintree.ProcessorArguments.PaymentMethodToken];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Token not found in process argument collection");

            return string.Empty;
        }

#endregion
    }
}