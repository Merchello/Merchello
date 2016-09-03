namespace Merchello.Web.Search
{
    using System;

    using Merchello.Core;

    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for entity proxy queries.
    /// </summary>
    internal abstract class ProxyQueryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyQueryBase"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        protected ProxyQueryBase(ICacheProvider cache)
        {
            Ensure.ParameterNotNull(cache, "The Cache was null");
            this.Cache = cache;
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        protected ICacheProvider Cache { get; private set; }

        /// <summary>
        /// Gets the cache key for request caching.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The cacheKey.
        /// </returns>
        protected string GetCacheKey(string methodName, params Guid[] keys)
        {
            return string.Format("{0}.{1}.{2}", methodName, this.GetType(), string.Join(string.Empty, keys));
        }
    }
}