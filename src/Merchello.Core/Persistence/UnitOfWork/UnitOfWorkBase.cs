namespace Merchello.Core.Persistence.UnitOfWork
{
    using System.Collections.Generic;

    using Merchello.Core.Acquired;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Repositories;

    /// <summary>
    /// Represents a Unit of Work.
    /// </summary>
    internal abstract class UnitOfWorkBase : DisposableObject, IUnitOfWork
    {
        /// <summary>
        /// The queue of operations.
        /// </summary>
        private readonly Queue<Operation> _operations = new Queue<Operation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkBase"/> class.
        /// </summary>
        /// <param name="factory">
        /// The repository factory.
        /// </param>
        protected UnitOfWorkBase(RepositoryFactory factory)
        {
            this.Factory = factory;
        }

        /// <summary>
        /// The types of unit of work operation.
        /// </summary>
        private enum OperationType
        {
            /// <summary>
            /// Insert operation
            /// </summary>
            Insert,

            /// <summary>
            /// Update operation
            /// </summary>
            Update,

            /// <summary>
            /// Delete operation.
            /// </summary>
            Delete
        }

        /// <summary>
        /// Gets the <see cref="RepositoryFactory"/>.
        /// </summary>
        protected RepositoryFactory Factory { get; }

        /// <summary>
        /// Gets a value indicating whether the unit of work is completed.
        /// </summary>
        protected bool Completed { get; private set; }

        /// <summary>
        /// Creates a repository.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <typeparam name="TRepository">
        /// The type of repository to create.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TRepository"/>.
        /// </returns>
        public abstract TRepository CreateRepository<TRepository>(string name = null)
            where TRepository : IRepository;

        /// <summary>
        /// Registers an entity to be added as part of this unit of work.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="repository">The repository in charge of the entity.</param>
        public void RegisterCreated(IEntity entity, IUnitOfWorkRepository repository)
        {
            this.Completed = false;
            this._operations.Enqueue(new Operation
            {
                Entity = entity,
                Repository = repository,
                Type = OperationType.Insert
            });
        }

        /// <summary>
        /// Registers an entity to be updated as part of this unit of work.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="repository">The repository in charge of the entity.</param>
        public void RegisterUpdated(IEntity entity, IUnitOfWorkRepository repository)
        {
            this.Completed = false;
            this._operations.Enqueue(new Operation
            {
                Entity = entity,
                Repository = repository,
                Type = OperationType.Update
            });
        }

        /// <summary>
        /// Registers an entity to be deleted as part of this unit of work.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="repository">The repository in charge of the entity.</param>
        public void RegisterDeleted(IEntity entity, IUnitOfWorkRepository repository)
        {
            this.Completed = false;
            this._operations.Enqueue(new Operation
            {
                Entity = entity,
                Repository = repository,
                Type = OperationType.Delete
            });
        }

        /// <summary>
        /// Begins the work.
        /// </summary>
        public virtual void Begin()
        {
        }

        /// <summary>
        /// Performs all operations in the queue.
        /// </summary>
        public virtual void Flush()
        {
            this.Begin();

            while (this._operations.Count > 0)
            {
                var operation = this._operations.Dequeue();
                switch (operation.Type)
                {
                    case OperationType.Insert:
                        operation.Repository.PersistNewItem(operation.Entity);
                        break;
                    case OperationType.Delete:
                        operation.Repository.PersistDeletedItem(operation.Entity);
                        break;
                    case OperationType.Update:
                        operation.Repository.PersistUpdatedItem(operation.Entity);
                        break;
                }
            }
        }

        /// <summary>
        /// Completes the Unit of Work.
        /// </summary>
        public virtual void Complete()
        {
            this.Flush();
            this.Completed = true;
        }

        /// <summary>
        /// Disposes the resources.
        /// </summary>
        protected override void DisposeResources()
        {
            // whatever hasn't been commited is lost
            // not sure we need this as we are being disposed...
            this._operations.Clear();
        }

        /// <summary>
        /// Provides a snapshot of an entity and the repository reference it belongs to.
        /// </summary>
        private sealed class Operation
        {
            /// <summary>
            /// Gets or sets the entity.
            /// </summary>
            /// <value>The entity.</value>
            public IEntity Entity { get; set; }

            /// <summary>
            /// Gets or sets the repository.
            /// </summary>
            /// <value>The repository.</value>
            public IUnitOfWorkRepository Repository { get; set; }

            /// <summary>
            /// Gets or sets the type of operation.
            /// </summary>
            /// <value>The type of operation.</value>
            public OperationType Type { get; set; }
        }
    }
}
