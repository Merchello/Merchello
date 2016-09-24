namespace Merchello.Core.Cache
{
    using Merchello.Core.Events;
    using Merchello.Core.Sync;

    /// <summary>
    /// Provides a base class for "JSON" cache refreshers.
    /// </summary>
    /// <typeparam name="TInstance">The actual cache refresher type.</typeparam>
    /// <remarks>Ensures that the correct events are raised when cache refreshing occurs.</remarks>
    internal abstract class JsonCacheRefresherBase<TInstance> : CacheRefresherBase<TInstance>, IJsonCacheRefresher
        where TInstance : ICacheRefresher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCacheRefresherBase{TInstance}"/> class.
        /// </summary>
        /// <param name="cacheHelper">
        /// The cache helper.
        /// </param>
        protected JsonCacheRefresherBase(ICacheHelper cacheHelper) 
            : base(cacheHelper)
        {
        }

        /// <summary>
        /// Refreshes based on JSON.
        /// </summary>
        /// <param name="json">
        /// The JSON string.
        /// </param>
        public virtual void Refresh(string json)
        {            
            OnCacheUpdated(this.Instance, new CacheRefresherEventArgs(json, MessageType.RefreshByJson));
        }
    }
}