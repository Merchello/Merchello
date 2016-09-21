namespace Merchello.Umbraco.Adapters
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Acquired;
    using Merchello.Core.Cache;

    /// <inheritdoc/>
    internal class IsolatedRuntimeCacheAdapter : IIsolatedRuntimeCache, IUmbracoAdapter
    {
        /// <summary>
        /// The Umbraco's IsolatedRuntimeCache.
        /// </summary>
        private readonly global::Umbraco.Core.Cache.IsolatedRuntimeCache _isolated;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsolatedRuntimeCacheAdapter"/> class.
        /// </summary>
        /// <param name="isolated">
        /// The isolated runtime cache.
        /// </param>
        public IsolatedRuntimeCacheAdapter(global::Umbraco.Core.Cache.IsolatedRuntimeCache isolated)
        {
            Ensure.ParameterNotNull(isolated, nameof(isolated));

            _isolated = isolated;
        }

        /// <inheritdoc/>
        public void ClearAllCaches()
        {
            _isolated.ClearAllCaches();
        }

        /// <inheritdoc/>
        public void ClearCache<T>()
        {
            _isolated.ClearCache<T>();
        }

        /// <inheritdoc/>
        public IAttempt<IRuntimeCacheProvider> GetCache<T>()
        {
            var attempt = _isolated.GetCache<T>();

            var result = attempt.Result != null ? new RuntimeCacheProviderAdapter(attempt.Result) : null;

            return attempt.Success
                       ? Attempt<IRuntimeCacheProvider>.Succeed(result)
                       : Attempt<IRuntimeCacheProvider>.Fail(result, attempt.Exception);
        }

        /// <inheritdoc/>
        public IRuntimeCacheProvider GetOrCreateCache(Type type)
        {
            return new RuntimeCacheProviderAdapter(_isolated.GetOrCreateCache(type));
        }

        /// <inheritdoc/>
        public IRuntimeCacheProvider GetOrCreateCache<T>()
        {
            return new RuntimeCacheProviderAdapter(_isolated.GetOrCreateCache<T>());
        }
    }
}