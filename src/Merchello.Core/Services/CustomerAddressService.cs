namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;

    using Models;
    using Models.TypeFields;
    using Persistence;
    using Persistence.Querying;
    using Persistence.UnitOfWork;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The customer address service.
    /// </summary>
    internal class CustomerAddressService : MerchelloRepositoryService, ICustomerAddressService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Constructos

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressService"/> class.
        /// </summary>
        public CustomerAddressService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CustomerAddressService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CustomerAddressService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CustomerAddressService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
        /// </param>
        public CustomerAddressService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        #region Event Handlers

        /////// <summary>
        /////// Occurs after Create
        /////// </summary>
        ////public static event TypedEventHandler<ICustomerAddressService, Events.NewEventArgs<ICustomerAddress>> Creating;

        /////// <summary>
        /////// Occurs after Create
        /////// </summary>
        ////public static event TypedEventHandler<ICustomerAddressService, Events.NewEventArgs<ICustomerAddress>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICustomerAddressService, SaveEventArgs<ICustomerAddress>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICustomerAddressService, SaveEventArgs<ICustomerAddress>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICustomerAddressService, DeleteEventArgs<ICustomerAddress>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICustomerAddressService, DeleteEventArgs<ICustomerAddress>> Deleted;

        #endregion

        /// <summary>
        /// Saves a single <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(ICustomerAddress address, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<ICustomerAddress>(address), this))
                {
                    ((CustomerAddress)address).WasCancelled = true;
                    return;
                }

            var count = GetCustomerAddressCount(address.CustomerKey, address.AddressType);
            if (count == 0 || address.IsDefault)
            {
               ClearDefaultCustomerAddress(address.CustomerKey, address.AddressType);
               address.IsDefault = true;
            }
           

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCustomerAddressRepository(uow))
                {
                    repository.AddOrUpdate(address);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomerAddress>(address), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="addresses">
        /// The addresses.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <remarks>
        /// TODO - come up with a validation strategy on batch saves that protects default address settings
        /// </remarks>
        public void Save(IEnumerable<ICustomerAddress> addresses, bool raiseEvents = true)
        {
            var addressArray = addresses as ICustomerAddress[] ?? addresses.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomerAddress>(addressArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCustomerAddressRepository(uow))
                {
                    foreach (var address in addressArray)
                    {
                        repository.AddOrUpdate(address);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomerAddress>(addressArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(ICustomerAddress address, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerAddress>(address), this);
           
            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCustomerAddressRepository(uow))
                {
                    repository.Delete(address);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerAddress>(address), this);

            // If we deleted the default address and there are other addresses of this type - pick one
            if (!address.IsDefault) return;
            
            var newDefault = GetByCustomerKey(address.CustomerKey, address.AddressType).FirstOrDefault();

            if (newDefault == null) return;
            
            newDefault.IsDefault = true;
            Save(newDefault);
        }       

        /// <summary>
        /// Gets a <see cref="ICustomerAddress"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public ICustomerAddress GetByKey(Guid key)
        {
            using (var repostitory = RepositoryFactory.CreateCustomerAddressRepository(UowProvider.GetUnitOfWork()))
            {
                return repostitory.Get(key);
            }
        }

        /// <summary>
        /// Gets the default customer address of a certain type
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public ICustomerAddress GetDefaultCustomerAddress(Guid customerKey, AddressType addressType)
        {
            using (var repostitory = RepositoryFactory.CreateCustomerAddressRepository(UowProvider.GetUnitOfWork()))
            {
                var typeFieldKey = EnumTypeFieldConverter.Address.GetTypeField(addressType).TypeKey;

                var query = Query<ICustomerAddress>.Builder.Where(x => x.CustomerKey == customerKey && x.AddressTypeFieldKey == typeFieldKey && x.IsDefault == true);

                return repostitory.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ICustomerAddress"/> by the customer key
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="ICustomerAddress"/>.
        /// </returns>
        /// <remarks>
        /// RSS This method becomes a bit superfluous now that we are exposing the customer address collection on ICustomer 
        /// </remarks>
        public IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey)
        {
            using (var repostitory = RepositoryFactory.CreateCustomerAddressRepository(UowProvider.GetUnitOfWork()))
            {
                return repostitory.GetByCustomerKey(customerKey);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ICustomerAddress"/> by the customer key filtered by an <see cref="AddressType"/>
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// A collection of <see cref="ICustomerAddress"/>.
        /// </returns>
        public IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey, AddressType addressType)
        {
            using (var repostitory = RepositoryFactory.CreateCustomerAddressRepository(UowProvider.GetUnitOfWork()))
            {
                var typeFieldKey = EnumTypeFieldConverter.Address.GetTypeField(addressType).TypeKey;

                var query = Query<ICustomerAddress>.Builder.Where(x => x.CustomerKey == customerKey && x.AddressTypeFieldKey == typeFieldKey);

                return repostitory.GetByQuery(query);
            }
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The collection of all customer addresses.
        /// </returns>
        public IEnumerable<ICustomerAddress> GetAll()
        {
            using (var repostitory = RepositoryFactory.CreateCustomerAddressRepository(UowProvider.GetUnitOfWork()))
            {
                return repostitory.GetAll();
            }
        }

        /// <summary>
        /// Gets the count of addresses for a customer.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Count(Guid customerKey)
        {
            using (var repository = RepositoryFactory.CreateCustomerAddressRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetCountByCustomerKey(customerKey);
            }
        }

        /// <summary>
        /// Gets the count of all <see cref="CustomerAddress"/> for a given customer
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The count
        /// </returns>
        internal int GetCustomerAddressCount(Guid customerKey)
        {
            using (var repository = RepositoryFactory.CreateCustomerAddressRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerAddress>.Builder.Where(x => x.CustomerKey == customerKey);
                
                return repository.Count(query);
            }
        }

        /// <summary>
        /// Gets the count of all <see cref="CustomerAddress"/> for a given customer by <see cref="AddressType"/>
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="addressType">
        /// The address Type.
        /// </param>
        /// <returns>
        /// The count
        /// </returns>
        internal int GetCustomerAddressCount(Guid customerKey, AddressType addressType)
        {
            using (var repostitory = RepositoryFactory.CreateCustomerAddressRepository(UowProvider.GetUnitOfWork()))
            {
                var typeFieldKey = EnumTypeFieldConverter.Address.GetTypeField(addressType).TypeKey;

                var query = Query<ICustomerAddress>.Builder.Where(x => x.CustomerKey == customerKey && x.AddressTypeFieldKey == typeFieldKey);

                return repostitory.Count(query);
            }
        }

        /// <summary>
        /// Deletes a collection of <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="addresses">
        /// The addresses.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <remarks>
        /// TODO - come up with a validation strategy on batch saves that protects default address settings
        /// </remarks>
        internal void Delete(IEnumerable<ICustomerAddress> addresses, bool raiseEvents = true)
        {
            var addressArray = addresses as ICustomerAddress[] ?? addresses.ToArray();
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerAddress>(addressArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCustomerAddressRepository(uow))
                {
                    foreach (var address in addressArray)
                    {
                        repository.Delete(address);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerAddress>(addressArray), this);
        }

        /// <summary>
        /// The clear default customer address.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        private void ClearDefaultCustomerAddress(Guid customerKey, AddressType addressType)
        {
            // there should only ever be one of these
            var addresses = GetByCustomerKey(customerKey, addressType).Where(x => x.IsDefault);

            var customerAddresses = addresses as ICustomerAddress[] ?? addresses.ToArray();
            if (!customerAddresses.Any()) return;

            foreach (var address in customerAddresses)
            {
                address.IsDefault = false;
                Save(address);
            }            
        }
    }
}