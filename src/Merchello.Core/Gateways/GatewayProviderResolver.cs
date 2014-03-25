using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways
{
    internal class GatewayProviderResolver : IGatewayProviderResolver
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly ConcurrentDictionary<Guid, IGatewayProvider> _gatewayProviderCache = new ConcurrentDictionary<Guid, IGatewayProvider>();
        private readonly Lazy<GatewayProviderFactory> _gatewayProviderFactory;
        
        internal GatewayProviderResolver(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");

            _gatewayProviderService = gatewayProviderService;

            _gatewayProviderFactory = new Lazy<GatewayProviderFactory>(() => new GatewayProviderFactory(_gatewayProviderService, runtimeCache));

            BuildGatewayProviderCache();
        }


        private void BuildGatewayProviderCache()
        {            
            // this will cache the list of all providers that have been "Activated"
            foreach (var provider in _gatewayProviderService.GetAllGatewayProviders())
            {
                _gatewayProviderCache.AddOrUpdate(provider.Key, provider, (x, y) => provider);
            }
        }


        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/>s by type
        /// </summary>
        public IEnumerable<IGatewayProvider> GetGatewayProviders<T>() where T : GatewayProviderBase
        {
            var gatewayProviderType = GetGatewayProviderType<T>();

            var providers =
                _gatewayProviderCache.Where(provider => provider.Value.GatewayProviderType == gatewayProviderType)
                    .Select(provider => provider.Value)
                    .ToList();
            return providers;
        }


        /// <summary>
        /// Gets a collection of instantiated gateway providers
        /// </summary>
        /// <param name="gatewayProviderType"></param>
        /// <returns></returns>
        public IEnumerable<T> CreateInstanceByGatewayProviderType<T>(GatewayProviderType gatewayProviderType) where T : GatewayProviderBase
        {
            return GetGatewayProviders<T>().Select(CreateInstanceByGatewayProvider<T>);

        }

        /// <summary>
        /// Returns an instantiation of a <see cref="T"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        public T CreateInstanceByGatewayProvider<T>(IGatewayProvider provider) where T : GatewayProviderBase
        {
            switch (GetGatewayProviderType<T>())
            {
                    case GatewayProviderType.Shipping:
                    return _gatewayProviderFactory.Value.GetInstance<ShippingGatewayProviderBase>(provider) as T;

                    case GatewayProviderType.Taxation:
                    return _gatewayProviderFactory.Value.GetInstance<TaxationGatewayProviderBase>(provider) as T;

                    case GatewayProviderType.Payment:
                    return _gatewayProviderFactory.Value.GetInstance<PaymentGatewayProviderBase>(provider) as T;
            }

            throw new InvalidOperationException("ResolveByGatewayProvider could not instantiant Type " + typeof(T).FullName);
        }

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public T CreateInstanceByKey<T>(Guid gatewayProviderKey) where T : GatewayProviderBase
        {
            var provider = _gatewayProviderCache.FirstOrDefault(x => x.Key == gatewayProviderKey).Value;
            return provider == null ? null : CreateInstanceByGatewayProvider<T>(provider);
        }

        /// <summary>
        /// Refreshes the <see cref="GatewayProviderBase"/> cache
        /// </summary>
        internal void RefreshCache()
        {
            BuildGatewayProviderCache();
        }

        /// <summary>
        /// Maps the type of T to a <see cref="GatewayProviderType"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns a <see cref="GatewayProviderType"/></returns>
        internal static GatewayProviderType GetGatewayProviderType<T>()
        {
            if (typeof(ShippingGatewayProviderBase).IsAssignableFrom(typeof(T))) return GatewayProviderType.Shipping;
            if (typeof(TaxationGatewayProviderBase).IsAssignableFrom(typeof(T))) return GatewayProviderType.Taxation;
            if (typeof(PaymentGatewayProviderBase).IsAssignableFrom(typeof(T))) return GatewayProviderType.Payment;
            throw new InvalidOperationException("Could not map GatewayProviderType from " + typeof(T).Name);
        }

    }
}