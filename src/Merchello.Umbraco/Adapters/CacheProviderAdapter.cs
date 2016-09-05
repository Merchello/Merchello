namespace Merchello.Umbraco.Adapters
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Cache;

    /// <summary>
    /// An adapter for using Umbraco's <see>
    ///         <cref>global::Umbraco.Core.Cache.ICacheProvider</cref>
    ///     </see> as <see cref="ICacheProvider"/>.
    /// </summary>
    internal class CacheProviderAdapter : ICacheProvider, IUmbracoAdapter
    {
        /// <summary>
        /// The Umbraco's cache provider.
        /// </summary>
        private readonly global::Umbraco.Core.Cache.ICacheProvider _cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheProviderAdapter"/> class.
        /// </summary>
        /// <param name="cacheProvider">
        /// Umbraco's actual cache provider.
        /// </param>
        public CacheProviderAdapter(global::Umbraco.Core.Cache.ICacheProvider cacheProvider)
        {
            Ensure.ParameterNotNull(cacheProvider, nameof(cacheProvider));
            _cacheProvider = cacheProvider;
        }

        /// <inheritdoc/>
        public void ClearAllCache()
        {
            _cacheProvider.ClearAllCache();
        }

        /// <inheritdoc/>
        public void ClearCacheItem(string key)
        {
            _cacheProvider.ClearCacheItem(key);
        }

        /// <inheritdoc/>
        public void ClearCacheObjectTypes(string typeName)
        {
            _cacheProvider.ClearCacheObjectTypes(typeName);
        }

        /// <inheritdoc/>
        public void ClearCacheObjectTypes<T>()
        {
            _cacheProvider.ClearCacheObjectTypes<T>();
        }

        /// <inheritdoc/>
        public void ClearCacheObjectTypes<T>(Func<string, T, bool> predicate)
        {
            _cacheProvider.ClearCacheObjectTypes<T>(predicate);
        }

        /// <inheritdoc/>
        public void ClearCacheByKeySearch(string keyStartsWith)
        {
            _cacheProvider.ClearCacheByKeySearch(keyStartsWith);
        }

        /// <inheritdoc/>
        public void ClearCacheByKeyExpression(string regexString)
        {
            _cacheProvider.ClearCacheByKeyExpression(regexString);
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetCacheItemsByKeySearch(string keyStartsWith)
        {
            return _cacheProvider.GetCacheItemsByKeySearch(keyStartsWith);
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetCacheItemsByKeyExpression(string regexString)
        {
            return _cacheProvider.GetCacheItemsByKeyExpression(regexString);
        }

        /// <inheritdoc/>
        public object GetCacheItem(string cacheKey)
        {
            return _cacheProvider.GetCacheItem(cacheKey);
        }

        /// <inheritdoc/>
        public object GetCacheItem(string cacheKey, Func<object> getCacheItem)
        {
            return _cacheProvider.GetCacheItem(cacheKey, getCacheItem);
        }
    }
}