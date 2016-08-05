namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Defines a paged query cache.
    /// </summary>
    public interface IPagedKeyQueryCache
    {
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
        Page<Guid> CachePage(string cacheKey, Page<Guid> p);

        /// <summary>
        /// Gets a page by it's cache key.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetPageByCacheKey(string cacheKey);

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
        string GetPagedQueryCacheKey<TSender>(
            string methodName,
            long page,
            long itemsPerPage,
            string sortBy,
            SortDirection sortDirection,
            IDictionary<string, string> args = null);
    }
}