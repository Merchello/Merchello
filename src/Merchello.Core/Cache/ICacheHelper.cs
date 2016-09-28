namespace Merchello.Core.Cache
{
    /// <summary>
    /// Defines a CacheHelper.
    /// </summary>
    internal interface ICacheHelper
    {
        /// <summary>
        /// Gets the <see cref="ICacheProvider"/> used for request caching.
        /// </summary>
        ICacheProvider RequestCache { get; }

        /// <summary>
        /// Gets the <see cref="IRuntimeCacheProvider"/> used for runtime caching.
        /// </summary>
        IRuntimeCacheProvider RuntimeCache { get; }

        /// <summary>
        /// Gets the <see cref="ICacheProvider"/> used for static (in memory) caches.
        /// </summary>
        ICacheProvider StaticCache { get; }

        /// <summary>
        /// Gets the  <see cref="IIsolatedRuntimeCache"/> cache.
        /// </summary>
        /// <remarks>
        /// Useful for repository level caches to ensure that cache lookups by key are fast so 
        /// that the repository doesn't need to search through all keys on a global scale.
        /// </remarks>
        IIsolatedRuntimeCache IsolatedRuntimeCache { get; }
    }
}