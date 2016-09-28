namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Acquired;
    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    /// <summary>
    /// Represents a queryable repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity
    /// </typeparam>
    internal abstract class RepositoryReadableQueryableBase<TEntity> : RepositoryReadableBase<TEntity>, IRepositoryQueryable<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// The caching policy.
        /// </summary>
        private IRepositoryCachePolicy<TEntity> _cachePolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryReadableQueryableBase{TEntity}"/> class.
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
        protected RepositoryReadableQueryableBase(IUnitOfWork work, ICacheHelper cache, ILogger logger)
            : base(work, cache, logger)
        {
        }

        /// <inheritdoc/>
        public abstract IQuery<TEntity> Query { get; }

        /// <inheritdoc/>
        public abstract IQueryFactory QueryFactory { get; }

        /// <summary>
        /// Gets the cache policy.
        /// </summary>
        protected override IRepositoryCachePolicy<TEntity> CachePolicy
        {
            get
            {
                var options = new RepositoryCachePolicyOptions(() =>
                {
                    // Get count of all entities of current type (TEntity) to ensure cached result is correct
                    var query = Query.Where(x => x.Key != null && x.Key != Guid.Empty);
                    return PerformCount(query);
                });

                _cachePolicy = new DefaultRepositoryCachePolicy<TEntity>(RuntimeCache, options);

                return _cachePolicy;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TEntity> GetByQuery(IQuery<TEntity> query)
        {
            return PerformGetByQuery(query).WhereNotNull();
        }

        /// <inheritdoc/>
        public int Count(IQuery<TEntity> query)
        {
            return PerformCount(query);
        }

        /// <summary>
        /// Performs a query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntity}"/>.
        /// </returns>
        protected abstract IEnumerable<TEntity> PerformGetByQuery(IQuery<TEntity> query);

        /// <summary>
        /// Performs a count query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The count of entities.
        /// </returns>
        protected abstract int PerformCount(IQuery<TEntity> query);
    }
}