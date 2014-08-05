using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the OrderService
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IStoreSettingService _storeSettingService;
        private readonly IShipmentService _shipmentService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public OrderService()
            : this(new RepositoryFactory(), new StoreSettingService(), new ShipmentService())
        {
        }

        public OrderService(RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService, IShipmentService shipmentService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, storeSettingService, shipmentService)
        {
        }

        public OrderService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService, IShipmentService shipmentService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");
            Mandate.ParameterNotNull(shipmentService, "shipmentService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _storeSettingService = storeSettingService;
            _shipmentService = shipmentService;

        }

        /// <summary>
        /// Creates a <see cref="IOrder"/> without saving it to the database
        /// </summary>
        /// <param name="orderStatusKey">The <see cref="IOrderStatus"/> key</param>
        /// <param name="invoiceKey">The invoice key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>The <see cref="IOrder"/></returns>
        public IOrder CreateOrder(Guid orderStatusKey, Guid invoiceKey, bool raiseEvents = true)
        {
            return CreateOrder(orderStatusKey, invoiceKey, 0, raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="IOrder"/> without saving it to the database
        /// </summary>
        /// <param name="orderStatusKey">
        /// The <see cref="IOrderStatus"/> key
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key
        /// </param>
        /// <param name="orderNumber">
        /// The order Number.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOrder"/>.
        /// </returns>
        /// <remarks>
        /// Order number must be a positive integer value or zero
        /// </remarks>
        public IOrder CreateOrder(Guid orderStatusKey, Guid invoiceKey, int orderNumber, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(orderStatusKey), "orderStatusKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
            Mandate.ParameterCondition(orderNumber >= 0, "orderNumber must be greater than or equal to 0");

            var status = GetOrderStatusByKey(orderStatusKey);

            var order = new Order(status, invoiceKey)
            {
                VersionKey = Guid.NewGuid(),
                OrderNumber = orderNumber,
                OrderDate = DateTime.Now
            };

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IOrder>(order), this))
                {
                    order.WasCancelled = true;
                    return order;
                }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IOrder>(order), this);

            return order;
        }

        /// <summary>
        /// Creates a <see cref="IOrder"/> and saves it to the database
        /// </summary>
        /// <param name="orderStatusKey">The <see cref="IOrderStatus"/> key</param>
        /// <param name="invoiceKey"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IOrder"/></returns>
        public IOrder CreateOrderWithKey(Guid orderStatusKey, Guid invoiceKey, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(orderStatusKey), "orderStatusKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");

            var status = GetOrderStatusByKey(orderStatusKey);

            var order = new Order(status, invoiceKey)
            {
                VersionKey = Guid.NewGuid(),
                OrderDate = DateTime.Now
            };

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IOrder>(order), this))
                {
                    order.WasCancelled = true;
                    return order;
                }


            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOrderRepository(uow))
                {
                    repository.AddOrUpdate(order);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IOrder>(order), this);

            return order;
        }

        /// <summary>
        /// Saves a single <see cref="IOrder"/>
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IOrder order, bool raiseEvents = true)
        {
            if (!((Order)order).HasIdentity && order.OrderNumber <= 0)
            {
                // We have to generate a new 'unique' order number off the configurable value
                ((Order)order).OrderNumber = _storeSettingService.GetNextOrderNumber();
            }

            var includesStatusChange = ((Order)order).IsPropertyDirty("OrderStatus") &&
                                       ((Order)order).HasIdentity;

            if (raiseEvents)
            {
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IOrder>(order), this))
                {
                    ((Order)order).WasCancelled = true;
                    return;
                }

                if (includesStatusChange) StatusChanging.RaiseEvent(new StatusChangeEventArgs<IOrder>(order), this);

            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOrderRepository(uow))
                {
                    repository.AddOrUpdate(order);
                    uow.Commit();
                }
            }

            if (!raiseEvents) return;

            Saved.RaiseEvent(new SaveEventArgs<IOrder>(order), this);
            if (includesStatusChange) StatusChanged.RaiseEvent(new StatusChangeEventArgs<IOrder>(order), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IOrder"/>
        /// </summary>
        /// <param name="orders">The collection of <see cref="IOrder"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IOrder> orders, bool raiseEvents = true)
        {
            // Generate Order Number for new Orders in the collection
            var ordersArray = orders as IOrder[] ?? orders.ToArray();
            var newOrderCount = ordersArray.Count(x => x.OrderNumber <= 0 && !((Order)x).HasIdentity);
            if (newOrderCount > 0)
            {
                var lastOrderNumber =
                    _storeSettingService.GetNextOrderNumber(newOrderCount);
                foreach (var newOrder in ordersArray.Where(x => x.OrderNumber <= 0 && !((Order)x).HasIdentity))
                {
                    ((Order)newOrder).OrderNumber = lastOrderNumber;
                    lastOrderNumber = lastOrderNumber - 1;
                }
            }

            var existingOrdersWithStatusChanges =
                ordersArray.Where(
                    x => ((Order)x).HasIdentity == false && ((Order)x).IsPropertyDirty("OrderStatus"))
                    .ToArray();

            if (raiseEvents)
            {
                Saving.RaiseEvent(new SaveEventArgs<IOrder>(ordersArray), this);
                if (existingOrdersWithStatusChanges.Any())
                    StatusChanging.RaiseEvent(new StatusChangeEventArgs<IOrder>(existingOrdersWithStatusChanges),
                        this);
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOrderRepository(uow))
                {
                    foreach (var order in ordersArray)
                    {
                        repository.AddOrUpdate(order);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents)
            {
                Saved.RaiseEvent(new SaveEventArgs<IOrder>(ordersArray), this);
                if (existingOrdersWithStatusChanges.Any())
                    StatusChanged.RaiseEvent(new StatusChangeEventArgs<IOrder>(existingOrdersWithStatusChanges),
                        this);
            }
        }

        /// <summary>
        /// Deletes a single <see cref="IOrder"/>
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IOrder order, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IOrder>(order), this))
                {
                    ((Order)order).WasCancelled = true;
                    return;
                }

            // Delete any shipment records associated with this order
            DeleteShipments(order);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOrderRepository(uow))
                {
                    repository.Delete(order);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IOrder>(order), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IOrder"/>
        /// </summary>
        /// <param name="orders">The collection of <see cref="IOrder"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IOrder> orders, bool raiseEvents = true)
        {
            var ordersArray = orders as IOrder[] ?? orders.ToArray();
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IOrder>(ordersArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOrderRepository(uow))
                {
                    foreach (var order in ordersArray)
                    {
                        // delete the shipments
                        DeleteShipments(order);

                        repository.Delete(order);
                    }
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IOrder>(ordersArray), this);
        }

        /// <summary>
        /// Deletes <see cref="IShipment"/>s associated with the order
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/></param>
        private void DeleteShipments(IOrder order)
        {
            // Delete any shipment records associated with this order
            var shipmentKeys = order.Items.Select(x => ((IOrderLineItem)x).ShipmentKey).Where(x => x != null).Distinct().Select(x => x.Value).ToArray();

            if (!shipmentKeys.Any()) return;

            var shipments = _shipmentService.GetByKeys(shipmentKeys).ToArray();
            if(shipments.Any()) _shipmentService.Delete(shipments);
        }


        /// <summary>
        /// Gets a <see cref="IOrder"/> given it's unique 'key' (Guid)
        /// </summary>
        /// <param name="key">The <see cref="IOrder"/>'s unique 'key' (Guid)</param>
        /// <returns><see cref="IOrder"/></returns>
        public IOrder GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateOrderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a <see cref="IOrder"/> given it's unique 'OrderNumber'
        /// </summary>
        /// <param name="orderNumber">The order number of the <see cref="IOrder"/> to be retrieved</param>
        /// <returns><see cref="IOrder"/></returns>
        public IOrder GetByOrderNumber(int orderNumber)
        {
            using (var repository = _repositoryFactory.CreateOrderRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IOrder>.Builder.Where(x => x.OrderNumber == orderNumber);

                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IOrder"/> for a given <see cref="IInvoice"/> key
        /// </summary>
        /// <param name="invoiceKey">The <see cref="IInvoice"/> key</param>
        /// <returns>A collection of <see cref="IOrder"/></returns>
        public IEnumerable<IOrder> GetOrdersByInvoiceKey(Guid invoiceKey)
        {
            using (var repository = _repositoryFactory.CreateOrderRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IOrder>.Builder.Where(x => x.InvoiceKey == invoiceKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets list of <see cref="IOrder"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of guid 'key' for the invoices to retrieve</param>
        /// <returns>List of <see cref="IOrder"/></returns>
        public IEnumerable<IOrder> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateOrderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets all of the orders
        /// </summary>
        internal IEnumerable<IOrder> GetAll()
        {
            using (var repository = _repositoryFactory.CreateOrderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Gets an <see cref="IOrderStatus"/> by it's key
        /// </summary>
        /// <param name="key">The <see cref="IInvoiceStatus"/> key</param>
        /// <returns><see cref="IInvoiceStatus"/></returns>
        public IOrderStatus GetOrderStatusByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateOrderStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Returns a collection of all <see cref="IOrderStatus"/>
        /// </summary>
        public IEnumerable<IOrderStatus> GetAllOrderStatuses()
        {
            using (var repository = _repositoryFactory.CreateOrderStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IOrderService, Events.NewEventArgs<IOrder>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IOrderService, Events.NewEventArgs<IOrder>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IOrderService, SaveEventArgs<IOrder>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IOrderService, SaveEventArgs<IOrder>> Saved;

        /// <summary>
        /// Occurs before an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IOrderService, StatusChangeEventArgs<IOrder>> StatusChanging;

        /// <summary>
        /// Occurs after an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IOrderService, StatusChangeEventArgs<IOrder>> StatusChanged;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IOrderService, DeleteEventArgs<IOrder>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IOrderService, DeleteEventArgs<IOrder>> Deleted;

        #endregion


    }
}