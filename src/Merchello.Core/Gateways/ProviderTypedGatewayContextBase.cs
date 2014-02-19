using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Base class for GatewayContext objects
    /// </summary>
    internal abstract class ProviderTypedGatewayContextBase<T> : IProviderTypedGatewayContextBase<T>
        where T : GatewayProviderBase
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IRuntimeCacheProvider _runtimeCache;

        protected ProviderTypedGatewayContextBase(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");

            _gatewayProviderService = gatewayProviderService;
            _runtimeCache = runtimeCache;
            BuildGatewayContext(gatewayProviderService, runtimeCache);           
        }

        private void BuildGatewayContext(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache)
        {
            _resolver = new GatewayProviderResolver(gatewayProviderService, runtimeCache);
        }

        /// <summary>
        /// Resolves all active <see cref="IGatewayProvider"/>s of T
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<T> ResolveAllActiveProviders();
    
        /// <summary>
        /// Resolves a <see cref="IGatewayProvider"/> by it's unique key
        /// </summary>
        /// <param name="key">The Guid 'key' of the provider</param>
        /// <returns>Returns a <see cref="IGatewayProvider"/> of type T</returns>
        public abstract T ResolveByKey(Guid key);

        private IGatewayProviderResolver _resolver;
        protected IGatewayProviderResolver GatewayProviderResolver
        {
            get{
            if(_resolver == null) throw new InvalidOperationException("GatewayProviderResolver has not been instantiated.");
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
        /// Gets the Runtime Cache
        /// </summary>
        protected IRuntimeCacheProvider RuntimeCache
        {
            get
            {
                return _runtimeCache;
            }
        }
    }
}