namespace Merchello.Providers.Payment.Models
{
    using System;
    using System.Reflection;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Resolvers;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Umbraco.Core.Logging;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// The provider settings extensions.
    /// </summary>
    public static class ProviderSettingsExtensions
    {

        /// <summary>
        /// Get the PayPal provider settings from the extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The deserialized <see cref="PayPalProviderSettings"/></returns>
        public static PayPalProviderSettings GetPayPalProviderSettings(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsKey(Constants.PayPal.ExtendedDataKeys.ProviderSettings)) return new PayPalProviderSettings();

            return
                JsonConvert.DeserializeObject<PayPalProviderSettings>(
                    extendedData.GetValue(Constants.PayPal.ExtendedDataKeys.ProviderSettings));
        }

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

        /// <summary>
        /// Gets the <see cref="ProviderSettingsMapperAttribute"/>.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="ProviderSettingsMapperAttribute"/>.
        /// </returns>
        internal static ProviderSettingsMapperAttribute ProviderSettingsMapping(this PaymentGatewayProviderBase provider)
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
        internal static T GetProviderSettings<T>(this PaymentGatewayProviderBase provider)
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
        internal static void SaveProviderSettings(this IGatewayProviderSettings record, PaymentGatewayProviderBase provider, IPaymentProviderSettings settings)
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
        private static IPaymentProviderSettings GetProviderSettings(this PaymentGatewayProviderBase provider)
        {
            var att = provider.ProviderSettingsMapping();
            if (att == null)
            {
                var invalidOp = new InvalidOperationException("Cannot use this method if the provider is not decorated with a ProviderSettingsMapperAttribute");
                LogHelper.Error(typeof(ProviderSettingsExtensions), "PaymentGatewayProvider does not have attribute", invalidOp);
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
            LogHelper.Error(
                typeof(ProviderSettingsExtensions),
                "Failed to create default provider settings",
                nullReference);

            throw nullReference;
        }
    }
}