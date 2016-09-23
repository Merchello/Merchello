namespace Merchello.Core.Persistence.UnitOfWork
{
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines the Unit Of Work-part of a repository
    /// </summary>
    internal interface IUnitOfWorkRepository
    {
        void PersistNewItem(IEntity entity);

        void PersistUpdatedItem(IEntity entity);

        void PersistDeletedItem(IEntity entity);
    }
}