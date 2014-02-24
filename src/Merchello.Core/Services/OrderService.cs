using System;
using System.Collections.Generic;
using System.Threading;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
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

        private static readonly ReaderWriterLockSlim Locker =
            new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public OrderService()
            : this(new RepositoryFactory(), new StoreSettingService())
        {
        }

        public OrderService(RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, storeSettingService)
        {
        }

        public OrderService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _storeSettingService = storeSettingService;
        }

        /// <summary>
        /// Creates a <see cref="IOrder"/> without saving it to the database
        /// </summary>
        /// <param name="orderStatusKey">The <see cref="IOrderStatus"/> key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IOrder"/></returns>
        public IOrder CreateOrder(Guid orderStatusKey, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(orderStatusKey), "orderStatusKey");

            var order = new Order(orderStatusKey)
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

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IOrder>(order), this);

            return order;
        }

        /// <summary>
        /// Creates a <see cref="IOrder"/> and saves it to the database
        /// </summary>
        /// <param name="orderStatusKey">The <see cref="IOrderStatus"/> key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IOrder"/></returns>
        public IOrder CreateOrderWithKey(Guid orderStatusKey, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(orderStatusKey), "orderStatusKey");

            var order = new Order(orderStatusKey)
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a collection of <see cref="IOrder"/>
        /// </summary>
        /// <param name="orders">The collection of <see cref="IOrder"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IOrder> orders, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a single <see cref="IOrder"/>
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IOrder order, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a collection <see cref="IOrder"/>
        /// </summary>
        /// <param name="orders">The collection of <see cref="IOrder"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IOrder> orders, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="IOrder"/> given it's unique 'key' (Guid)
        /// </summary>
        /// <param name="key">The <see cref="IOrder"/>'s unique 'key' (Guid)</param>
        /// <returns><see cref="IOrder"/></returns>
        public IOrder GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="IOrder"/> given it's unique 'OrderNumber'
        /// </summary>
        /// <param name="orderNumber">The order number of the <see cref="IOrder"/> to be retrieved</param>
        /// <returns><see cref="IOrder"/></returns>
        public IOrder GetByOrderNumber(int orderNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets list of <see cref="IOrder"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of guid 'key' for the invoices to retrieve</param>
        /// <returns>List of <see cref="IOrder"/></returns>
        public IEnumerable<IOrder> GetByKeys(IEnumerable<Guid> keys)
        {
            throw new NotImplementedException();
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