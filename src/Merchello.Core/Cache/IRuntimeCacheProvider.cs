namespace Merchello.Core.Cache
{
    using System;
    using System.Web.Caching;

    using CacheItemPriority = System.Web.Caching.CacheItemPriority;

    /// <summary>
    /// An abstract class for implementing a runtime cache provider
    /// </summary>
    public interface IRuntimeCacheProvider : ICacheProvider
    {
        object GetCacheItem(
            string cacheKey, 
            Func<object> getCacheItem, 
            TimeSpan? timeout,
            bool isSliding = false,
            CacheItemPriority priority = CacheItemPriority.Normal,
            CacheItemRemovedCallback removedCallback = null,
            string[] dependentFiles = null);

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
