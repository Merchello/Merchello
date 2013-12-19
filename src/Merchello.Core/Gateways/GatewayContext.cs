using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    internal class GatewayContext : IGatewayContext
    {
        
        private readonly ConcurrentDictionary<Guid, IGatewayProvider> _gatewayProviderCache = new ConcurrentDictionary<Guid, IGatewayProvider>();

        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IRuntimeCacheProvider _runtimeCache;

        public GatewayContext(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");
            
            _gatewayProviderService = gatewayProviderService;
            _runtimeCache = runtimeCache;

            BuildGatewayProviderCache();

        }

        private void BuildGatewayProviderCache()
        {
            foreach (var provider in _gatewayProviderService.GetAllGatewayProviders())
            {
                _gatewayProviderCache.AddOrUpdate(provider.Key, provider, (x, y) => provider);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/>s by type
        /// </summary>
        public IEnumerable<IGatewayProvider> GetGatewayProviders(GatewayProviderType gatewayProviderType)
        {
            var providers =
                _gatewayProviderCache.Where(provider => provider.Value.GatewayProviderType == gatewayProviderType)
                    .Select(provider => provider.Value)
                    .ToList();
            return providers;
        }


        /// <summary>
        /// Returns an instantiation of a <see cref="ShippingGatewayProvider"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        public ShippingGatewayProvider GetShippingGatewayProvider(IGatewayProvider provider)
        {
            if(GatewayProviderType.Shipping != provider.GatewayProviderType) throw new InvalidOperationException("This provider cannot be instantiated as a Shipping Provider");
            return GetInstance<ShippingGatewayProvider>(provider);
        }


        private T GetInstance<T>(IGatewayProvider provider) where T : GatewayBase
        {
            return ActivateGateway(provider) as T;
        }


        /// <summary>
        /// Refreshes the <see cref="GatewayBase"/> cache
        /// </summary>
        public void RefreshCache()
        {
            BuildGatewayProviderCache();
        }

        /// <summary>
        /// Creates an instance of a <see cref="GatewayBase"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <returns></returns>
        private GatewayBase ActivateGateway(IGatewayProvider gatewayProvider)
        {
            var ctorArgs = new[] { typeof(IGatewayProviderService), typeof(IGatewayProvider), typeof(IRuntimeCacheProvider) };
            var ctoArgValues = new object[] { _gatewayProviderService, gatewayProvider, _runtimeCache };
            return ActivatorHelper.CreateInstance<GatewayBase>(Type.GetType(gatewayProvider.TypeFullName), ctorArgs, ctoArgValues);
        }
    }
}