namespace Merchello.Providers.Models
{
    using System;
    using System.Reflection;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    // using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Models;
    // using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Providers.Resolvers;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Umbraco.Core.Logging;


    /// <summary>
    /// The provider settings extensions.
    /// </summary>
    public static class ProviderSettingsExtensions
    {
        /// <summary>
        /// Gets the <see cref="ProviderSettingsMapperAttribute"/>.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="ProviderSettingsMapperAttribute"/>.
        /// </returns>
        public static ProviderSettingsMapperAttribute ProviderSettingsMapping(this PaymentGatewayProviderBase provider)
        {
            return provider.GetType().GetCustomAttribute<ProviderSettingsMapperAttribute>(false);
        }

        /// <summary>
        /// Gets a type <see cref="IPaymentProviderSettings"/>.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <typeparam name="T">
        /// The type of the Payment Provider Settings
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetProviderSettings<T>(this PaymentGatewayProviderBase provider)
            where T : IPaymentProviderSettings
        {
            return (T)provider.GetProviderSettings();
        }

        /// <summary>
        /// Saves the provider settings.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public static void SaveProviderSettings(this IGatewayProviderSettings record, PaymentGatewayProviderBase provider, IPaymentProviderSettings settings)
        {
            var att = provider.ProviderSettingsMapping();
            if (att == null) return;

            if (settings.GetType() == att.SettingsType)
            {
                var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var json = JsonConvert.SerializeObject(settings, Formatting.None, jsonSerializerSettings);
                record.ExtendedData.SetValue(att.Key, json);
            }
        }


        /// <summary>
        /// Gets the provider settings.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentProviderSettings"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if the ProviderSettingsResolver is not initialize
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Throw an exception if provider passed is not decorated with the ProviderSettingsMapperAttribute
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// Throws an exception if the default settings cannot be instantiated
        /// </exception>
        public static IPaymentProviderSettings GetProviderSettings(this PaymentGatewayProviderBase provider)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            var att = provider.ProviderSettingsMapping();
            if (att == null)
            {
                var invalidOp = new InvalidOperationException("Cannot use this method if the provider is not decorated with a ProviderSettingsMapperAttribute");
                MultiLogHelper.Error(typeof(ProviderSettingsExtensions), "PaymentGatewayProvider does not have attribute", invalidOp, logData);
                throw invalidOp;
            }

            if (provider.ExtendedData.ContainsKey(att.Key))
            {
                try
                {
                    return (IPaymentProviderSettings)JsonConvert.DeserializeObject(provider.ExtendedData.GetValue(att.Key), att.SettingsType);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(typeof(ProviderSettingsExtensions), "Failed to deserialize provider settings attempting to return default settings.", ex);
                }
            }

            var attempt = ProviderSettingsResolver.Current.ResolveByType(provider.GetType());

            if (attempt.Success) return attempt.Result;

            var nullReference = new NullReferenceException("Provider settings for provider was not mapped");
            MultiLogHelper.Error(
                typeof(ProviderSettingsExtensions),
                "Failed to create default provider settings",
                nullReference,
                logData);

            throw nullReference;
        }
    }
}