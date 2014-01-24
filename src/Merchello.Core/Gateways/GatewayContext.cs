using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    internal class GatewayContext : IGatewayContext
    {
        
        private readonly ConcurrentDictionary<Guid, IGatewayProvider> _gatewayProviderCache = new ConcurrentDictionary<Guid, IGatewayProvider>();
        private readonly IGatewayProviderFactory _gatewayProviderFactory;
        private readonly IGatewayProviderService _gatewayProviderService;

        public GatewayContext(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");
            
            _gatewayProviderFactory = new GatewayProviderFactory(gatewayProviderService, runtimeCache);
            _gatewayProviderService = gatewayProviderService;
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

        public IEnumerable<IShipmentRateQuote> GetShipRateQuotesForShipment(IShipment shipment)
        {
            var providers = ResolveByGatewayProviderType(GatewayProviderType.Shipping);
            var quotes = new List<IShipmentRateQuote>();
            foreach (var provider in providers)
            {
                quotes.AddRange(((ShippingGatewayProviderBase)provider).QuoteAvailableShipMethodsForShipment(shipment));
            }
            return quotes.OrderBy(x => x.Rate);
        }

        /// <summary>
        /// Gets a collection of instantiated gateway providers
        /// </summary>
        /// <param name="gatewayProviderType"></param>
        /// <returns></returns>
        public IEnumerable<GatewayProviderBase> ResolveByGatewayProviderType(GatewayProviderType gatewayProviderType)
        {
            var providers = GetGatewayProviders(gatewayProviderType);

            var gatewayProviders = new List<GatewayProviderBase>();
            foreach (var provider in providers)
            {
                if(gatewayProviderType == GatewayProviderType.Shipping)
                    gatewayProviders.Add(ResolveByGatewayProvider<ShippingGatewayProviderBase>(provider));
            }
            return gatewayProviders;
        }

        /// <summary>
        /// Returns an instantiation of a <see cref="T"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        public T ResolveByGatewayProvider<T>(IGatewayProvider provider) where T : GatewayProviderBase
        {

            if (typeof(ShippingGatewayProviderBase).IsAssignableFrom(typeof(T))) return _gatewayProviderFactory.GetInstance<ShippingGatewayProviderBase>(provider) as T;

            return null;
        }
        
        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public T ResolveByKey<T>(Guid gatewayProviderKey) where T : GatewayProviderBase
        {
            var provider = _gatewayProviderCache.FirstOrDefault(x => x.Key == gatewayProviderKey).Value;
            return provider == null ? null : ResolveByGatewayProvider<T>(provider);
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