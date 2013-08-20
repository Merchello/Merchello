using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Events;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Address Service, 
    /// </summary>
    public class AddressService : IAddressService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public AddressService()
            : this(new RepositoryFactory())
        { }

        public AddressService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public AddressService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IAddressService Members

        /// <summary>
        /// Creates an <see cref="IAddress"/> object
        /// </summary>
        public IAddress CreateAddress(Guid customerPk, string label, ITypeField addressType, 
            string address1, string address2, 
            string locality, string region, string postalCode, string countryCode)
        {
            var address = new Address(customerPk)
                {
                    Label = label,
                    AddressTypeFieldKey = addressType.TypeKey,
                    Address1 = address1,
                    Address2 = address2,
                    Locality = locality,
                    Region = region,
                    PostalCode = postalCode,
                    CountryCode = countryCode
                };

            Created.RaiseEvent(new NewEventArgs<IAddress>(address), this);

            return address;
        }

        /// <summary>
        /// Saves a single <see cref="IAddress"/> object
        /// </summary>
        /// <param name="address">The <see cref="IAddress"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IAddress address, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IAddress>(address), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAddressRepository(uow))
                {
                    repository.AddOrUpdate(address);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IAddress>(address), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IAddress"/> objects.
        /// </summary>
        /// <param name="addresses">Collection of <see cref="IAddress"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IAddress> addresses, bool raiseEvents = true)
        {
            var addressArray = addresses as IAddress[] ?? addresses.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IAddress>(addressArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAddressRepository(uow))
                {
                    foreach (var address in addressArray)
                    {
                        repository.AddOrUpdate(address);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IAddress>(addressArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IAddress"/> object
        /// </summary>
        /// <param name="address">The <see cref="IAddress"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IAddress address, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IAddress>(address), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAddressRepository(uow))
                {
                    repository.Delete(address);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IAddress>(address), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IAddress"/> objects
        /// </summary>
        /// <param name="addresses">Collection of <see cref="IAddress"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IAddress> addresses, bool raiseEvents = true)
        {
            var addressArray = addresses as IAddress[] ?? addresses.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IAddress>(addressArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAddressRepository(uow))
                {
                    foreach (var address in addressArray)
                    {
                        repository.Delete(address);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IAddress>(addressArray), this);
        }

        /// <summary>
        /// Gets a Address by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Address</param>
        /// <returns><see cref="IAddress"/></returns>
        public IAddress GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateAddressRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of Address give a list of unique keys
        /// </summary>
        /// <param name="keys">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IAddress> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateAddressRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        public IEnumerable<IAddress> GetAll()
        {
            using (var repository = _repositoryFactory.CreateAddressRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IAddressService, DeleteEventArgs<IAddress>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IAddressService, DeleteEventArgs<IAddress>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IAddressService, SaveEventArgs<IAddress>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IAddressService, SaveEventArgs<IAddress>> Saved;

        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IAddressService, NewEventArgs<IAddress>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IAddressService, NewEventArgs<IAddress>> Created;

        /// <summary>
        /// Occurs before Converting anonymous users to Address
        /// </summary>
        public static event TypedEventHandler<IAddressService, ConvertEventArgs<IAddress>> Converting;

        /// <summary>
        /// Occurs after Converting anonymous users to Address
        /// </summary>
        public static event TypedEventHandler<IAddressService, ConvertEventArgs<IAddress>> Converted;

        #endregion


     
    }
}