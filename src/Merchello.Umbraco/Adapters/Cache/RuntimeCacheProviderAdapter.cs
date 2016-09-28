namespace Merchello.Umbraco.Adapters
{
    using System;
    using System.Web.Caching;

    using Merchello.Core;
    using Merchello.Core.Cache;

    /// <summary>
    /// An adapter for using Umbraco's <see>
    ///         <cref>global::Umbraco.Core.Cache.IRuntimeCacheProvider</cref>
    ///     </see> as <see cref="IRuntimeCacheProvider"/>
    /// </summary>
    internal class RuntimeCacheProviderAdapter : CacheProviderAdapter, IRuntimeCacheProvider
    {
        /// <summary>
        /// Umbraco's runtime cache provider.
        /// </summary>
        private readonly global::Umbraco.Core.Cache.IRuntimeCacheProvider _cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeCacheProviderAdapter"/> class.
        /// </summary>
        /// <param name="cacheProvider">
        /// Umbraco's runtime cache provider.
        /// </param>
        public RuntimeCacheProviderAdapter(global::Umbraco.Core.Cache.IRuntimeCacheProvider cacheProvider)
            : base(cacheProvider)
        {
            Ensure.ParameterNotNull(cacheProvider, nameof(cacheProvider));
            this._cacheProvider = cacheProvider;
        }

        /// <inheritdoc/>
        public object GetCacheItem(
            string cacheKey,
            Func<object> getCacheItem,
            TimeSpan? timeout,
            bool isSliding = false,
            CacheItemPriority priority = CacheItemPriority.Normal,
            CacheItemRemovedCallback removedCallback = null,
            string[] dependentFiles = null)
        {
            return this._cacheProvider.GetCacheItem(
                cacheKey,
                getCacheItem,
                timeout,
                isSliding,
                priority,
                removedCallback,
                dependentFiles);
        }

        /// <inheritdoc/>
        public void InsertCacheItem(
            string cacheKey,
            Func<object> getCacheItem,
            TimeSpan? timeout = null,
            bool isSliding = false,
            CacheItemPriority priority = CacheItemPriority.Normal,
            CacheItemRemovedCallback removedCallback = null,
            string[] dependentFiles = null)
        {
            this._cacheProvider.InsertCacheItem(
                cacheKey,
                getCacheItem,
                timeout,
                isSliding,
                priority,
                removedCallback,
                dependentFiles);
        }
    }
}