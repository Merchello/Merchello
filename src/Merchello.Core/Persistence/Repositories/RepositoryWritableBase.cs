namespace Merchello.Core.Persistence.Repositories
{
    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.UnitOfWork;

    /// <summary>
    /// Represents a writable repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity.
    /// </typeparam>
    internal abstract class RepositoryWritableBase<TEntity> : RepositoryReadableQueryableBase<TEntity>, IRepositoryWritable<TEntity>, IUnitOfWorkRepository
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWritableBase{TEntity}"/> class.
        /// </summary>
        /// <param name="work">
        /// The unit of work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        protected RepositoryWritableBase(IUnitOfWork work, ICacheHelper cache, ILogger logger)
            : base(work, cache, logger)
        {
        }

        /// <inheritdoc/>
        public void AddOrUpdate(TEntity entity)
        {
            if (entity.HasIdentity == false)
            {
                UnitOfWork.RegisterCreated(entity, this);
            }
            else
            {
                UnitOfWork.RegisterUpdated(entity, this);
            }
        }

        /// <inheritdoc/>
        public void Delete(TEntity entity)
        {
            UnitOfWork?.RegisterDeleted(entity, this);
        }

        /// <inheritdoc/>
        public void PersistNewItem(IEntity entity)
        {
            CachePolicy.Create((TEntity)entity, PersistNewItem);
        }

        /// <inheritdoc/>
        public void PersistUpdatedItem(IEntity entity)
        {
            CachePolicy.Update((TEntity)entity, PersistNewItem);
        }

        /// <inheritdoc/>
        public void PersistDeletedItem(IEntity entity)
        {
            CachePolicy.Delete((TEntity)entity, PersistNewItem);
        }

        /// <summary>
        /// Performs the actual saving of a new entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected abstract void PersistNewItem(TEntity entity);

        /// <summary>
        /// Preforms the actual updating of an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected abstract void PersistUpdatedItem(TEntity entity);

        /// <summary>
        /// Preforms the actual delete of the entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected abstract void PersistDeletedItem(TEntity entity);
    }
}