using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;
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
            // this will cache the list of all providers that have been "Activated"
            foreach (var provider in _gatewayProviderService.GetAllGatewayProviders())
            {
                _gatewayProviderCache.AddOrUpdate(provider.Key, provider, (x, y) => provider);
            }
        }


        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/>s by type
        /// </summary>
        /// TODO this could be refactored to not have to instantiate the object each time (ObjectLifeTimeScope.Application)
        public IEnumerable<IGatewayProvider> GetActivatedProviders<T>() where T : GatewayProviderBase
        {
            var gatewayProviderType = GetGatewayProviderType<T>();

            var providers =
                _gatewayProviderCache.Where(provider => provider.Value.GatewayProviderType == gatewayProviderType)
                    .Select(provider => provider.Value)
                    .ToList();
            return providers;
        }

        /// <summary>
        /// Gets a collection of inactive (not saved) <see cref="IGatewayProvider"/> by type
        /// </summary>
        public IEnumerable<IGatewayProvider> GetAllProviders<T>() where T : GatewayProviderBase
        {
            var gatewayProviderType = GetGatewayProviderType<T>();
            
            switch (gatewayProviderType)
            {
                case GatewayProviderType.Payment:
                    return BuildGatewayProvidersFromResolved<T>(PaymentGatewayProviderResolver.Current.ProviderTypes, gatewayProviderType);
                case GatewayProviderType.Notification:
                    return BuildGatewayProvidersFromResolved<T>(NotificationGatewayProviderResolver.Current.ProviderTypes, gatewayProviderType);
                case GatewayProviderType.Shipping:
                    return BuildGatewayProvidersFromResolved<T>(ShippingGatewayProviderResolver.Current.ProviderTypes, gatewayProviderType);
                case GatewayProviderType.Taxation:
                    return BuildGatewayProvidersFromResolved<T>(TaxationGatewayProviderResolver.Current.ProviderTypes, gatewayProviderType);                        
            }

            throw new InvalidOperationException("GetAllProviders could resolve a Type " + typeof(T));
        }

        private IEnumerable<IGatewayProvider> BuildGatewayProvidersFromResolved<T>(IEnumerable<Type> types, GatewayProviderType gatewayProviderType) where T : GatewayProviderBase
        {
            var existing = GetActivatedProviders<T>().ToArray();
            
            
            var providers = new List<IGatewayProvider>();

            var factory = new Persistence.Factories.GatewayProviderFactory();
            
            foreach (var t in types)
            {
                var att = t.GetCustomAttributes<GatewayProviderActivationAttribute>(false).FirstOrDefault();
                if (att != null)
                {
                    providers.Add(
                        existing.Any(x => x.Key == att.Key)
                            ? existing.First(x => x.Key == att.Key)
                            : factory.BuildEntity(t, gatewayProviderType)
                        );
                }
            }

            return providers;
        }

        
        /// <summary>
        /// Gets a collection of instantiated gateway providers
        /// </summary>
        /// <param name="gatewayProviderType"></param>
        /// <returns></returns>
        public IEnumerable<T> CreateInstances<T>(GatewayProviderType gatewayProviderType) where T : GatewayProviderBase
        {
            return GetActivatedProviders<T>().Select(CreateInstance<T>);

        }

        /// <summary>
        /// Returns an instantiation of a <see cref="T"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        public T CreateInstance<T>(IGatewayProvider provider) where T : GatewayProviderBase
        {
            switch (GetGatewayProviderType<T>())
            {
                case GatewayProviderType.Payment:
                return _gatewayProviderFactory.Value.GetInstance<PaymentGatewayProviderBase>(provider) as T;

                case GatewayProviderType.Notification:
                return _gatewayProviderFactory.Value.GetInstance<NotificationGatewayProviderBase>(provider) as T;

                case GatewayProviderType.Shipping:
                return _gatewayProviderFactory.Value.GetInstance<ShippingGatewayProviderBase>(provider) as T;

                case GatewayProviderType.Taxation:
                return _gatewayProviderFactory.Value.GetInstance<TaxationGatewayProviderBase>(provider) as T;
            }

            throw new InvalidOperationException("CreateInstance could not instantiant Type " + typeof(T).FullName);
        }

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public T CreateInstance<T>(Guid gatewayProviderKey) where T : GatewayProviderBase
        {
            var provider = _gatewayProviderCache.FirstOrDefault(x => x.Key == gatewayProviderKey).Value;
            return provider == null ? null : CreateInstance<T>(provider);
        }

        /// <summary>
        /// Refreshes the <see cref="GatewayProviderBase"/> cache
        /// </summary>
        public void RefreshCache()
        {
            _gatewayProviderCache.Clear();
            BuildGatewayProviderCache();
        }

        /// <summary>
        /// Maps the type of T to a <see cref="GatewayProviderType"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns a <see cref="GatewayProviderType"/></returns>
        internal static GatewayProviderType GetGatewayProviderType<T>()
        {
            if (typeof(PaymentGatewayProviderBase).IsAssignableFrom(typeof(T))) return GatewayProviderType.Payment;
            if (typeof(NotificationGatewayProviderBase).IsAssignableFrom(typeof(T))) return GatewayProviderType.Notification;
            if (typeof(ShippingGatewayProviderBase).IsAssignableFrom(typeof(T))) return GatewayProviderType.Shipping;
            if (typeof(TaxationGatewayProviderBase).IsAssignableFrom(typeof(T))) return GatewayProviderType.Taxation;
            
            throw new InvalidOperationException("Could not map GatewayProviderType from " + typeof(T).Name);
        }

    }
}