using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    internal class GatewayProviderResolver : MerchelloManyObjectsResolverBase<GatewayProviderResolver, GatewayProviderBase>,  IGatewayProviderResolver
    {        
        private readonly ConcurrentDictionary<Guid, GatewayProviderBase> _activatedGatewayProviderCache = new ConcurrentDictionary<Guid, GatewayProviderBase>();
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IRuntimeCacheProvider _runtimeCache;

        internal GatewayProviderResolver(IEnumerable<Type> values, IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache)
            : base(values)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");

            _gatewayProviderService = gatewayProviderService;
            _runtimeCache = runtimeCache;

            BuildActivatedGatewayProviderCache();
        }


        private void BuildActivatedGatewayProviderCache()
        {
            // this will cache the list of all providers that have been "Activated"
            foreach (var provider in _gatewayProviderService.GetAllGatewayProviders())
            {
                var gwProvider = CreateInstances(new object[] {_gatewayProviderService, provider, _runtimeCache}).FirstOrDefault();

                if(gwProvider != null) AddOrUpdateCache(gwProvider);
            }
        }

        private void AddOrUpdateCache(GatewayProviderBase provider)
        {
            _activatedGatewayProviderCache.AddOrUpdate(provider.Key, provider, (x, y) => provider);
        }

        protected override IEnumerable<GatewayProviderBase> Values
        {
            get
            {
                if (_activatedGatewayProviderCache.Count == InstanceTypes.Count())
                    return _activatedGatewayProviderCache.Values;

                var allResolved = new List<GatewayProviderBase>();

                var factory = new Persistence.Factories.GatewayProviderFactory();

                using (GetWriteLock())
                {
                    allResolved.AddRange(_activatedGatewayProviderCache.Values);

                    var inactive =
                        InstanceTypes.Where(
                            x =>
                                _activatedGatewayProviderCache.Values.All(
                                    y => x.GetCustomAttribute<GatewayProviderActivationAttribute>(false).Key != y.Key));

                    allResolved.AddRange(
                        inactive.Select(
                            type => factory.BuildEntity(type, GetGatewayProviderType(type))
                            ).Select(provider => 
                                CreateInstances(new object[] {_gatewayProviderService, provider, _runtimeCache}).FirstOrDefault()
                                ).Where(instance => instance != null));
                }

                return allResolved;
            }
        }

        /// <summary>
        /// Gets a collection of all activated PaymentGatewayProviders
        /// </summary>
        internal IEnumerable<PaymentGatewayProviderBase> PaymentGatewayProviders
        {
            get { return ActivatedProvidersOf<PaymentGatewayProviderBase>() as IEnumerable<PaymentGatewayProviderBase>; }
        }
             

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <param name="activatedOnly">Search only activated providers</param>
        /// <returns>An instantiated GatewayProvider</returns>
        public T GetProviderByKey<T>(Guid gatewayProviderKey, bool activatedOnly = true) where T : GatewayProviderBase
        {
            if (activatedOnly) 
            return ActivatedProvidersOf<T>().FirstOrDefault(x => x.Key == gatewayProviderKey) as T;

            return Values.FirstOrDefault(x => x.Key == gatewayProviderKey) as T;

        }

        /// <summary>
        /// Gets a collection of 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<GatewayProviderBase> AllProvidersOf<T>() where T : GatewayProviderBase
        {
            return Values.Where(instance => instance.GetType().IsAssignableFrom(typeof (T)));
        }


        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/>s by type
        /// </summary>
        public IEnumerable<GatewayProviderBase> ActivatedProvidersOf<T>() where T  : GatewayProviderBase
        {
            return _activatedGatewayProviderCache.Values.Where(x => x.GetType().IsAssignableFrom(typeof (T)));
        }

        public void RefreshCache()
        {
            _activatedGatewayProviderCache.Clear();
            BuildActivatedGatewayProviderCache();
        }


        /// <summary>
        /// Maps the type of T to a <see cref="GatewayProviderType"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns a <see cref="GatewayProviderType"/></returns>
        internal static GatewayProviderType GetGatewayProviderType(Type type)
        {
            if (typeof(PaymentGatewayProviderBase).IsAssignableFrom(type)) return GatewayProviderType.Payment;
            if (typeof(NotificationGatewayProviderBase).IsAssignableFrom(type)) return GatewayProviderType.Notification;
            if (typeof(ShippingGatewayProviderBase).IsAssignableFrom(type)) return GatewayProviderType.Shipping;
            if (typeof(TaxationGatewayProviderBase).IsAssignableFrom(type)) return GatewayProviderType.Taxation;

            throw new InvalidOperationException("Could not map GatewayProviderType from " + type.Name);
        }
    }
}