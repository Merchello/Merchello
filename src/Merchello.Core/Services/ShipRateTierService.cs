using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the ShipRateTierService
    /// </summary>
    internal class ShipRateTierService : IShipRateTierService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        internal ShipRateTierService()
            : this(new RepositoryFactory())
        { }

        internal ShipRateTierService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        internal ShipRateTierService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Saves a single <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier">The <see cref="IShipRateTier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IShipRateTier shipRateTier, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IShipRateTier>(shipRateTier), this))
            {
                ((ShipRateTier) shipRateTier).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipRateTierRepository(uow))
                {
                    repository.AddOrUpdate(shipRateTier);
                    uow.Commit();
                }
            }

            if(raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipRateTier>(shipRateTier), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTierList">The collection of <see cref="IShipRateTier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IEnumerable<IShipRateTier> shipRateTierList, bool raiseEvents = true)
        {
            var shipRateTiersArray = shipRateTierList as IShipRateTier[] ?? shipRateTierList.ToArray();
            if(raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IShipRateTier>(shipRateTiersArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipRateTierRepository(uow))
                {
                    foreach (var shipRateTier in shipRateTiersArray)
                    {
                        repository.AddOrUpdate(shipRateTier);
                    }
                    uow.Commit();
                }
            }

            if(raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipRateTier>(shipRateTiersArray), this);
        }

        /// <summary>
        /// Deletes a <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier">The <see cref="IShipRateTier"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Delete(IShipRateTier shipRateTier, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IShipRateTier>(shipRateTier), this))
            {
                ((ShipRateTier) shipRateTier).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipRateTierRepository(uow))
                {
                    repository.Delete(shipRateTier);
                    uow.Commit();
                }
            }

            if(raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipRateTier>(shipRateTier), this);
        }

        /// <summary>
        /// Gets a list of <see cref="IShipRateTier"/> objects given a <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">Guid</param>
        /// <returns>A collection of <see cref="IShipRateTier"/></returns>
        public IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey)
        {
            using (var repository = _repositoryFactory.CreateShipRateTierRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IShipRateTier>.Builder.Where(x => x.ShipMethodKey == shipMethodKey);
                return repository.GetByQuery(query);
            }
        }

        #region Event Handlers

        ///// <summary>
        ///// Occurs after Create
        ///// </summary>
        //public static event TypedEventHandler<IShipRateTierService, Events.NewEventArgs<IShipRateTier>> Creating;


        ///// <summary>
        ///// Occurs after Create
        ///// </summary>
        //public static event TypedEventHandler<IShipRateTierService, Events.NewEventArgs<IShipRateTier>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShipRateTierService, SaveEventArgs<IShipRateTier>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShipRateTierService, SaveEventArgs<IShipRateTier>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShipRateTierService, DeleteEventArgs<IShipRateTier>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShipRateTierService, DeleteEventArgs<IShipRateTier>> Deleted;

        #endregion
    }
}