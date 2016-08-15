namespace Merchello.Core.Persistence.UnitOfWork
{
    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core.Persistence.UnitOfWork;

    /// <summary>
    /// Defines a Unit Of Work
    /// </summary>
    /// <remarks>
    /// This is required due to Umbraco's IUnitOfWork dependency on Umbraco.Core.Models.EntityBase.IEntity
    /// </remarks>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Queues an insert
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        void RegisterAdded(IEntity entity, IUnitOfWorkRepository repository);

        /// <summary>
        /// Queues an update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        void RegisterChanged(IEntity entity, IUnitOfWorkRepository repository);

        /// <summary>
        /// Queues a delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        void RegisterRemoved(IEntity entity, IUnitOfWorkRepository repository);

        /// <summary>
        /// Commits a transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// The commit a bulk transaction.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The type of the entities
        /// </typeparam>
        void CommitBulk<TEntity>() where TEntity : IEntity;

        /// <summary>
        /// Gets the key.
        /// </summary>
        object Key { get; }
    }
}