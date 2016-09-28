namespace Merchello.Core.Cache
{
    using System;

    /// <summary>
    /// The IsolatedRuntimeCache interface.
    /// </summary>
    internal interface IIsolatedRuntimeCache
    {
        /// <summary>
        /// Clear all of the caches.
        /// </summary>
        void ClearAllCaches();

        /// <summary>
        /// Clears cache based on the isolated type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of that defines the isolated cache.
        /// </typeparam>
        void ClearCache<T>();

        /// <summary>
        /// Gets the cache for the isolated type.
        /// </summary>
        /// <typeparam name="T">
        /// The isolated type
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAttempt{T}"/>.
        /// </returns>
        IAttempt<IRuntimeCacheProvider> GetCache<T>();

        /// <summary>
        /// Gets or creates the isolated runtime cache.
        /// </summary>
        /// <param name="type">
        /// The type for the isolated cache.
        /// </param>
        /// <returns>
        /// The <see cref="IRuntimeCacheProvider"/>.
        /// </returns>
        IRuntimeCacheProvider GetOrCreateCache(Type type);

        /// <summary>
        /// Gets or creates the isolated runtime cache.
        /// </summary>
        /// <typeparam name="T">
        /// The type for the isolated cache.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IRuntimeCacheProvider"/>.
        /// </returns>
        IRuntimeCacheProvider GetOrCreateCache<T>();
    }
}