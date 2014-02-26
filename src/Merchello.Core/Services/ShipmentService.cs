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

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the ShipmentService
    /// </summary>
    public class ShipmentService : IShipmentService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IStoreSettingService _storeSettingService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public ShipmentService()
            : this(new RepositoryFactory())
        { }

        public ShipmentService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public ShipmentService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }


        #region Shipment


        /// <summary>
        /// Saves a single <see cref="IShipment"/> object
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IShipment shipment, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IShipment>(shipment), this))
                {
                    ((Shipment)shipment).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipmentRepository(uow))
                {
                    repository.AddOrUpdate(shipment);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipment>(shipment), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipment"/> objects
        /// </summary>
        /// <param name="shipmentList">Collection of <see cref="IShipment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            var shipmentsArray = shipmentList as IShipment[] ?? shipmentList.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IShipment>(shipmentsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipmentRepository(uow))
                {
                    foreach (var shipment in shipmentsArray)
                    {
                        repository.AddOrUpdate(shipment);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipment>(shipmentsArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IShipment"/> object
        /// </summary>
        /// <param name="shipment"><see cref="IShipment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IShipment shipment, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IShipment>(shipment), this))
                {
                    ((Shipment)shipment).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipmentRepository(uow))
                {
                    repository.Delete(shipment);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipment>(shipment), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IShipment"/> objects
        /// </summary>
        /// <param name="shipmentList">Collection of <see cref="IShipment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            var shipmentsArray = shipmentList as IShipment[] ?? shipmentList.ToArray();

            if (raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<IShipment>(shipmentsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipmentRepository(uow))
                {
                    foreach (var shipment in shipmentsArray)
                    {
                        repository.Delete(shipment);    
                    }                    
                }
                uow.Commit();
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipment>(shipmentsArray), this);
        }

        /// <summary>
        /// Gets an <see cref="IShipment"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="key">Guid pk of the Shipment to retrieve</param>
        /// <returns><see cref="IShipment"/></returns>
        public IShipment GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IShipment"/> object given a ship method Key
        /// </summary>
        /// <param name="shipMethodKey">The pk of the shipMethod</param>
        /// <returns>A collection of <see cref="IShipment"/></returns>
        public IEnumerable<IShipment> GetShipmentsByShipMethodKey(Guid shipMethodKey)
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IShipment>.Builder.Where(x => x.ShipMethodKey == shipMethodKey);

                return repository.GetByQuery(query);
            }
        }



        /// <summary>
        /// Gets list of <see cref="IShipment"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid keys for Shipment objects to retrieve</param>
        /// <returns>List of <see cref="IShipment"/></returns>
        public IEnumerable<IShipment> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        #endregion



        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShipmentService, Events.NewEventArgs<IShipment>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShipmentService, Events.NewEventArgs<IShipment>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShipmentService, SaveEventArgs<IShipment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShipmentService, SaveEventArgs<IShipment>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShipmentService, DeleteEventArgs<IShipment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShipmentService, DeleteEventArgs<IShipment>> Deleted;

        #endregion
     
    }
}