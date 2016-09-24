namespace Merchello.Core.Cache
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// A base class for repository cache policies.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    internal abstract class RepositoryCachePolicyBase<TEntity> : IRepositoryCachePolicy<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCachePolicyBase{TEntity}"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        protected RepositoryCachePolicyBase(IRuntimeCacheProvider cache)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            this.Cache = cache;
        }

        /// <summary>
        /// Gets the runtime cache.
        /// </summary>
        protected IRuntimeCacheProvider Cache { get; }

        /// <inheritdoc/>
        public abstract TEntity Get(Guid key, Func<Guid, TEntity> performGet, Func<Guid[], IEnumerable<TEntity>> performGetAll);

        /// <inheritdoc/>
        public abstract TEntity GetCached(Guid key);

        /// <inheritdoc/>
        public abstract bool Exists(Guid key, Func<Guid, bool> performExists, Func<Guid[], IEnumerable<TEntity>> performGetAll);

        /// <inheritdoc/>
        public abstract void Create(TEntity entity, Action<TEntity> persistNew);

        /// <inheritdoc/>
        public abstract void Update(TEntity entity, Action<TEntity> persistUpdated);

        /// <inheritdoc/>
        public abstract void Delete(TEntity entity, Action<TEntity> persistDeleted);

        /// <inheritdoc/>
        public abstract TEntity[] GetAll(Guid[] keys, Func<Guid[], IEnumerable<TEntity>> performGetAll);

        /// <inheritdoc/>
        public abstract void ClearAll();
    }
}