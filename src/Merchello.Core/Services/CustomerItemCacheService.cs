using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Customer Registry Service 
    /// </summary>
    public class CustomerItemCacheService : ICustomerItemCacheService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public CustomerItemCacheService()
            : this(new RepositoryFactory())
        { }

        public CustomerItemCacheService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public CustomerItemCacheService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region ICustomerItemRegisterService Members

        /// <summary>
        /// Creates a basket for a consumer with a given type
        /// </summary>
        public ICustomerItemCache GetCustomerItemCacheWithKey(ICustomerBase customer, ItemCacheType itemCacheType)
        {

            // determine if the consumer already has a item cache of this type, if so return it.
            var itemCache = GetCustomerItemCacheByCustomer(customer, itemCacheType);
            if (itemCache != null) return itemCache;

            itemCache = new CustomerItemCache(customer.Key, itemCacheType);
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICustomerItemCache>(itemCache), this))
            {
                //registry.WasCancelled = true;
                return itemCache;
            }

            itemCache.CustomerKey = customer.Key;

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(uow))
                {
                    repository.AddOrUpdate(itemCache);
                    uow.Commit();
                }
            }

            Created.RaiseEvent(new Events.NewEventArgs<ICustomerItemCache>(itemCache), this);

            return itemCache;
        }

        /// <summary>
        /// Saves a single <see cref="ICustomerItemCache"/> object
        /// </summary>
        /// <param name="customerItemCache">The <see cref="ICustomerItemCache"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(ICustomerItemCache customerItemCache, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomerItemCache>(customerItemCache), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(uow))
                {
                    repository.AddOrUpdate(customerItemCache);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomerItemCache>(customerItemCache), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="ICustomerItemCache"/> objects.
        /// </summary>
        /// <param name="customerItemCaches">Collection of <see cref="CustomerItemCache"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ICustomerItemCache> customerItemCaches, bool raiseEvents = true)
        {
            var basketArray = customerItemCaches as ICustomerItemCache[] ?? customerItemCaches.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomerItemCache>(basketArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(uow))
                {
                    foreach (var basket in basketArray)
                    {
                        repository.AddOrUpdate(basket);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomerItemCache>(basketArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICustomerItemCache"/> object
        /// </summary>
        /// <param name="customerItemCache">The <see cref="ICustomerItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ICustomerItemCache customerItemCache, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerItemCache>(customerItemCache), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(uow))
                {
                    repository.Delete(customerItemCache);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerItemCache>(customerItemCache), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ICustomerItemCache"/> objects
        /// </summary>
        /// <param name="customerItemCaches">Collection of <see cref="ICustomerItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<ICustomerItemCache> customerItemCaches, bool raiseEvents = true)
        {
            var basketArray = customerItemCaches as ICustomerItemCache[] ?? customerItemCaches.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerItemCache>(basketArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(uow))
                {
                    foreach (var basket in basketArray)
                    {
                        repository.Delete(basket);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerItemCache>(basketArray), this);
        }

        /// <summary>
        /// Gets a Basket by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Basket</param>
        /// <returns><see cref="ICustomerItemCache"/></returns>
        public ICustomerItemCache GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of Basket give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<ICustomerItemCache> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        /// <summary>
        /// Returns the customer item cache of a given type.  This method will not create an item cache if the cache does not exist.
        /// </summary>
        public ICustomerItemCache GetCustomerItemCacheByCustomer(ICustomerBase customer, ItemCacheType itemCacheType)
        {
            var typeKey = EnumTypeFieldConverter.CustomerItemItemCache.GetTypeField(itemCacheType).TypeKey;
            return GetCustomerItemCacheByCustomer(customer, typeKey);
        }

        /// <summary>
        /// Returns a collection of item caches for the consumer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public IEnumerable<ICustomerItemCache> GetCustomerItemCacheByCustomer(ICustomerBase customer)
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerItemCache>.Builder.Where(x => x.CustomerKey == customer.Key);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Returns the customer item cache of a given type. This method will not create an item cache if the cache does not exist.
        /// </summary>
        public ICustomerItemCache GetCustomerItemCacheByCustomer(ICustomerBase customer, Guid itemCacheTfKey)
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerItemCache>.Builder.Where(x => x.CustomerKey == customer.Key && x.ItemCacheTfKey == itemCacheTfKey);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

       

        public IEnumerable<ICustomerItemCache> GetAll()
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }      

        #endregion

      


        #region Event Handlers


        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<ICustomerItemCacheService, Events.NewEventArgs<ICustomerItemCache>> Creating; 

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICustomerItemCacheService, Events.NewEventArgs<ICustomerItemCache>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICustomerItemCacheService, SaveEventArgs<ICustomerItemCache>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICustomerItemCacheService, SaveEventArgs<ICustomerItemCache>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICustomerItemCacheService, DeleteEventArgs<ICustomerItemCache>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICustomerItemCacheService, DeleteEventArgs<ICustomerItemCache>> Deleted;



        #endregion




    }
}