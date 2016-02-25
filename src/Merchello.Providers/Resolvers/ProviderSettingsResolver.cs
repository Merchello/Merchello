namespace Merchello.Providers.Resolvers
{
    using System;

    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The provider settings resolver.
    /// </summary>
    internal sealed class ProviderSettingsResolver
    {
        /// <summary>
        /// The current instance of the ProviderSettingsResolver.
        /// </summary>
        private static ProviderSettingsResolver _instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="ProviderSettingsResolver"/> class from being created.
        /// </summary>
        private ProviderSettingsResolver()
        {
        }

        /// <summary>
        /// Gets the current instance of the singleton.
        /// </summary>
        public static ProviderSettingsResolver Current
        {
            get
            {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_instance == null)
                {
                    _instance = new ProviderSettingsResolver();
                }

                return _instance;
            }   
        }

        /// <summary>
        /// Gets the default settings for a provider.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<IPaymentProviderSettings> ResolveByType(Type provider)
        {
            var attemptType = ResolveSettingsType(provider);
            if (!attemptType.Success) return Attempt<IPaymentProviderSettings>.Fail();  // could not resolve type

            // Get the provider settings type result
            return this.GetDefaultSettings(attemptType.Result);
        }

        /// <summary>
        /// Gets an instantiated default provider settings.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{IPaymentProviderSettings}"/>.
        /// </returns>
        public Attempt<IPaymentProviderSettings> GetDefaultSettings(Type settings)
        {
            try
            {
                // attempt to create an instance of the type
                var instance = Activator.CreateInstance(settings) as IPaymentProviderSettings;
                return instance != null
                    ? Attempt<IPaymentProviderSettings>.Succeed(instance)
                    : Attempt<IPaymentProviderSettings>.Fail();
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(ProviderSettingsExtensions), "Could not instantiate settings of type " + settings, ex);
                return Attempt<IPaymentProviderSettings>.Fail(ex);
            }
        }

        /// <summary>
        /// The resolve settings type.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        private static Attempt<Type> ResolveSettingsType(Type provider)
        {
            var att = provider.GetCustomAttribute<ProviderSettingsMapperAttribute>(false);

            return att != null ? 
                Attempt<Type>.Succeed(att.SettingsType) : 
                Attempt<Type>.Fail();
        }
    }
}