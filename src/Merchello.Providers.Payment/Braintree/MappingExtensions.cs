namespace Merchello.Providers.Payment.Braintree
{
    using System.Diagnostics.CodeAnalysis;

    using AutoMapper;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Models;

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
            return new BraintreeGateway(settings.EnvironmentType.AsEnvironment(), settings.MerchantId, settings.PublicKey, settings.PrivateKey);
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
    }
}