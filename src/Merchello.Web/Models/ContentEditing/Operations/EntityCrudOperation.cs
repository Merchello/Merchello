namespace Merchello.Web.Models.ContentEditing.Operations
{
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence;

    /// <summary>
    /// The entity crud operation.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity
    /// </typeparam>
    internal class EntityCrudOperation<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCrudOperation{TEntity}"/> class.
        /// </summary>
        /// <param name="transType">
        /// The trans type.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public EntityCrudOperation(TransactionType transType, TEntity entity)
        {
            this.TransactionType = transType;
            this.Entity = entity;
        }

        /// <summary>
        /// Gets the transaction type.
        /// </summary>
        public TransactionType TransactionType { get; private set; }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public TEntity Entity { get; private set; }
    }
}