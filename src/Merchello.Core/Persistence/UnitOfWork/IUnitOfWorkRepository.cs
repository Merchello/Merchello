using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Persistence.UnitOfWork
{
    /// <summary>
    /// Defines the Unit Of Work-part of a repository
    /// </summary>
    /// <remarks>
    /// This is required due to Umbraco's IUnitOfWorkRepository dependency on Umbraco.Core.Models.EntityBase.IEntity
    /// </remarks>
    public interface IUnitOfWorkRepository
    {
        void PersistNewItem(IEntity entity);
        void PersistUpdatedItem(IEntity entity);
        void PersistDeletedItem(IEntity entity);
    }
}