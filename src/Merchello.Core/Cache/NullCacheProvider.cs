namespace Merchello.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Caching;
    using Umbraco.Core.Cache;

    /// <summary>
    /// The null cache provider.
    /// </summary>
    internal class NullCacheProvider : IRuntimeCacheProvider
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        public virtual void ClearAllCache()
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public virtual void ClearCacheItem(string key)
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        public virtual void ClearCacheObjectTypes(string typeName)
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <typeparam name="T">
        /// The type of T
        /// </typeparam>
        public virtual void ClearCacheObjectTypes<T>()
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <typeparam name="T">
        /// The type of T
        /// </typeparam>
        public virtual void ClearCacheObjectTypes<T>(Func<string, T, bool> predicate)
        {
        }


        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="keyStartsWith">
        /// The key starts with.
        /// </param>
        public virtual void ClearCacheByKeySearch(string keyStartsWith)
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="regexString">
        /// The regex string.
        /// </param>
        public virtual void ClearCacheByKeyExpression(string regexString)
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="keyStartsWith">
        /// The key starts with.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Object}"/>.
        /// </returns>
        public virtual IEnumerable<object> GetCacheItemsByKeySearch(string keyStartsWith)
        {
            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="regexString">
        /// The regex string.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Object}"/>.
        /// </returns>
        public IEnumerable<object> GetCacheItemsByKeyExpression(string regexString)
        {
            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <returns>
        /// The default <see cref="object"/>.
        /// </returns>
        public virtual object GetCacheItem(string cacheKey)
        {
            return default(object);
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="getCacheItem">
        /// The get cache item.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object GetCacheItem(string cacheKey, Func<object> getCacheItem)
        {
            return getCacheItem();
        }

        /// <summary>
        /// Does nothing
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
        /// The <see cref="object"/>.
        /// </returns>
        public object GetCacheItem(string cacheKey, Func<object> getCacheItem, TimeSpan? timeout, bool isSliding = false, CacheItemPriority priority = CacheItemPriority.Normal, CacheItemRemovedCallback removedCallback = null, string[] dependentFiles = null)
        {
            return getCacheItem();
        }

        /// <summary>
        /// Does nothing
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
        public void InsertCacheItem(string cacheKey, Func<object> getCacheItem, TimeSpan? timeout = null, bool isSliding = false, CacheItemPriority priority = CacheItemPriority.Normal, CacheItemRemovedCallback removedCallback = null, string[] dependentFiles = null)
        {
            
        }
    }
}