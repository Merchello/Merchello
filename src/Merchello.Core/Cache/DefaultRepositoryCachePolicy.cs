namespace Merchello.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.Cache;
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents the default cache policy.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <remarks>
    /// <para>The default cache policy caches entities with a 5 minutes sliding expiration.</para>
    /// <para>Each entity is cached individually.</para>
    /// <para>If options.GetAllCacheAllowZeroCount then a 'zero-count' array is cached when GetAll finds nothing.</para>
    /// <para>If options.GetAllCacheValidateCount then we check against the db when getting many entities.</para>
    /// </remarks>
    /// This class is a modified version of Umbraco's class with the same name. Modified to use Merchello's entities.
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v8/src/Umbraco.Core/Cache/DefaultRepositoryCachePolicy.cs"/>
    internal class DefaultRepositoryCachePolicy<TEntity> : RepositoryCachePolicyBase<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// The empty entities.
        /// </summary>
        private static readonly TEntity[] EmptyEntities = new TEntity[0];

        /// <summary>
        /// The repository caching options.
        /// </summary>
        private readonly RepositoryCachePolicyOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRepositoryCachePolicy{TEntity}"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        public DefaultRepositoryCachePolicy(IRuntimeCacheProvider cache, RepositoryCachePolicyOptions options)
            : base(cache)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            this._options = options;
        }

        /// <inheritdoc/>
        public override TEntity Get(Guid key, Func<Guid, TEntity> performGet, Func<Guid[], IEnumerable<TEntity>> performGetAll)
        {
            var cacheKey = GetEntityCacheKey(key);
            var fromCache = Cache.GetCacheItem<TEntity>(cacheKey);

            // if found in cache then return else fetch and cache
            if (fromCache != null)
                return fromCache;
            var entity = performGet(key);

            if (entity != null && entity.HasIdentity)
                InsertEntity(cacheKey, entity);

            return entity;
        }

        /// <inheritdoc/>
        public override TEntity GetCached(Guid key)
        {
            var cacheKey = GetEntityCacheKey(key);
            return Cache.GetCacheItem<TEntity>(cacheKey);
        }

        /// <inheritdoc/>
        public override bool Exists(Guid key, Func<Guid, bool> performExists, Func<Guid[], IEnumerable<TEntity>> performGetAll)
        {
            // if found in cache the return else check
            var cacheKey = GetEntityCacheKey(key);
            var fromCache = Cache.GetCacheItem<TEntity>(cacheKey);
            return fromCache != null || performExists(key);
        }

        /// <inheritdoc/>
        public override TEntity[] GetAll(Guid[] keys, Func<Guid[], IEnumerable<TEntity>> performGetAll)
        {
            if (keys.Length > 0)
            {
                // try to get each entity from the cache
                // if we can find all of them, return
                var entities = keys.Select(GetCached).WhereNotNull().ToArray();
                if (keys.Length.Equals(entities.Length))
                    return entities; // no need for null checks, we are not caching nulls
            }
            else
            {
                // get everything we have
                var entities = Cache.GetCacheItemsByKeySearch<TEntity>(GetEntityTypeCacheKey())
                    .ToArray(); // no need for null checks, we are not caching nulls

                if (entities.Length > 0)
                {
                    // if some of them were in the cache...
                    if (_options.GetAllCacheValidateCount)
                    {
                        // need to validate the count, get the actual count and return if ok
                        var totalCount = _options.PerformCount();
                        if (entities.Length == totalCount)
                            return entities;
                    }
                    else
                    {
                        // no need to validate, just return what we have and assume it's all there is
                        return entities;
                    }
                }
                else if (_options.GetAllCacheAllowZeroCount)
                {
                    // if none of them were in the cache
                    // and we allow zero count - check for the special (empty) entry
                    var empty = Cache.GetCacheItem<TEntity[]>(GetEntityTypeCacheKey());
                    if (empty != null) return empty;
                }
            }

            // cache failed, get from repo and cache
            var repoEntities = performGetAll(keys)
                .WhereNotNull() // exclude nulls!
                .Where(x => x.HasIdentity) // be safe, though would be weird...
                .ToArray();

            // note: if empty & allow zero count, will cache a special (empty) entry
            InsertEntities(keys, repoEntities);

            return repoEntities;
        }

        /// <inheritdoc />
        public override void ClearAll()
        {
            this.Cache.ClearCacheByKeySearch(this.GetEntityTypeCacheKey());
        }

        /// <inheritdoc />
        public override void Create(TEntity entity, Action<TEntity> persistNew)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                persistNew(entity);

                // just to be safe, we cannot cache an item without an identity
                if (entity.HasIdentity)
                {
                    this.Cache.InsertCacheItem(this.GetEntityCacheKey(entity.Key), () => entity, TimeSpan.FromMinutes(5), true);
                }

                // if there's a GetAllCacheAllowZeroCount cache, ensure it is cleared
                this.Cache.ClearCacheItem(this.GetEntityTypeCacheKey());
            }
            catch
            {
                // if an exception is thrown we need to remove the entry from cache
                this.Cache.ClearCacheItem(this.GetEntityCacheKey(entity.Key));

                // if there's a GetAllCacheAllowZeroCount cache, ensure it is cleared
                this.Cache.ClearCacheItem(this.GetEntityTypeCacheKey());

                throw;
            }
        }

        /// <inheritdoc />
        public override void Update(TEntity entity, Action<TEntity> persistUpdated)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                persistUpdated(entity);

                // just to be safe, we cannot cache an item without an identity
                if (entity.HasIdentity)
                {
                    this.Cache.InsertCacheItem(this.GetEntityCacheKey(entity.Key), () => entity, TimeSpan.FromMinutes(5), true);
                }

                // if there's a GetAllCacheAllowZeroCount cache, ensure it is cleared
                this.Cache.ClearCacheItem(this.GetEntityTypeCacheKey());
            }
            catch
            {
                // if an exception is thrown we need to remove the entry from cache
                this.Cache.ClearCacheItem(this.GetEntityCacheKey(entity.Key));

                // if there's a GetAllCacheAllowZeroCount cache, ensure it is cleared
                this.Cache.ClearCacheItem(this.GetEntityTypeCacheKey());

                throw;
            }
        }

        /// <inheritdoc />
        public override void Delete(TEntity entity, Action<TEntity> persistDeleted)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                persistDeleted(entity);
            }
            finally
            {
                // whatever happens, clear the cache
                var cacheKey = this.GetEntityCacheKey(entity.Key);
                this.Cache.ClearCacheItem(cacheKey);

                // if there's a GetAllCacheAllowZeroCount cache, ensure it is cleared
                this.Cache.ClearCacheItem(this.GetEntityTypeCacheKey());
            }
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The cache key.
        /// </returns>
        protected string GetEntityCacheKey(Guid key)
        {
            if (key == null || key.Equals(Guid.Empty)) throw new ArgumentNullException(nameof(key));
            return this.GetEntityTypeCacheKey() + key;
        }

        /// <summary>
        /// Gets the entity type cache key.
        /// </summary>
        /// <returns>
        /// The cache key.
        /// </returns>
        protected string GetEntityTypeCacheKey()
        {
            return $"mRepo_{typeof(TEntity).Name}_";
        }

        /// <summary>
        /// Inserts an entity into cache.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected virtual void InsertEntity(string cacheKey, TEntity entity)
        {
            this.Cache.InsertCacheItem(cacheKey, () => entity, TimeSpan.FromMinutes(5), true);
        }

        /// <summary>
        /// Inserts a collection of entities into cache.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <param name="entities">
        /// The entities.
        /// </param>
        protected virtual void InsertEntities(Guid[] keys, TEntity[] entities)
        {
            if (keys.Length == 0 && entities.Length == 0 && this._options.GetAllCacheAllowZeroCount)
            {
                // getting all of them, and finding nothing.
                // if we can cache a zero count, cache an empty array,
                // for as long as the cache is not cleared (no expiration)
                this.Cache.InsertCacheItem(this.GetEntityTypeCacheKey(), () => EmptyEntities);
            }
            else
            {
                // individually cache each item
                foreach (var entity in entities)
                {
                    var capture = entity;
                    this.Cache.InsertCacheItem(this.GetEntityCacheKey(entity.Key), () => capture, TimeSpan.FromMinutes(5), true);
                }
            }
        }
    }
}