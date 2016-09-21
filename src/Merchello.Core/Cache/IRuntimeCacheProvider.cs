namespace Merchello.Core.Cache
{
    using System;
    using System.Web.Caching;

    using CacheItemPriority = System.Web.Caching.CacheItemPriority;

    /// <summary>
    /// An abstract class for implementing a runtime cache provider
    /// </summary>
    internal interface IRuntimeCacheProvider : ICacheProvider
    {
        /// <summary>
        /// Gets a cached item.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="getCacheItem">
        /// The get cache item.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <param name="isSliding">
        /// The is sliding.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="removedCallback">
        /// The removed callback.
        /// </param>
        /// <param name="dependentFiles">
        /// The dependent files.
        /// </param>
        /// <returns>
        /// The cached object.
        /// </returns>
        object GetCacheItem(
            string cacheKey, 
            Func<object> getCacheItem, 
            TimeSpan? timeout,
            bool isSliding = false,
            CacheItemPriority priority = CacheItemPriority.Normal,
            CacheItemRemovedCallback removedCallback = null,
            string[] dependentFiles = null);

        /// <summary>
        /// Inserts a cached item.
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="getCacheItem">
        /// The get cache item.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <param name="isSliding">
        /// The is sliding.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="removedCallback">
        /// The removed callback.
        /// </param>
        /// <param name="dependentFiles">
        /// The dependent files.
        /// </param>
        void InsertCacheItem(
            string cacheKey,
            Func<object> getCacheItem,
            TimeSpan? timeout = null,
            bool isSliding = false,
            CacheItemPriority priority = CacheItemPriority.Normal,
            CacheItemRemovedCallback removedCallback = null,
            string[] dependentFiles = null);
    }
}
