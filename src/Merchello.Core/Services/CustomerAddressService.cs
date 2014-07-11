namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Models;
    using Models.TypeFields;
    using Persistence;
    using Persistence.Querying;
    using Persistence.UnitOfWork;
    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// The customer address service.
    /// </summary>
    internal class CustomerAddressService : ICustomerAddressService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The uow provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressService"/> class.
        /// </summary>
        public CustomerAddressService()
            : this(new RepositoryFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public CustomerAddressService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
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
        public CustomerAddressService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }


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

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerAddressRepository(uow))
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
        public void Save(IEnumerable<ICustomerAddress> addresses, bool raiseEvents = true)
        {
            var addressArray = addresses as ICustomerAddress[] ?? addresses.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomerAddress>(addressArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerAddressRepository(uow))
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
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerAddressRepository(uow))
                {
                    repository.Delete(address);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerAddress>(address), this);
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
        public void Delete(IEnumerable<ICustomerAddress> addresses, bool raiseEvents = true)
        {
            var addressArray = addresses as ICustomerAddress[] ?? addresses.ToArray();
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerAddress>(addressArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerAddressRepository(uow))
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
            using (var repostitory = _repositoryFactory.CreateCustomerAddressRepository(_uowProvider.GetUnitOfWork()))
            {
                return repostitory.Get(key);
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
        public IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey)
        {
            using (var repostitory = _repositoryFactory.CreateCustomerAddressRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerAddress>.Builder.Where(x => x.CustomerKey == customerKey);

                return repostitory.GetByQuery(query);
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
            using (var repostitory = _repositoryFactory.CreateCustomerAddressRepository(_uowProvider.GetUnitOfWork()))
            {
                var typeFieldKey = EnumTypeFieldConverter.Address.GetTypeField(addressType).TypeKey;

                var query = Query<ICustomerAddress>.Builder.Where(x => x.CustomerKey == customerKey && x.AddressTypeFieldKey == typeFieldKey);

                return repostitory.GetByQuery(query);
            }
        }
    }
}