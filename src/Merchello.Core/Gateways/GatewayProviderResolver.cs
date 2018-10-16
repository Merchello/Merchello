namespace Merchello.Core.Gateways
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Gateways.Notification;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.ObjectResolution;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Cache;

    /// <summary>
    /// The gateway provider resolver.
    /// </summary>
    internal class GatewayProviderResolver : MerchelloManyObjectsResolverBase<GatewayProviderResolver, GatewayProviderBase>, IGatewayProviderResolver
    {
        /// <summary>
        /// The lock.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Stores a list of <see cref="GatewayProviderSettings"/> in the singleton for quick reference
        /// </summary>
        private readonly ConcurrentDictionary<Guid, IGatewayProviderSettings> _activatedProviderSettingsCache = new ConcurrentDictionary<Guid, IGatewayProviderSettings>();

        /// <summary>
        /// The gateway provider service.
        /// </summary>
        private readonly IGatewayProviderService _gatewayProviderService;

        /// <summary>
        /// The runtime cache.
        /// </summary>
        private readonly IRuntimeCacheProvider _runtimeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayProviderResolver"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="runtimeCache">
        /// The runtime cache.
        /// </param>
        internal GatewayProviderResolver(
            IEnumerable<Type> values,
            IGatewayProviderService gatewayProviderService,
            IRuntimeCacheProvider runtimeCache)
            : base(values)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");

            _gatewayProviderService = gatewayProviderService;
            _runtimeCache = runtimeCache;

            BuildActivatedProviderSettingsCache();
        }

        protected override IEnumerable<GatewayProviderBase> Values
        {
            get
            {
                // TODO - Must be a better way to do this. Bit of a hack
                var activatedGateways = GetActivatedProviders().ToArray();

                var activatedGatewayKeys = activatedGateways.Select(x => x.Key).ToArray();

                var factory = new Persistence.Factories.GatewayProviderSettingsFactory();

                var allGateWays = InstanceTypes.Select(type => factory.BuildEntity(type, GetGatewayProviderType(type)))
                    .Select(CreateInstance).Where(attempt => attempt.Success)
                    .Select(x => x.Result)
                    .Where(x => !activatedGatewayKeys.Contains(x.Key))
                    .ToList();

                allGateWays.AddRange(activatedGateways);

                return allGateWays;
            }
        }

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey">The Gateway Provider Key</param>
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
        /// <returns>
        /// The collection of GatewayProviderBase.
        /// </returns>
        public IEnumerable<GatewayProviderBase> GetActivatedProviders()
        {
            return _activatedProviderSettingsCache.Values
                .Select(CreateInstance)
                .Where(attempt => attempt.Success)
                .Select(x => x.Result).ToList();
        }

        /// <summary>
        /// Gets a collection of all providers regardless of type
        /// </summary>
        /// <returns>
        /// The collection of gateway providers.
        /// </returns>
        public IEnumerable<GatewayProviderBase> GetAllProviders()
        {
            return Values;
        }

        /// <summary>
        /// Gets a collection of 
        /// </summary>
        /// <typeparam name="T">The type of GatewayProvider</typeparam>
        /// <returns>The collection of gateway providers</returns>
        public IEnumerable<GatewayProviderBase> GetAllProviders<T>() where T : GatewayProviderBase
        {
            return (from value in Values let t = value.GetType() where typeof(T).IsAssignableFrom(t) select value as T).ToList();
        }


        /// <summary>
        /// Gets a collection of <see cref="IGatewayProviderSettings"/>s by type
        /// </summary>
        /// <typeparam name="T">
        /// The type of gateway provider
        /// </typeparam>
        /// <returns>
        /// The collection of gateway providers.
        /// </returns>
        public IEnumerable<GatewayProviderBase> GetActivatedProviders<T>() where T : GatewayProviderBase
        {
            return (
                from value in this.GetActivatedProviders()
                let t = value.GetType()
                where typeof(T).IsAssignableFrom(t)
                select value as T).ToList();
        }

        /// <summary>
        /// Refreshes the provider cache.
        /// </summary>
        public void RefreshCache()
        {
            _activatedProviderSettingsCache.Clear();
            BuildActivatedProviderSettingsCache();
        }

        /// <summary>
        /// Maps the type of T to a <see cref="GatewayProviderType"/>
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// Returns a <see cref="GatewayProviderType"/>
        /// </returns>
        internal static GatewayProviderType GetGatewayProviderType(Type type)
        {
            if (typeof(PaymentGatewayProviderBase).IsAssignableFrom(type)) return GatewayProviderType.Payment;
            if (typeof(NotificationGatewayProviderBase).IsAssignableFrom(type)) return GatewayProviderType.Notification;
            if (typeof(ShippingGatewayProviderBase).IsAssignableFrom(type)) return GatewayProviderType.Shipping;
            if (typeof(TaxationGatewayProviderBase).IsAssignableFrom(type)) return GatewayProviderType.Taxation;

            throw new InvalidOperationException("Could not map GatewayProviderType from " + type.Name);
        }

        /// <summary>
        /// Returns a WriteLock to use when modifying collections
        /// </summary>
        /// <returns>Gets the write lock</returns>
        protected override WriteLock GetWriteLock()
        {
            return new WriteLock(_lock);
        }


        private void BuildActivatedProviderSettingsCache()
        {
            var settings = _gatewayProviderService.GetAllGatewayProviders().ToList();
            foreach (var setting in settings)
            {
                _activatedProviderSettingsCache.AddOrUpdate(setting.Key, setting, (x, y) => setting);
            }
        }


        /// <summary>
        /// The create instance.
        /// </summary>
        /// <param name="providerSettings">
        /// The provider settings.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        private Attempt<GatewayProviderBase> CreateInstance(IGatewayProviderSettings providerSettings)
        {
            var providerType = InstanceTypes.FirstOrDefault(x => x.GetCustomAttribute<GatewayProviderActivationAttribute>(false).Key == providerSettings.Key);

            return providerSettings == null ?
                Attempt<GatewayProviderBase>.Fail(new Exception(string.Format("Failed to find type for provider {0}", providerSettings.Name))) :
                ActivatorHelper.CreateInstance<GatewayProviderBase>(providerType, new object[] { _gatewayProviderService, providerSettings, _runtimeCache });
        }
    }
}