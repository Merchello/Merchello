using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.ConversionStrategies;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the anonymous customer service
    /// </summary>
    internal class AnonymousCustomerService : IAnonymousCustomerService
    {

        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly ICustomerService _customerService;
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public AnonymousCustomerService(ICustomerService customerService)
            : this(new RepositoryFactory(), customerService)
        { }

        public AnonymousCustomerService(RepositoryFactory repositoryFactory, ICustomerService customerService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, customerService)
        { }

        public AnonymousCustomerService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ICustomerService customerService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(customerService, "customerService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _customerService = customerService;
        }


        #region Overrides IAnonymousCustomerService
        

        /// <summary>
        /// Creates an <see cref="IAnonymousCustomer"/> object
        /// </summary>
        public IAnonymousCustomer CreateAnonymousCustomer(bool raiseEvents = true)
        {
            var anonymous = new AnonymousCustomer(DateTime.Now);

            Created.RaiseEvent(new Events.NewEventArgs<IAnonymousCustomer>(anonymous), this);

            return anonymous;
        }


        /// <summary>
        /// Saves a single <see cref="IAnonymousCustomer"/> object
        /// </summary>
        public void Save(IAnonymousCustomer anonymous, bool raiseEvents = true)
        {
            if(raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IAnonymousCustomer>(anonymous), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    repository.AddOrUpdate(anonymous);
                    uow.Commit();
                }                
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IAnonymousCustomer>(anonymous), this);
        }


        /// <summary>
        /// Deletes a single <see cref="IAnonymousCustomer"/> object
        /// </summary>        
        public void Delete(IAnonymousCustomer anonymous, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IAnonymousCustomer>(anonymous), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    repository.Delete(anonymous);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IAnonymousCustomer>(anonymous), this);
        }


        /// <summary>
        /// Deletes a collection <see cref="IAnonymousCustomer"/> objects
        /// </summary>
        /// <param name="anonymousList">Collection of <see cref="IAnonymousCustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IAnonymousCustomer> anonymousList, bool raiseEvents = true)
        {
            var anonymousArray = anonymousList as IAnonymousCustomer[] ?? anonymousList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IAnonymousCustomer>(anonymousArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    foreach (var anonymous in anonymousArray)
                    {
                        repository.Delete(anonymous);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IAnonymousCustomer>(anonymousArray), this);
        }


        /// <summary>
        /// Gets a Anonymous by its unique id - pk
        /// </summary>
        /// <param name="key">Guid key for the <see cref="IAnonymousCustomer"/></param>
        /// <returns><see cref="IAnonymousCustomer"/></returns>
        public IAnonymousCustomer GetByKey(System.Guid key)
        {
            using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IAnonymousCustomer"/> given a list of unique keys
        /// </summary>
        /// <param name="keys">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IAnonymousCustomer> GetByKeys(IEnumerable<System.Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateAnonymousCustomerRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }


        /// <summary>
        /// Converts an anonymous customer into a customer
        /// </summary>
        /// <param name="anonymous">The <see cref="IAnonymousCustomer"/> object to a <see cref="ICustomer"/> obect</param>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="email">The email address of the customer</param>
        /// <returns><see cref="ICustomer"/></returns>
        public ICustomer ConvertToCustomer(IAnonymousCustomer anonymous, string firstName, string lastName, string email)
        {
            // assert there is not already a customer with this key
            var customer = _customerService.GetByKey(anonymous.Key);
            if (customer == null)
            {
                var strategy = new AnonymousCustomerToNewCustomerStrategy(anonymous, firstName, lastName, email, _customerService);
                return ConvertToCustomer(anonymous, strategy);
            }
            //if (customer.MemberId != null)
            //{
            //    var strategy = new AnonymousCustomerToExistingCustomerStrategy(anonymous, (int)customer.MemberId, _customerService);
            //    return ConvertToCustomer(anonymous, strategy);
            //}
            throw new InvalidOperationException("Cannot convert anonymous user.");
        }


        /// <summary>
        /// Converts an anonymous customer into a customer
        /// </summary>
        /// <param name="anonymous">The <see cref="IAnonymousCustomer"/> object to a <see cref="ICustomer"/> obect</param>
        
        /// <param name="strategy">The strategy to use when converting the anonymous customer to a customer</param>
        /// <returns><see cref="ICustomer"/></returns>
        internal ICustomer ConvertToCustomer(IAnonymousCustomer anonymous, IAnonymousCustomerConversionStrategy strategy)
        {
            Mandate.ParameterNotNull(strategy, "strategy");

            var customer = strategy.ConvertToCustomer();
            Delete(anonymous);
            return customer;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IAnonymousCustomerService, DeleteEventArgs<IAnonymousCustomer>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, DeleteEventArgs<IAnonymousCustomer>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, SaveEventArgs<IAnonymousCustomer>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, SaveEventArgs<IAnonymousCustomer>> Saved;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, Events.NewEventArgs<IAnonymousCustomer>> Created;


        #endregion

    }
}