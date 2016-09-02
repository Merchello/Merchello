namespace Merchello.Web.Services
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Services;

    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for EntityCollection proxy services.
    /// </summary>
    internal abstract class EntityCollectionProxyServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProxyServiceBase"/> class.
        /// </summary>
        /// <param name="entityCollectionService">
        /// The services.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        protected EntityCollectionProxyServiceBase(IEntityCollectionService entityCollectionService, ICacheProvider cache)
        {
            Ensure.ParameterNotNull(entityCollectionService, "The EntityCollectionService was null");
            Ensure.ParameterNotNull(cache, "The Cache was null");
            this.Service = entityCollectionService;
            this.Cache = cache;
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        protected ICacheProvider Cache { get; private set; }

        /// <summary>
        /// Gets the service.
        /// </summary>
        protected IEntityCollectionService Service { get; private set; }

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