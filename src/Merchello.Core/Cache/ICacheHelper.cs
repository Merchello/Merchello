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
    }
}