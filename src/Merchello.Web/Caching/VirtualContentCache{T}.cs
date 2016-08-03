namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
    internal abstract class VirtualContentCache<TContent, TEntity> : IVirtualContentCache<TContent, TEntity>
        where TContent : IPublishedContent
        where TEntity : IEntity
    {
        /// <summary>
        /// The <see cref="IRuntimeCacheProvider"/>.
        /// </summary>
        private readonly IRuntimeCacheProvider _cache;

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
        protected VirtualContentCache(IRuntimeCacheProvider cache, Func<Guid, TContent> fetch)
        {
            Mandate.ParameterNotNull(cache, "cache");
            _cache = cache;
            _fetch = fetch;
        }

        /// <summary>
        /// Gets the <see cref="IRuntimeCacheProvider"/>.
        /// </summary>
        protected IRuntimeCacheProvider Cache
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
            var cacheKey = GetCacheKey(key);

            var content = (TContent)_cache.GetCacheItem(cacheKey);
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
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<TContent> GetPagedCollectionByCacheKey(string cacheKey, string sortBy)
        {
            var p = GetPageByCacheKey(cacheKey);
            return p != null ?
                MapPagedCollection(p, sortBy) :
                null;
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
        /// <param name="entities">
        /// The entities.
        /// </param>
        private void ClearVirtualCache(IEnumerable<TEntity> entities)
        {
            foreach (var e in entities)
            {
                ClearVirtualCache(e);
            }

            _cache.ClearCacheByKeySearch(GetPagedPrefix());
        }

        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        private void ClearVirtualCache(TEntity entity)
        {
            _cache.ClearCacheItem(GetCacheKey(entity.Key));
        }

        /// <summary>
        /// Gets a cache key for storing paged collection query results.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetPagedQueryCacheKey(
            string methodName,
            long page,
            long itemsPerPage,
            string sortBy,
            SortDirection sortDirection,
            IDictionary<string, string> args = null)
        {
            var sb = new StringBuilder();
            sb.Append(methodName)
            .Append(page)
            .Append(itemsPerPage)
            .Append(sortBy)
            .Append(sortDirection);

            if (args != null)
            {
                foreach (var key in args.Keys)
                {
                    sb.Append(string.Format("{0}.{1}", key, args[key]));
                }
            }

            return string.Format("{0}{1}", GetPagedPrefix(), sb.ToString().GetHashCode());
        }

        /// <summary>
        /// Gets a page by it's cache key.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal virtual Page<Guid> GetPageByCacheKey(string cacheKey)
        {
            return (Page<Guid>)_cache.GetCacheItem(cacheKey);
        }

        /// <summary>
        /// Caches a page.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        internal virtual void CachePage(string cacheKey, Page<Guid> p)
        {
            _cache.GetCacheItem(cacheKey, () => p);
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
            _cache.GetCacheItem(cacheKey, () => content);
            return content;
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
        protected virtual PagedCollection<TContent> MapPagedCollection(Page<Guid> page, string sortBy)
        {
            var items = page.Items.Select(GetByKey).Where(x => x != null).ToArray();

            if (items.Count() != page.ItemsPerPage)
            {
                MultiLogHelper.Warn<VirtualContentCache<TContent, TEntity>>("Could not map all items to virtual content");
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
        /// Gets the paged prefix.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetPagedPrefix()
        {
            return string.Format("merchpagevcontent{0}", typeof(TEntity));
        }

        /// <summary>
        /// The internal cache key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetCacheKey(Guid key)
        {
            // use the key first so we can clear it more easily
            return string.Format("{0}.{1}", key, typeof(TContent));
        }
    }

}