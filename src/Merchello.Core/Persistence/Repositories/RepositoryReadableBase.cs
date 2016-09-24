namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    /// <inheritdoc/>
    internal abstract class RepositoryReadableBase<TEntity> : RepositoryBase, IRepositoryReadable<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// The caching policy.
        /// </summary>
        private IRepositoryCachePolicy<TEntity> _cachePolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryReadableBase{TEntity}"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        protected RepositoryReadableBase(IUnitOfWork work, ICacheHelper cache, ILogger logger)
            : base(work, cache, logger)
        {
        }

        /// <summary>
        /// Gets the cache policy.
        /// </summary>
        protected virtual IRepositoryCachePolicy<TEntity> CachePolicy
        {
            get
            {
                if (_cachePolicy != null) return _cachePolicy;

                _cachePolicy = new DefaultRepositoryCachePolicy<TEntity>(RuntimeCache, new RepositoryCachePolicyOptions());

                return _cachePolicy;
            }
        }

        /// <inheritdoc/>
        public TEntity Get(Guid key)
        {
            return CachePolicy.Get(key, PerformGet, PerformGetAll);
        }

        /// <inheritdoc/>
        public IEnumerable<TEntity> GetAll(params Guid[] keys)
        {
            return CachePolicy.GetAll(keys, PerformGetAll);
        }

        /// <inheritdoc/>
        public bool Exists(Guid key)
        {
            return CachePolicy.Exists(key, PerformExists, PerformGetAll);
        }

        /// <summary>
        /// Performs the actual get by key query.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        protected abstract TEntity PerformGet(Guid key);

        /// <summary>
        /// Performs the actual "get all" query.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TEntity}"/>.
        /// </returns>
        protected abstract IEnumerable<TEntity> PerformGetAll(params Guid[] keys);

        /// <summary>
        /// Performs exists query.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the entity exists.
        /// </returns>
        protected abstract bool PerformExists(Guid key);
    }
}