namespace Merchello.Core.Cache
{
    using System;

    /// <summary>
    /// Represents a cache refresher for load balancing.
    /// </summary>
    public interface ICacheRefresher
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        Guid UniqueIdentifier { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Refreshes all cached items.
        /// </summary>
        void RefreshAll();

        /// <summary>
        /// Refreshes a cached item.
        /// </summary>
        /// <param name="key">
        /// The key of the cached item.
        /// </param>
        void Refresh(Guid key);

        /// <summary>
        /// Removes a cached item.
        /// </summary>
        /// <param name="key">
        /// The key of the cached item.
        /// </param>
        void Remove(Guid key);
    }

    /// <summary>
    /// Strongly type cache refresher that is able to refresh cache of real instances of objects as well as keys
    /// </summary>
    /// <typeparam name="T">
    /// The type of the cached item
    /// </typeparam>
    /// <remarks>
    /// This is much better for performance when we're not running in a load balanced environment so we can refresh the cache
    /// against a already resolved object instead of looking the object back up by it's key. 
    /// </remarks>
    public interface ICacheRefresher<in T> : ICacheRefresher
    {
        /// <summary>
        /// Refreshes the cached instance.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        void Refresh(T instance);

        /// <summary>
        /// Removes a cached instance.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        void Remove(T instance);
    }
}