namespace Merchello.Umbraco.Models
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Cache;
    using Merchello.Core.Models.EntityBase;

    /// <inheritdoc/>
    internal class CacheSurrogate<TEntity> : CacheModelBase<TEntity>, ICacheSurrogate<TEntity>
        where TEntity : class, IEntity, ITracksDirty, IDeepCloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSurrogate{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to be cached.
        /// </param>
        public CacheSurrogate(TEntity entity)
            : base(entity)
        {
        }

        /// <summary>
        /// Implementation of Umbraco's IDeepCloneable.
        /// </summary>
        /// <returns>
        /// The deep cloned object.
        /// </returns>
        public object DeepClone()
        {
            var model = this.InnerDeepClone();
            return new CacheSurrogate<TEntity>(model);
        }
    }
}