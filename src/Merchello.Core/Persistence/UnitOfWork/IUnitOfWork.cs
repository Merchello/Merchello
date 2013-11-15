using Merchello.Core.Models.EntityBase;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.UnitOfWork
{
    /// <summary>
    /// Defines a Unit Of Work
    /// </summary>
    /// <remarks>
    /// This is required due to Umbraco's IUnitOfWork dependency on Umbraco.Core.Models.EntityBase.IEntity
    /// </remarks>
    public interface IUnitOfWork
    {
        void RegisterAdded(IEntity entity, IUnitOfWorkRepository repository);
        void RegisterChanged(IEntity entity, IUnitOfWorkRepository repository);
        void RegisterRemoved(IEntity entity, IUnitOfWorkRepository repository);
        void Commit();
        object Key { get; }
    }
}