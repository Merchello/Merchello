namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// Represents a cache for paged Queries.
    /// </summary>
    internal class PagedKeyQueryCache : IPagedKeyQueryCache
    {
        /// <summary>
        /// The <see cref="ICacheProvider"/>.
        /// </summary>
        private readonly ICacheProvider _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedKeyQueryCache"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public PagedKeyQueryCache(CacheHelper cache)
        {
            Mandate.ParameterNotNull(cache, "cache");
            _cache = cache.RequestCache;
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
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> CachePage(string cacheKey, Page<Guid> p)
        {
            if (p == null) return default(Page<Guid>);

            return (Page<Guid>)_cache.GetCacheItem(cacheKey, () => p);
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
        public Page<Guid> GetPageByCacheKey(string cacheKey)
        {
            return (Page<Guid>)_cache.GetCacheItem(cacheKey);
        }

        /// <summary>
        /// Gets a cache key for storing paged collection query results.
        /// </summary>
        /// <typeparam name="TSender">
        /// The type of the sender
        /// </typeparam>
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
        public string GetPagedQueryCacheKey<TSender>(
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

            return string.Format("{0}{1}", GetPagedPrefix<TSender>(), sb.ToString().GetHashCode());
        }

        /// <summary>
        /// Gets the paged prefix.
        /// </summary>
        /// <typeparam name="TSender">
        /// The type of the sender
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetPagedPrefix<TSender>()
        {
            return string.Format("merchpage{0}", typeof(TSender));
        }
    }
}