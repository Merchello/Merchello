namespace Merchello.Core.Persistence.UnitOfWork
{
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines the Unit Of Work-part of a repository
    /// </summary>
    internal interface IUnitOfWorkRepository
    {
        /// <summary>
        /// Registers work for inserting a new entity to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void PersistNewItem(IEntity entity);

        /// <summary>
        /// Registers work for updating an existing entity database record.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void PersistUpdatedItem(IEntity entity);

        /// <summary>
        /// Registers work for deleting an entity database record.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void PersistDeletedItem(IEntity entity);
    }
}