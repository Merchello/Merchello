namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Events;
    using Umbraco.Core.Models;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// Represents a virtual <see cref="IPublishedContent"/> cache.
    /// </summary>
    /// <typeparam name="TContent">
    /// The type of <see cref="IPublishedContent"/>
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of associated entity to be used in cache purges
    /// </typeparam>
    public abstract class VirtualContentCache<TContent, TEntity> : IVirtualContentCache<TContent, TEntity>
        where TContent : IPublishedContent
        where TEntity : IEntity
    {
        /// <summary>
        /// The <see cref="IRuntimeCacheProvider"/>.
        /// </summary>
        private readonly CacheHelper _cache;

        /// <summary>
        /// A function used to fetch the IPublishedContent in the case it was not found in cache
        /// </summary>
        private readonly Func<Guid, TContent> _fetch;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualContentCache{TContent,TEntity}"/> class. 
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="fetch">
        /// The fetch.
        /// </param>
        /// <param name="modified">
        /// The modified.
        /// </param>
        protected VirtualContentCache(CacheHelper cache, Func<Guid, TContent> fetch, bool modified)
        {
            Mandate.ParameterNotNull(cache, "cache");
            _cache = cache;
            _fetch = fetch;
            ModifiedVersion = modified;
        }

        /// <summary>
        /// Gets or sets a value indicating whether modified version.
        /// </summary>
        internal bool ModifiedVersion { get; set; }

        /// <summary>
        /// Gets the <see cref="IRuntimeCacheProvider"/>.
        /// </summary>
        protected CacheHelper Cache
        {
            get
            {
                return _cache;
            }
        }

        /// <summary>
        /// Gets the virtual content by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The virtual <see cref="IPublishedContent"/>.
        /// </returns>
        public TContent GetByKey(Guid key)
        {
            //return _fetch.Invoke(key);

            var cacheKey = GetCacheKey(key, ModifiedVersion);

            var content = (TContent)_cache.RuntimeCache.GetCacheItem(cacheKey);
            if (content != null) return content;

            return _fetch != null ? 
                CacheContent(cacheKey, _fetch.Invoke(key)) :
                default(TContent);
        }

        /// <summary>
        /// Gets virtual <see cref="IPublishedContent"/> by it's key.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TContent}"/>.
        /// </returns>
        public IEnumerable<TContent> GetByKeys(IEnumerable<Guid> keys)
        {
            return keys.Where(x => x != Guid.Empty).Select(GetByKey);
        }


        /// <summary>
        /// Gets a cached paged collection by it's cacheKey.
        /// </summary>
        /// <param name="pagedKeys">
        /// The page <see cref="Guid"/>.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<TContent> GetPagedCollectionByCacheKey(Page<Guid> pagedKeys, string sortBy)
        {
            return pagedKeys != null ?
                MapPagedCollection(pagedKeys, sortBy) :
                null;
        }

        /// <summary>
        /// Maps a <see cref="Page{Guid}"/> to <see cref="PagedCollection{TContent}"/>.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public virtual PagedCollection<TContent> MapPagedCollection(Page<Guid> page, string sortBy)
        {
            var items = page.Items.Select(GetByKey).Where(x => x != null).ToArray();

            if (items.Count() != page.Items.Count)
            {
                MultiLogHelper.Debug<VirtualContentCache<TContent, TEntity>>("Could not map all items to virtual content");
            }

            return new PagedCollection<TContent>
            {
                CurrentPage = page.CurrentPage,
                PageSize = items.Count(),
                TotalPages = page.TotalPages,
                TotalItems = page.TotalItems,
                Items = items,
                SortField = sortBy
            };
        }

        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public virtual void ClearVirtualCache(SaveEventArgs<TEntity> e)
        {
            ClearVirtualCache(e.SavedEntities);
        }

        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public virtual void ClearVirtualCache(DeleteEventArgs<TEntity> e)
        {
            ClearVirtualCache(e.DeletedEntities);
        }

        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public virtual void ClearVirtualCache(Guid key)
        {
            _cache.RuntimeCache.ClearCacheItem(GetCacheKey(key, true));
            _cache.RuntimeCache.ClearCacheItem(GetCacheKey(key, false));
        }

        /// <summary>
        /// Caches content.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The typed <see cref="IPublishedContent"/>.
        /// </returns>
        internal virtual TContent CacheContent(string cacheKey, TContent content)
        {
            if (content == null) return default(TContent);
            _cache.RuntimeCache.GetCacheItem(cacheKey, () => content, TimeSpan.FromMinutes(15), true);
            return content;
        }


        /// <summary>
        /// The internal cache key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="modified">
        /// The modified.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetCacheKey(Guid key, bool modified)
        {
            // use the key first so we can clear it more easily
            return string.Format("{0}.{1}.{2}", key, typeof(TContent), modified);
        }

        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        private void ClearVirtualCache(IEnumerable<TEntity> entities)
        {
            foreach (var e in entities)
            {
                ClearVirtualCache(e);
            }
        }

        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        private void ClearVirtualCache(TEntity entity)
        {
            ClearVirtualCache(entity.Key);
        }
    }
}