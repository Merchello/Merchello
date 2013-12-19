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
        
        private readonly ConcurrentDictionary<IGatewayProvider, Lazy<GatewayBase>> _gatewayProviderCache = new ConcurrentDictionary<IGatewayProvider, Lazy<GatewayBase>>();

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
                var lazy = new Lazy<GatewayBase>(() => ActivateGateway(provider));
                _gatewayProviderCache.AddOrUpdate(provider, lazy, (x, y) => lazy);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/>s by type
        /// </summary>
        public IEnumerable<IGatewayProvider> GetGatewayProviders(GatewayProviderType gatewayProviderType)
        {
            var providers = _gatewayProviderCache.Keys.Where(provider => provider.GatewayProviderType == gatewayProviderType).ToList();
            return providers;
        }


        /// <summary>
        /// Returns an instantiation of a <see cref="ShippingGatewayBase"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        public ShippingGatewayBase InstantiateShippingGateway(IGatewayProvider provider)
        {
            if(GatewayProviderType.Shipping != provider.GatewayProviderType) throw new InvalidOperationException("This provider cannot be instantiated as a Shipping Provider");
            return GetInstance<ShippingGatewayBase>(provider);
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

        private GatewayBase ActivateGateway(IGatewayProvider gatewayProvider)
        {
            var ctorArgs = new[] { typeof(IGatewayProviderService), typeof(IGatewayProvider), typeof(IRuntimeCacheProvider) };
            var ctoArgValues = new object[] { _gatewayProviderService, gatewayProvider, _runtimeCache };
            return ActivatorHelper.CreateInstance<GatewayBase>(Type.GetType(gatewayProvider.TypeFullName), ctorArgs, ctoArgValues);
        }
    }
}