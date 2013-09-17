using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Events;
using Merchello.Core.Persistence.Repositories;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Customer Service, 
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public CustomerService()
            : this(new RepositoryFactory())
        { }

        public CustomerService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public CustomerService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            
            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region ICustomerService Members

        /// <summary>
        /// Creates an <see cref="ICustomer"/> object
        /// </summary>
        /// <param name="firstName">First name of the customer</param>
        /// <param name="lastName">Last name of the customer</param>
        /// <param name="email">Email address of the customer</param>
        /// <param name="memberId">The Umbraco memberId of the customer</param>
        /// <returns></returns>
        public ICustomer CreateCustomer(string firstName, string lastName, string email, int? memberId = null)
        {
            var customer = new Customer(0, 0, null)
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    MemberId = memberId
                };

            Created.RaiseEvent(new NewEventArgs<ICustomer>(customer), this);

            return customer;
        }



        /// <summary>yg
        /// Saves a single <see cref="ICustomer"/> object
        /// </summary>
        /// <param name="customer">The <see cref="ICustomer"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(ICustomer customer, bool raiseEvents = true)
        {
            if(raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomer>(customer), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerRepository(uow))
                {
                    repository.AddOrUpdate(customer);
                    uow.Commit();
                }

                if(raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomer>(customer), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="ICustomer"/> objects.
        /// </summary>
        /// <param name="customers">Collection of <see cref="ICustomer"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ICustomer> customers, bool raiseEvents = true)
        {
            var customerArray = customers as ICustomer[] ?? customers.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomer>(customerArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerRepository(uow))
                {
                    foreach (var customer in customerArray)
                    {
                        repository.AddOrUpdate(customer);
                    }
                    uow.Commit();
                }               
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomer>(customerArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICustomer"/> object
        /// </summary>
        /// <param name="customer">The <see cref="ICustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ICustomer customer, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomer>(customer), this);

            using(new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerRepository(uow))
                {
                    repository.Delete(customer);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomer>(customer), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ICustomer"/> objects
        /// </summary>
        /// <param name="customers">Collection of <see cref="ICustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<ICustomer> customers, bool raiseEvents = true)
        {
            var customerArray = customers as ICustomer[] ?? customers.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomer>(customerArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerRepository(uow))
                {
                    foreach (var customer in customerArray)
                    {
                        repository.Delete(customer);
                    }
                    uow.Commit();                    
                }                
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomer>(customerArray), this);
        }

        /// <summary>
        /// Gets a customer by its unique id - key
        /// </summary>
        /// <param name="key">Guid key for the customer</param>
        /// <returns><see cref="ICustomer"/></returns>
        public ICustomer GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateCustomerRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a list of customer give a list of unique keys
        /// </summary>
        /// <param name="keys">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<ICustomer> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateCustomerRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets an <see cref="ICustomer"/> object by its Umbraco MemberId
        /// </summary>
        /// <param name="memberId">The Umbraco MemberId of the customer to return</param>
        /// <returns><see cref="ICustomer"/> object or null if not found</returns>
        public ICustomer GetByMemberId(int? memberId)
        {
            using (var repository = _repositoryFactory.CreateCustomerRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetByMemberId(memberId);
            }
        }

        #endregion

        public IPage<ICustomer> GetCustomerByPage(long page, long itemsPerPage)
        {
            using (var repository = _repositoryFactory.CreateCustomerRepository(_uowProvider.GetUnitOfWork()))
            {
                return ((CustomerRepository) repository).GetCustomerByPage(page, itemsPerPage);
            }
        }

        internal IEnumerable<ICustomer> GetAll()
        {
            using (var repository = _repositoryFactory.CreateCustomerRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICustomerService, DeleteEventArgs<ICustomer>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICustomerService, DeleteEventArgs<ICustomer>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICustomerService, SaveEventArgs<ICustomer>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICustomerService, SaveEventArgs<ICustomer>> Saved;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICustomerService, NewEventArgs<ICustomer>> Created;



        #endregion

    }
}