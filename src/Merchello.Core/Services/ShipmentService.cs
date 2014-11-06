namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Strategies.Packaging;

    using Models;
    using Persistence;
    using Persistence.Querying;
    using Persistence.UnitOfWork;
    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents the ShipmentService
    /// </summary>
    public class ShipmentService : IShipmentService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The Unit of Work provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentService"/> class.
        /// </summary>
        public ShipmentService()
            : this(new RepositoryFactory())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public ShipmentService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, new StoreSettingService(repositoryFactory))
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="storeSettingService">
        /// The store Setting Service.
        /// </param>
        public ShipmentService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _storeSettingService = storeSettingService;
        }


        #region Event Handlers

        public static event TypedEventHandler<IShipmentService, Events.NewEventArgs<IShipment>> Creating;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShipmentService, SaveEventArgs<IShipment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShipmentService, SaveEventArgs<IShipment>> Saved;

        /// <summary>
        /// Occurs before an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IShipmentService, StatusChangeEventArgs<IShipment>> StatusChanging;

        /// <summary>
        /// Occurs after an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IShipmentService, StatusChangeEventArgs<IShipment>> StatusChanged;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShipmentService, DeleteEventArgs<IShipment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShipmentService, DeleteEventArgs<IShipment>> Deleted;

        /// <summary>
        /// Special event that fires when an order record is updated
        /// </summary>
        internal static event TypedEventHandler<IShipmentService, SaveEventArgs<IOrder>> UpdatedOrder; 

        #endregion

        #region Shipment

        /// <summary>
        /// Creates a <see cref="IShipment"/> without persisting it to the database.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment status.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        public IShipment CreateShipment(IShipmentStatus shipmentStatus, bool raiseEvents = true)
        {
            return CreateShipment(shipmentStatus, new Address(), new Address(), new LineItemCollection());
        }

        /// <summary>
        /// Creates a <see cref="IShipment"/> without persisting it to the database.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment status.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        public IShipment CreateShipment(IShipmentStatus shipmentStatus, IAddress origin, IAddress destination, bool raiseEvents = true)
        {
            return CreateShipment(shipmentStatus, origin, destination, new LineItemCollection());
        }

        /// <summary>
        /// Creates a <see cref="IShipment"/> without persisting it to the database.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment status.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        public IShipment CreateShipment(IShipmentStatus shipmentStatus, IAddress origin, IAddress destination, LineItemCollection items, bool raiseEvents = true)
        {
            Mandate.ParameterNotNull(shipmentStatus, "shipmentStatus");
            Mandate.ParameterNotNull(origin, "origin");
            Mandate.ParameterNotNull(destination, "destination");
            Mandate.ParameterNotNull(items, "items");

            // Use the visitor to filter out and validate shippable line items
            var visitor = new ShippableProductVisitor();
            items.Accept(visitor);

            var lineItemCollection = new LineItemCollection();

            foreach (var item in visitor.ShippableItems)
            {
                lineItemCollection.Add(item);
            }

            var shipment = new Shipment(shipmentStatus, origin, destination, lineItemCollection);

            if (!raiseEvents)
            {
                return shipment;
            }

            Creating.RaiseEvent(new Events.NewEventArgs<IShipment>(shipment), this);
            return shipment;
        }

        /// <summary>
        /// Saves a single <see cref="IShipment"/> object
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IShipment shipment, bool raiseEvents = true)
        {
            if (!((Shipment)shipment).HasIdentity && shipment.ShipmentNumber <= 0)
            {
                // We have to generate a new 'unique' invoice number off the configurable value
                ((Shipment)shipment).ShipmentNumber = _storeSettingService.GetNextShipmentNumber();
            }

            var includesStatusChange = ((Shipment)shipment).IsPropertyDirty("ShipmentStatusKey") &&
                                       ((Shipment)shipment).HasIdentity == false;

            if (raiseEvents)
            {
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IShipment>(shipment), this))
                {
                    ((Shipment)shipment).WasCancelled = true;
                    return;
                }

                if (includesStatusChange) StatusChanging.RaiseEvent(new StatusChangeEventArgs<IShipment>(shipment), this);
            }


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

            if (!raiseEvents) return;
            
            Saved.RaiseEvent(new SaveEventArgs<IShipment>(shipment), this);
            if (includesStatusChange) StatusChanged.RaiseEvent(new StatusChangeEventArgs<IShipment>(shipment), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipment"/> objects
        /// </summary>
        /// <param name="shipmentList">Collection of <see cref="IShipment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            var shipmentsArray = shipmentList as IShipment[] ?? shipmentList.ToArray();

            var newShipmentCount = shipmentsArray.Count(x => x.ShipmentNumber <= 0 && !((Shipment)x).HasIdentity);


            if (newShipmentCount > 0)
            {
                var lastShipmentumber =
                    _storeSettingService.GetNextShipmentNumber(newShipmentCount);
                foreach (var newShipment in shipmentsArray.Where(x => x.ShipmentNumber <= 0 && !((Shipment)x).HasIdentity))
                {
                    ((Shipment)newShipment).ShipmentNumber = lastShipmentumber;
                    lastShipmentumber = lastShipmentumber - 1;
                }
            }

            var existingShipmentsWithStatusChanges =
                shipmentsArray.Where(
                    x => ((Shipment)x).HasIdentity == false && ((Shipment)x).IsPropertyDirty("ShipmentStatusKey"))
                    .ToArray();

            if (raiseEvents)
            {
                Saving.RaiseEvent(new SaveEventArgs<IShipment>(shipmentsArray), this);
                if (existingShipmentsWithStatusChanges.Any())
                    StatusChanging.RaiseEvent(
                        new StatusChangeEventArgs<IShipment>(existingShipmentsWithStatusChanges),
                        this);
            }

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

            if (!raiseEvents) return;
            Saved.RaiseEvent(new SaveEventArgs<IShipment>(shipmentsArray), this);
            if (existingShipmentsWithStatusChanges.Any())
                StatusChanged.RaiseEvent(new StatusChangeEventArgs<IShipment>(existingShipmentsWithStatusChanges), this);
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
                    UpdateOrderLineItemShipmentKeys(shipment);
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
                        UpdateOrderLineItemShipmentKeys(shipment);
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

        /// <summary>
        /// Gets a collection of <see cref="IShipment"/> give an order key
        /// </summary>
        /// <param name="orderKey">
        /// The order Key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IShipment}"/>.
        /// </returns>
        public IEnumerable<IShipment> GetShipmentsByOrderKey(Guid orderKey)
        {
            var items = Enumerable.Empty<IOrderLineItem>();

            using (var repository = _repositoryFactory.CreateOrderLineItemRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IOrderLineItem>.Builder.Where(x => x.ContainerKey == orderKey && x.ShipmentKey != Guid.Empty);

                items = repository.GetByQuery(query);
            }

            var orderLineItems = items as IOrderLineItem[] ?? items.ToArray();
            if (orderLineItems.Any())
            {
                var keys = orderLineItems.Where(x => x.ShipmentKey != null).Select(x => x.ShipmentKey.Value).ToArray();
                using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
                {
                    return repository.GetAll(keys);
                }
            }

            return Enumerable.Empty<IShipment>();
        }


        /// <summary>
        /// Gets an <see cref="IShipmentStatus"/> by it's key
        /// </summary>
        /// <param name="key">The <see cref="IShipmentStatus"/> key</param>
        /// <returns><see cref="IShipmentStatus"/></returns>
        public IShipmentStatus GetShipmentStatusByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateShipmentStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Returns a collection of all <see cref="IShipmentStatus"/>
        /// </summary>
        /// <returns>
        /// The collection of <see cref="IShipmentStatus"/>.
        /// </returns>
        public IEnumerable<IShipmentStatus> GetAllShipmentStatuses()
        {
            using (var repository = _repositoryFactory.CreateShipmentStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll().OrderBy(x => x.SortOrder);
            }
        }

        /// <summary>
        /// Gets all <see cref="IShipment"/>
        /// </summary>
        /// <returns>A collection of <see cref="IShipment"/></returns>
        internal IEnumerable<IShipment> GetAll()
        {
            using (var repository = _repositoryFactory.CreateShipmentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        #endregion

        /// <summary>
        /// Updates any order line items when a shipment is deleted to null
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        private void UpdateOrderLineItemShipmentKeys(IShipment shipment)
        {
            using (var repository = _repositoryFactory.CreateOrderRepository(_uowProvider.GetUnitOfWork()))
            {
                // there really should only ever be one of these
                var orderKeys = shipment.Items.Select(x => ((OrderLineItem) x).ContainerKey).Distinct();

                foreach (var orderKey in orderKeys)
                {
                    var order = repository.Get(orderKey);

                    if (order != null)
                    {
                        var items = order.Items.Where(x => ((OrderLineItem)x).ShipmentKey == shipment.Key);

                        foreach (var item in items)
                        {
                            ((OrderLineItem)item).ShipmentKey = null;
                        }

                        repository.AddOrUpdate(order);
                        UpdatedOrder.RaiseEvent(new SaveEventArgs<IOrder>(order), this);
                    }
                }
            }
        }
    }
}