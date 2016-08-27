namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.ObjectResolution;

    /// <summary>
    /// The entity collection provider manager.
    /// </summary>
    internal class EntityCollectionProviderResolver : ResolverBase<EntityCollectionProviderResolver>, IEntityCollectionProviderResolver
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The activated gateway provider cache.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, Type> _entityCollectionProviderCache = new ConcurrentDictionary<Guid, Type>();

        /// <summary>
        /// The instance types.
        /// </summary>
        private readonly List<Type> _instanceTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProviderResolver"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        internal EntityCollectionProviderResolver(IEnumerable<Type> values, IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _merchelloContext = merchelloContext;
            _instanceTypes = values.ToList();          
        }

        /// <summary>
        /// Gets or sets a value indicating whether is initialized.
        /// </summary>
        private static bool IsInitialized { get; set; }

        /// <summary>
        /// Gets the provider key from the attribute
        /// </summary>
        /// <typeparam name="T">
        /// The type of the provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetProviderKey<T>()
        {
            return GetProviderKey(typeof(T));
        }

        /// <summary>
        /// The get provider key.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetProviderKey(Type type)
        {
            return GetProviderKeys(type).FirstOrDefault();
        }

        /// <summary>
        /// Gets the provider keys for a given type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{Guid}"/>.
        /// </returns>
        public IEnumerable<Guid> GetProviderKeys<T>()
        {
            return GetProviderKeys(typeof(T));
        }

        /// <summary>
        /// Gets the provider keys for a given type.
        /// </summary>
        /// <param name="type">
        /// The type of the provider.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Guid}"/>.
        /// </returns>
        public IEnumerable<Guid> GetProviderKeys(Type type)
        {
            var foundTypes = _instanceTypes.Where(type.IsAssignableFrom);
            var foundArray = foundTypes as Type[] ?? foundTypes.ToArray();
            return foundArray.Any()
                ? foundArray.Select(x => x.GetCustomAttribute<EntityCollectionProviderAttribute>(false).Key) :
                Enumerable.Empty<Guid>();
        }

        /// <summary>
        /// Gets the <see cref="EntityCollectionProviderAttribute"/> from the provider of type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityCollectionProviderAttribute"/>.
        /// </returns>
        public EntityCollectionProviderAttribute GetProviderAttribute<T>()
        {
            return GetProviderAttributes<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="EntityCollectionProviderAttribute"/> from the provider of type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionProviderAttribute}"/>.
        /// </returns>
        public IEnumerable<EntityCollectionProviderAttribute> GetProviderAttributes<T>()
        {
            return GetProviderAttribute(typeof(T));
        }

        /// <summary>
        /// Gets the provider attributes for all resolved types.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionProviderAttribute}"/>.
        /// </returns>
        public IEnumerable<EntityCollectionProviderAttribute> GetProviderAttributes()
        {
            this.EnsureInitialized();
            return _instanceTypes.Select(x => x.GetCustomAttribute<EntityCollectionProviderAttribute>(false));
        }

        /// <summary>
        /// Gets a collection of resolved entity collection provider types for a specific entity type.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        public IEnumerable<Type> GetProviderTypesForEntityType(EntityType entityType)
        {
            this.EnsureInitialized();

            return
                _instanceTypes.Where(
                    x =>
                    x.GetCustomAttribute<EntityCollectionProviderAttribute>(false).EntityTfKey
                    == EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey);
        }

        /// <summary>
        /// The get provider for collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        public Attempt<EntityCollectionProviderBase> GetProviderForCollection(Guid collectionKey)
        {
            this.EnsureInitialized();

            // check the cache
            if (_entityCollectionProviderCache.ContainsKey(collectionKey))
            {
                var attempt = this.CreateInstance(_entityCollectionProviderCache[collectionKey], collectionKey);
                return attempt;
            }

            var nullReference = new NullReferenceException("EntityCollectionProvider could not be resolved for the collection.");
            return Attempt<EntityCollectionProviderBase>.Fail(nullReference);
        }

        /// <summary>
        /// Gets the provider attribute for providers responsible for specified filter attribute collections.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderAttribute"/>.
        /// </returns>
        public EntityCollectionProviderAttribute GetProviderAttributeForFilter(Guid collectionKey)
        {
            var attempt = GetProviderForCollection(collectionKey);
            if (!attempt.Success)
            {
                MultiLogHelper.WarnWithException<EntityCollectionProviderResolver>(
                    "Failed to retreive provider for collection",
                    attempt.Exception);

                return null;
            }

            var provider = attempt.Result as IEntityFilterGroupProvider;
            if (provider == null)
            {
                var nullRef = new NullReferenceException("Provider found did not implement IEntityFilterProvider");
                MultiLogHelper.WarnWithException<EntityCollectionProviderResolver>("Invalid type", nullRef);
                return null;
            }

            return GetProviderAttribute(provider.FilterProviderType).FirstOrDefault();
        }

        /// <summary>
        /// The get provider for collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <typeparam name="T">
        /// The type of the provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public Attempt<T> GetProviderForCollection<T>(Guid collectionKey) where T : EntityCollectionProviderBase
        {
            var attempt = GetProviderForCollection(collectionKey);
            return attempt.Success ? 
               attempt.Result is T ? 
               Attempt<T>.Succeed(attempt.Result as T) :
               Attempt<T>.Fail(new InvalidCastException("Provider was resolved but was not of expected type."))
               : Attempt<T>.Fail(attempt.Exception);
        }

        /// <summary>
        /// Adds or Updates provider type definitions in the concurrent dictionary cache.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection Key.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        internal void AddOrUpdateCache(Guid collectionKey, Type provider)
        {
            _entityCollectionProviderCache.AddOrUpdate(collectionKey, provider, (x, y) => provider);
        }

        /// <summary>
        /// The add or update cache.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        internal void AddOrUpdateCache(IEntityCollection collection)
        {
            var type = this.GetTypeByProviderKey(collection.ProviderKey);
            if (type != null) AddOrUpdateCache(collection.Key, type);
        }

        /// <summary>
        /// Removes provider type definitions in the concurrent dictionary cache.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        internal void RemoveFromCache(Guid collectionKey)
        {
            this.EnsureInitialized();

            Type provider;
            if (!_entityCollectionProviderCache.TryRemove(collectionKey, out provider))
            {
                LogHelper.Info<EntityCollectionProviderResolver>("Failed to remove provider associated with collect " + collectionKey + " from cache");
            }
        }

        /// <summary>
        /// Gets the provider attributes of providers with matching types
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionProviderAttribute}"/>.
        /// </returns>
        private IEnumerable<EntityCollectionProviderAttribute> GetProviderAttribute(Type type)
        {
            this.EnsureInitialized();

            var foundTypes = _instanceTypes.Where(type.IsAssignableFrom);
            var typesArray = foundTypes as Type[] ?? foundTypes.ToArray();
            return typesArray.Any() ?
                typesArray.Select(x => x.GetCustomAttribute<EntityCollectionProviderAttribute>(false))
                : Enumerable.Empty<EntityCollectionProviderAttribute>();
        }

        /// <summary>
        /// The create instance.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="collectionKey">
        /// The collection Key.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        private Attempt<EntityCollectionProviderBase> CreateInstance(Type provider, Guid collectionKey)
        {
            return ActivatorHelper.CreateInstance<EntityCollectionProviderBase>(
                provider,
                new object[] { _merchelloContext, collectionKey });
        }

        /// <summary>
        /// The get type by provider key.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        private Type GetTypeByProviderKey(Guid providerKey)
        {
            return 
                _instanceTypes.FirstOrDefault(
                    x => x.GetCustomAttribute<EntityCollectionProviderAttribute>(true).Key == providerKey);
        }

        /// <summary>
        /// The ensure initialized.
        /// </summary>
        private void EnsureInitialized()
        {
            var defined =
                _instanceTypes
                    .Select(x => x.GetCustomAttribute<EntityCollectionProviderAttribute>(false).Key).Distinct().Count();

            var registered =
                _entityCollectionProviderCache.Select(
                    x => x.Value.GetCustomAttribute<EntityCollectionProviderAttribute>(false).Key).Distinct().Count();

            if (defined < registered) IsInitialized = false;
            if (!IsInitialized) this.Initialize();
        }

        /// <summary>
        /// Ensures the provider.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool EnusureUniqueProvider(Guid providerKey)
        {
            //if (_entityCollectionProviderCache.Any(x => x.Value.GetCustomAttribute<EntityCollectionProviderAttribute>(false).Key == providerKey)) return false;
           
            return _merchelloContext.Services.EntityCollectionService.CollectionCountManagedByProvider(providerKey) <= 1;
        }

        /// <summary>
        /// Initializes the resolver.
        /// </summary>
        private void Initialize()
        {
            var collections = _merchelloContext.Services.EntityCollectionService.GetAll().ToArray();

            foreach (var collection in collections)
            {
                var type = this.GetTypeByProviderKey(collection.ProviderKey);
                if (type != null)
                {
                    var att = type.GetCustomAttribute<EntityCollectionProviderAttribute>(false);
                    this.AddOrUpdateCache(collection.Key, type);
                }
                else
                {
                    // remove this collection
                    _merchelloContext.Services.EntityCollectionService.Delete(collection);
                }
            }

            // Find any providers that should need to register themselves
            var unregistered =
                _instanceTypes.Where(
                    x =>
                    x.GetCustomAttribute<EntityCollectionProviderAttribute>(false).ManagesUniqueCollection
                    && collections.All(
                        y => y.ProviderKey != x.GetCustomAttribute<EntityCollectionProviderAttribute>(false).Key));

            foreach (var reg in unregistered)
            {
                var att = reg.GetCustomAttribute<EntityCollectionProviderAttribute>(false);

                if (EnusureUniqueProvider(att.Key))
                {
                    var collection = ((EntityCollectionService)_merchelloContext.Services.EntityCollectionService).CreateEntityCollectionWithKey(
                        att.EntityTfKey,
                        att.Key,
                        att.Name);

                    this.AddOrUpdateCache(collection.Key, reg);
                }
            }

            IsInitialized = true;
        }
    }
}