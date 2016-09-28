namespace Merchello.Core.Cache
{
    using System;

    using Merchello.Core.Events;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Sync;

    /// <summary>
    /// A base class for cache refreshers to inherit from that ensures the correct events are raised
    /// when cache refreshing occurs.
    /// </summary>
    /// <typeparam name="TInstanceType">The real cache refresher type, this is used for raising strongly typed events</typeparam>
    internal abstract class CacheRefresherBase<TInstanceType> : ICacheRefresher
        where TInstanceType : ICacheRefresher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheRefresherBase{TInstanceType}"/> class.
        /// </summary>
        /// <param name="cacheHelper">
        /// The cache helper.
        /// </param>
        protected CacheRefresherBase(ICacheHelper cacheHelper)
        {
            this.CacheHelper = cacheHelper;
        }

        /// <summary>
        /// An event that is raised when cache is updated on an individual server
        /// </summary>
        /// <remarks>
        /// This event will fire on each server configured for an Umbraco project whenever a cache refresher
        /// is updated.
        /// </remarks>
        public event TypedEventHandler<TInstanceType, CacheRefresherEventArgs> CacheUpdated;

        /// <inheritdoc/>
        public abstract Guid UniqueIdentifier { get; }

        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the real instance of the object ('this') for use  in strongly typed events
        /// </summary>
        protected abstract TInstanceType Instance { get; }

        /// <inheritdoc/>
        protected ICacheHelper CacheHelper { get; }

        /// <inheritdoc/>
        public virtual void RefreshAll()
        {
            OnCacheUpdated(this.Instance, new CacheRefresherEventArgs(null, MessageType.RefreshAll));
        }

        /// <inheritdoc/>
        public virtual void Refresh(Guid key)
        {
            OnCacheUpdated(this.Instance, new CacheRefresherEventArgs(key, MessageType.RemoveByKey));
        }

        /// <inheritdoc/>
        public void Remove(Guid key)
        {
            OnCacheUpdated(this.Instance, new CacheRefresherEventArgs(key, MessageType.RemoveByKey));
        }

        /// <summary>
        /// Clears the cache for all repository entities of this type
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        internal void ClearAllIsolatedCacheByEntityType<TEntity>()
            where TEntity : class, IEntity
        {
            this.CacheHelper.IsolatedRuntimeCache.ClearCache<TEntity>();
        }

        /// <summary>
        /// Raises the event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event arguments</param>
        protected void OnCacheUpdated(TInstanceType sender, CacheRefresherEventArgs args)
        {
            if (CacheUpdated != null)
            {
                CacheUpdated(sender, args);
            }
        }
    }
}