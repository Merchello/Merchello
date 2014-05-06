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
                var attempt = CreateInstance(provider);
                if(attempt.Success) AddOrUpdateCache(attempt.Result);
            }
        }

        private void AddOrUpdateCache(GatewayProviderBase provider)
        {
            _activatedGatewayProviderCache.AddOrUpdate(provider.Key, provider, (x, y) => provider);
        }

        private Attempt<GatewayProviderBase> CreateInstance(IGatewayProvider provider)
        {
            return ActivatorHelper.CreateInstance<GatewayProviderBase>(provider.TypeFullName, new object[] { _gatewayProviderService, provider, _runtimeCache });
        }

        ///// <summary>
        ///// Asserts the assembly versions get updated (if applicable) when the context is instantiated.
        ///// </summary>
        ///// TODO revist this.  Probably better to do something like this in the bootstrapper
        //private void AssertProviderVersions()
        //{
        //    var all = GetAllProviders().ToArray();
        //    var activated = GetAllActivatedProviders();

        //    foreach (var provider in activated)
        //    {
        //        var key = provider.Key;
        //        var resolved = all.FirstOrDefault(x => x.Key == key);

        //        if (resolved == null) continue;
        //        if (provider.GatewayProvider.TypeFullName.Equals(resolved.GatewayProvider.TypeFullName)) continue;

        //        provider.GatewayProvider.TypeFullName = resolved.GatewayProvider.TypeFullName;
        //        GatewayProviderService.Save(provider.GatewayProvider);
        //    }
        //}


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

                    var inactive = (from it in InstanceTypes let key = it.GetCustomAttribute<GatewayProviderActivationAttribute>(false).Key 
                                    where !_activatedGatewayProviderCache.ContainsKey(key) select it).ToList();

                    allResolved.AddRange(
                        inactive.Select(
                            type => factory.BuildEntity(type, GetGatewayProviderType(type))
                            ).Select(CreateInstance).Where(attempt => attempt.Success).Select(x => x.Result));
                }

                return allResolved;
            }
        }

        /// <summary>
        /// Gets a collection of all activated PaymentGatewayProviders
        /// </summary>
        internal IEnumerable<PaymentGatewayProviderBase> PaymentGatewayProviders
        {
            get { return GetActivatedProviders<PaymentGatewayProviderBase>() as IEnumerable<PaymentGatewayProviderBase>; }
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
            return GetActivatedProviders<T>().FirstOrDefault(x => x.Key == gatewayProviderKey) as T;

            return Values.FirstOrDefault(x => x.Key == gatewayProviderKey) as T;

        }

        /// <summary>
        /// Gets a collection of 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<GatewayProviderBase> GetAllProviders<T>() where T : GatewayProviderBase
        {
            return (from value in Values let t = value.GetType() where typeof (T).IsAssignableFrom(t) select value as T).ToList();
        }


        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/>s by type
        /// </summary>
        public IEnumerable<GatewayProviderBase> GetActivatedProviders<T>() where T  : GatewayProviderBase
        {
            var type = typeof (T);
            return
                _activatedGatewayProviderCache.Values.Where(
                    x => x.GatewayProvider.GatewayProviderType == GetGatewayProviderType(type));
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