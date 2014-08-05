namespace Merchello.Core.Gateways
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Services;
    using Umbraco.Core.Cache;

    /// <summary>
    /// Defines the GatewayBase
    /// </summary>
    public abstract class GatewayProviderBase : IProvider
    {
        #region Fields

        /// <summary>
        /// The gateway provider settings.
        /// </summary>
        private readonly IGatewayProviderSettings _gatewayProviderSettings;

        /// <summary>
        /// The gateway provider service.
        /// </summary>
        private readonly IGatewayProviderService _gatewayProviderService;

        /// <summary>
        /// The runtime cache.
        /// </summary>
        private readonly IRuntimeCacheProvider _runtimeCache;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayProviderBase"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">The <see cref="IGatewayProviderService"/></param>
        /// <param name="gatewayProviderSettings">The <see cref="IGatewayProviderSettings"/></param>
        /// <param name="runtimeCacheProvider">Umbraco's <see cref="IRuntimeCacheProvider"/></param>
        protected GatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(gatewayProviderSettings, "gatewayProvider");
            Mandate.ParameterNotNull(runtimeCacheProvider, "runtimeCacheProvider");

            _gatewayProviderService = gatewayProviderService;
            _gatewayProviderSettings = gatewayProviderSettings;
            _runtimeCache = runtimeCacheProvider;
        }

        /// <summary>
        /// Gets the unique Key that will be used
        /// </summary>
        public Guid Key 
        {
            get { return _gatewayProviderSettings.Key; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        public IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderSettings"/>
        /// </summary>
        public virtual IGatewayProviderSettings GatewayProviderSettings 
        {
            get { return _gatewayProviderSettings; }
        }

        /// <summary>
        /// Gets the ExtendedData collection from the <see cref="IGatewayProviderSettings"/>
        /// </summary>
        public virtual ExtendedDataCollection ExtendedData
        {
            get { return _gatewayProviderSettings.ExtendedData; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this provider is "activated"
        /// </summary>
        public virtual bool Activated
        {
            get { return _gatewayProviderSettings.Activated; }
        }

        /// <summary>
        /// Gets the RuntimeCache
        /// </summary>
        /// <returns></returns>
        protected IRuntimeCacheProvider RuntimeCache
        {
            get { return _runtimeCache; }
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="IGatewayResource"/></returns>
        public abstract IEnumerable<IGatewayResource> ListResourcesOffered();
    }
}