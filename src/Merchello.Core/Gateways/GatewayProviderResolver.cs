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

namespace Merchello.Core.Gateways
{
    internal class GatewayProviderResolver : IGatewayProviderResolver
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IRuntimeCacheProvider _runtimeCache;
        private readonly ConcurrentDictionary<Guid, IGatewayProvider> _gatewayProviderCache = new ConcurrentDictionary<Guid, IGatewayProvider>();
        private readonly Lazy<GatewayProviderFactory> _gatewayProviderFactory;
        
        internal GatewayProviderResolver(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");

            _gatewayProviderService = gatewayProviderService;
            _runtimeCache = runtimeCache;

            _gatewayProviderFactory = new Lazy<GatewayProviderFactory>(() => new GatewayProviderFactory(_gatewayProviderService, _runtimeCache));

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

        ///// <summary>
        ///// Gets a collection of instantiated gateway providers
        ///// </summary>
        ///// <param name="gatewayProviderType"></param>
        ///// <returns></returns>
        //public IEnumerable<T> ResolveByGatewayProviderType<T>(GatewayProviderType gatewayProviderType)
        //    where T : GatewayProviderBase;
        //{
        //    var providers = GetGatewayProviders(gatewayProviderType);

        //    var gatewayProviders = new List<GatewayProviderBase>();
        //    foreach (var provider in providers)
        //    {
        //        switch (gatewayProviderType)
        //        {
        //            case GatewayProviderType.Shipping:
        //                gatewayProviders.Add(ResolveByGatewayProvider<ShippingGatewayProviderBase>(provider));
        //                break;
        //            case GatewayProviderType.Taxation:
        //                gatewayProviders.Add(ResolveByGatewayProvider<TaxationGatewayProviderBase>(provider));
        //                break;
        //            case GatewayProviderType.Payment:
        //                gatewayProviders.Add(ResolveByGatewayProvider<PaymentGatewayProviderBase>(provider));
        //                break;
        //        }

        //    }
        //    return gatewayProviders;
        //}


        /// <summary>
        /// Returns an instantiation of a <see cref="T"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        public T ResolveByGatewayProvider<T>(IGatewayProvider provider) where T : GatewayProviderBase
        {

            if (typeof(ShippingGatewayProviderBase).IsAssignableFrom(typeof(T)))
                return _gatewayProviderFactory.Value.GetInstance<ShippingGatewayProviderBase>(provider) as T;

            if (typeof(TaxationGatewayProviderBase).IsAssignableFrom(typeof(T)))
                return _gatewayProviderFactory.Value.GetInstance<TaxationGatewayProviderBase>(provider) as T;

            if (typeof(PaymentGatewayProviderBase).IsAssignableFrom(typeof(T)))
                return _gatewayProviderFactory.Value.GetInstance<PaymentGatewayProviderBase>(provider) as T;

            throw new InvalidOperationException("ResolveByGatewayProvider could not instantiant Type " + typeof(T).FullName);
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
        internal void RefreshCache()
        {
            BuildGatewayProviderCache();
        }

    }
}