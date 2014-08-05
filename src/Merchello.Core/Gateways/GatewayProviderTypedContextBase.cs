namespace Merchello.Core.Gateways
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Services;

    /// <summary>
    /// Base class for GatewayContext objects
    /// </summary>
    /// <typeparam name="T">
    /// The type of the gateway provider
    /// </typeparam>
    public abstract class GatewayProviderTypedContextBase<T> : IGatewayProviderTypedContextBase<T>
        where T : GatewayProviderBase
    {
        /// <summary>
        /// The gateway provider service.
        /// </summary>
        private readonly IGatewayProviderService _gatewayProviderService;

        /// <summary>
        /// The resolver.
        /// </summary>
        private readonly IGatewayProviderResolver _resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayProviderTypedContextBase{T}"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        protected GatewayProviderTypedContextBase(IGatewayProviderService gatewayProviderService, IGatewayProviderResolver resolver)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");            
            Mandate.ParameterNotNull(resolver, "resolver");

            _gatewayProviderService = gatewayProviderService;            
            _resolver = resolver;
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderResolver"/>
        /// </summary>
        protected IGatewayProviderResolver GatewayProviderResolver
        {
            get
            {
                if (_resolver == null) throw new InvalidOperationException("GatewayProviderResolver has not been instantiated.");
                return _resolver;
            }
        }

        /// <summary>
        /// Gets the GatewayProviderService
        /// </summary>
        protected IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }

        /// <summary>
        /// Lists all actived <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <returns>A collection of all "activated" GatewayProvider of the particular type T</returns>
        public IEnumerable<GatewayProviderBase> GetAllActivatedProviders()
        {
            return GatewayProviderResolver.GetActivatedProviders<T>();
        }

        /// <summary>
        /// Lists all available providers.  This list includes providers that are just resolved and not configured
        /// </summary>
        /// <returns>A collection of all Gatewayprovider</returns>
        public IEnumerable<GatewayProviderBase> GetAllProviders()
        {
            return GatewayProviderResolver.GetAllProviders<T>();
        }

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <param name="gatewayProviderKey">
        /// The gateway provider key
        /// </param>
        /// <param name="activatedOnly">
        /// Search only activated providers
        /// </param>
        /// <returns>
        /// An instantiated GatewayProvider
        /// </returns>
        public T GetProviderByKey(Guid gatewayProviderKey, bool activatedOnly = true)
        {
            return GatewayProviderResolver.GetProviderByKey<T>(gatewayProviderKey, activatedOnly);
        }

        /// <summary>
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (Guid) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (Guid) of the <see cref="IGatewayMethod"/></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public abstract T GetProviderByMethodKey(Guid gatewayMethodKey);

        /// <summary>
        /// Creates an instance GatewayProvider given its registered Key
        /// </summary>
        /// <param name="gatewayProviderKey">
        /// The gateway Provider Key.
        /// </param>
        /// <returns>
        /// An instance of the gateway provider.
        /// </returns>
        [Obsolete("Use GetProviderByKey instead")]
        public T CreateInstance(Guid gatewayProviderKey)
        {
            return GetProviderByKey(gatewayProviderKey);
        }

        /// <summary>
        /// Activates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="provider">The GatewayProvider to be activated</param>
        public void ActivateProvider(GatewayProviderBase provider)
        {
            ActivateProvider(provider.GatewayProviderSettings);
        }

        /// <summary>
        /// Activates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderSettings">The <see cref="IGatewayProviderSettings"/> to be activated</param>
        public void ActivateProvider(IGatewayProviderSettings gatewayProviderSettings)
        {
            if (gatewayProviderSettings.Activated) return;
            GatewayProviderService.Save(gatewayProviderSettings);
            GatewayProviderResolver.RefreshCache();
        }

        /// <summary>
        /// Deactivates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="provider">The GatewayProvider to be deactivated</param>
        public void DeactivateProvider(GatewayProviderBase provider)
        {
            DeactivateProvider(provider.GatewayProviderSettings);
        }

        /// <summary>
        /// Deactivates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderSettings">The <see cref="IGatewayProviderSettings"/> to be deactivated</param>
        public void DeactivateProvider(IGatewayProviderSettings gatewayProviderSettings)
        {
            if (!gatewayProviderSettings.Activated) return;
            GatewayProviderService.Delete(gatewayProviderSettings);
            GatewayProviderResolver.RefreshCache();
        }
    }
}