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


        public void Save(ICustomerAddress address)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<ICustomerAddress> addresses)
        {
            throw new NotImplementedException();
        }

        public void Delete(ICustomerAddress address)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<ICustomerAddress> addresses)
        {
            throw new NotImplementedException();
        }

        public ICustomerAddress GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey, AddressType addressType)
        {
            throw new NotImplementedException();
        }
    }
}