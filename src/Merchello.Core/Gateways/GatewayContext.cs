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
        private readonly IGatewayProviderFactory _gatewayProviderFactory;

        public GatewayContext(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");
            
            _gatewayProviderFactory = new GatewayProviderFactory(gatewayProviderService, runtimeCache);

            BuildGatewayProviderCache(gatewayProviderService);

        }

        private void BuildGatewayProviderCache(IGatewayProviderService gatewayProviderService)
        {
            foreach (var provider in gatewayProviderService.GetAllGatewayProviders())
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
            return _gatewayProviderFactory.GetInstance<ShippingGatewayProvider>(provider);
        }

        /// <summary>
        /// Refreshes the <see cref="GatewayProviderBase"/> cache
        /// </summary>
        public void RefreshCache()
        {
            BuildGatewayProviderCache(((GatewayProviderFactory)_gatewayProviderFactory).GatewayProviderService);
        }

        
    }
}