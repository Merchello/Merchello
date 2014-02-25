using System;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines the GatewayBase
    /// </summary>
    public abstract class GatewayProviderBase : IProvider
    {        
        private readonly IGatewayProvider _gatewayProvider;
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IRuntimeCacheProvider _runtimeCache;

        protected GatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(gatewayProvider, "gatewayProvider");
            Mandate.ParameterNotNull(runtimeCacheProvider, "runtimeCacheProvider");

            _gatewayProviderService = gatewayProviderService;
            _gatewayProvider = gatewayProvider;
            _runtimeCache = runtimeCacheProvider;
        }


        // The properties Name and Key will be likely become attribute defined properties
        // TODO enable devs to define each of these values. 
        
        /// <summary>
        /// The name of the GatewayProvider
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The unique Key that will be used
        /// </summary>
        public abstract Guid Key { get;  }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        protected IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProvider"/>
        /// </summary>
        public virtual IGatewayProvider GatewayProvider 
        {
            get { return _gatewayProvider; }
        }

        /// <summary>
        /// Gets the RuntimeCache
        /// </summary>
        /// <returns></returns>
        protected IRuntimeCacheProvider RuntimeCache
        {
            get { return _runtimeCache; }
        }
    }
}