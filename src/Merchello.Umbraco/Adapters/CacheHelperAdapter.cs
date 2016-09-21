namespace Merchello.Umbraco.Adapters
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Cache;

    /// <summary>
    /// Represents an adapter for Umbraco's CacheHelper.
    /// </summary>
    /// <remarks>
    /// This is for legacy purposes only. MerchelloContext.Current.Cache is obsolete.
    /// </remarks>
    internal sealed class CacheHelperAdapter : ICacheHelper, IUmbracoAdapter
    {
        /// <summary>
        /// Umbraco's CacheHelper..
        /// </summary>
        private readonly global::Umbraco.Core.Cache.CacheHelper _cacheHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelperAdapter"/> class. 
        /// <para>An adapter to use Umbraco's <see>
        ///         <cref>global::Umbraco.Core.CacheHelper</cref>
        ///     </see>
        ///     as <see cref="ICacheHelper"/></para>
        /// </summary>
        /// <param name="cache">
        /// Umbraco's CacheHelper.
        /// </param>
        public CacheHelperAdapter(global::Umbraco.Core.Cache.CacheHelper cache)
        {
            Ensure.ParameterNotNull(cache, "cache");

            _cacheHelper = cache;
        }

        /// <inheritdoc/>
        public ICacheProvider RequestCache
        {
            get
            {
                return new CacheProviderAdapter(_cacheHelper.RequestCache);
            }
        }

        /// <inheritdoc/>
        public IRuntimeCacheProvider RuntimeCache
        {
            get
            {
                return new RuntimeCacheProviderAdapter(_cacheHelper.RuntimeCache);
            }
        }

        /// <inheritdoc/>
        public ICacheProvider StaticCache
        {
            get
            {
                return new CacheProviderAdapter(_cacheHelper.StaticCache);
            }
        }

        /// <inheritdoc/>
        public IIsolatedRuntimeCache IsolatedRuntimeCache
        {
            get
            {
                return new IsolatedRuntimeCacheAdapter(_cacheHelper.IsolatedRuntimeCache);
            }
        }
    }
}