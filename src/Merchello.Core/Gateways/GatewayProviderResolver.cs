using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

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
                if(attempt.Success) 
                    AddOrUpdateCache(attempt.Result);
                else
                {
                    LogHelper.Error<GatewayProviderResolver>(string.Format("Failed to create instance of type {0}", provider.Name), attempt.Exception);
                }

            }
        }

        private void AddOrUpdateCache(GatewayProviderBase provider)
        {
            _activatedGatewayProviderCache.AddOrUpdate(provider.Key, provider, (x, y) => provider);
        }

        private Attempt<GatewayProviderBase> CreateInstance(IGatewayProviderSettings providerSettings)
        {
            var providerType = InstanceTypes.FirstOrDefault(x => x.GetCustomAttribute<GatewayProviderActivationAttribute>(false).Key == providerSettings.Key);

            return providerSettings == null ? 
                Attempt<GatewayProviderBase>.Fail(new Exception(string.Format("Failed to find type for provider {0}", providerSettings.Name))) : 
                ActivatorHelper.CreateInstance<GatewayProviderBase>(providerType, new object[] { _gatewayProviderService, providerSettings, _runtimeCache });
        }

       


        protected override IEnumerable<GatewayProviderBase> Values
        {
            get
            {
                if (_activatedGatewayProviderCache.Count == InstanceTypes.Count())
                    return _activatedGatewayProviderCache.Values;

                var allResolved = new List<GatewayProviderBase>();

                var factory = new Persistence.Factories.GatewayProviderSettingsFactory();

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
        /// Gets a collection of all "activated" providers regardless of type
        /// </summary>
        public IEnumerable<GatewayProviderBase> GetActivatedProviders()
        {
            return _activatedGatewayProviderCache.Values;
        }

        /// <summary>
        /// Gets a collection of all providers regardless of type
        /// </summary>
        public IEnumerable<GatewayProviderBase> GetAllProviders()
        {
            return Values;
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
        /// Gets a collection of <see cref="IGatewayProviderSettings"/>s by type
        /// </summary>
        public IEnumerable<GatewayProviderBase> GetActivatedProviders<T>() where T  : GatewayProviderBase
        {
            return (from value in _activatedGatewayProviderCache.Values let t = value.GetType() where typeof(T).IsAssignableFrom(t) select value as T).ToList();
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