using System;
using System.Collections.Generic;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.UnitOfWork;
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


        public void Save(IShipment shipment, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IShipment shipment, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public IShipment GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipment> GetShipmentsForShipMethod(Guid shipMethodKey)
        {
            throw new NotImplementedException();
        }



        public IEnumerable<IShipment> GetByKeys(IEnumerable<Guid> keys)
        {
            throw new NotImplementedException();
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