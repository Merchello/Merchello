namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.ObjectResolution;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the OfferProviderResolver.
    /// </summary>
    internal class OfferProviderResolver : MerchelloManyObjectsResolverBase<OfferProviderResolver, OfferProviderBase>, IOfferProviderResolver
    {
        /// <summary>
        /// The lock.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The activated gateway provider cache.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, OfferProviderBase> _offerProviderCache = new ConcurrentDictionary<Guid, OfferProviderBase>();

        /// <summary>
        /// The <see cref="IOfferSettingsService"/>.
        /// </summary>
        private readonly IOfferSettingsService _offerSettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferProviderResolver"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="offerSettingsService">
        /// The offer settings service.
        /// </param>
        public OfferProviderResolver(IEnumerable<Type> value, IOfferSettingsService offerSettingsService)
            : base(value)
        {
            Mandate.ParameterNotNull(offerSettingsService, "offerSettingsService");
            _offerSettingsService = offerSettingsService;

            BuildOfferProviderCache();
        }

        /// <summary>
        /// Gets the collection of instantiated OfferProviders.
        /// </summary>
        protected override IEnumerable<OfferProviderBase> Values
        {
            get
            {
                return _offerProviderCache.Values;
            }
        }

        /// <summary>
        /// Gets a <see cref="IOfferProvider"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="OfferProviderBase"/>.
        /// </returns>
        public OfferProviderBase GetByKey(Guid key)
        {
            return _offerProviderCache.Values.FirstOrDefault(x => x.Key == key);
        }

        /// <summary>
        /// Same as Values
        /// </summary>
        /// <returns>
        /// <seealso cref="Values"/>.
        /// </returns>
        public IEnumerable<OfferProviderBase> GetOfferProviders()
        {
            return Values;
        }

        /// <summary>
        /// Gets an instantiated provider by it's type
        /// </summary>
        /// <typeparam name="T">
        /// The type of the provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T GetOfferProvider<T>() where T : OfferProviderBase
        {
            return (from value in _offerProviderCache.Values let t = value.GetType() where typeof(T).IsAssignableFrom(t) select value as T).FirstOrDefault();
        }

        /// <summary>
        /// Builds the provider cache.
        /// </summary>
        private void BuildOfferProviderCache()
        {
            foreach (var type in InstanceTypes)
            {                
                var attempt = ActivatorHelper.CreateInstance<OfferProviderBase>(type, new object[] { _offerSettingsService });
                if (attempt.Success)
                {
                    AddOrUpdateCache(attempt.Result);
                }
                else
                {
                    LogHelper.Error<OfferProviderResolver>("Failed to instantiate type: " + type.Name, attempt.Exception);
                }
            }
        }

        /// <summary>
        /// The add or update cache.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        private void AddOrUpdateCache(OfferProviderBase provider)
        {
            _offerProviderCache.AddOrUpdate(provider.Key, provider, (x, y) => provider);
        }

    }
}