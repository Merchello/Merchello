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
    /// Represents the Customer Registry Service 
    /// </summary>
    public class ItemCacheService : MerchelloRepositoryService, IItemCacheService
    {
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheService"/> class.
        /// </summary>
        public ItemCacheService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ItemCacheService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ItemCacheService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheService"/> class.
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
        public ItemCacheService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheService"/> class.
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
        public ItemCacheService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion


        /// <summary>
        /// Creates a basket for a consumer with a given type
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCacheType">
        /// The item Cache Type.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        public IItemCache GetItemCacheWithKey(ICustomerBase customer, ItemCacheType itemCacheType)
        {
            return GetItemCacheWithKey(customer, itemCacheType, Guid.NewGuid());
        }

        /// <summary>
        /// Creates a basket for a consumer with a given type
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCacheType">
        /// The item Cache Type.
        /// </param>
        /// <param name="versionKey">
        /// The version Key.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        public IItemCache GetItemCacheWithKey(ICustomerBase customer, ItemCacheType itemCacheType, Guid versionKey)
        {
            Mandate.ParameterCondition(Guid.Empty != versionKey, "versionKey");

            // determine if the consumer already has a item cache of this type, if so return it.
            var itemCache = GetItemCacheByCustomer(customer, itemCacheType);
            if (itemCache != null) return itemCache;

            itemCache = new ItemCache(customer.Key, itemCacheType)
            {
                VersionKey = versionKey
            };

            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IItemCache>(itemCache), this))
            {
                // registry.WasCancelled = true;
                return itemCache;
            }

            itemCache.EntityKey = customer.Key;

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateItemCacheRepository(uow))
                {
                    repository.AddOrUpdate(itemCache);
                    uow.Commit();
                }
            }

            Created.RaiseEvent(new Events.NewEventArgs<IItemCache>(itemCache), this);

            return itemCache;
        }

        /// <summary>
        /// Saves a single <see cref="IItemCache"/> object
        /// </summary>
        /// <param name="itemCache">The <see cref="IItemCache"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IItemCache itemCache, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IItemCache>(itemCache), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateItemCacheRepository(uow))
                {
                    repository.AddOrUpdate(itemCache);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IItemCache>(itemCache), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IItemCache"/> objects.
        /// </summary>
        /// <param name="itemCaches">Collection of <see cref="ItemCache"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IItemCache> itemCaches, bool raiseEvents = true)
        {
            var basketArray = itemCaches as IItemCache[] ?? itemCaches.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IItemCache>(basketArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateItemCacheRepository(uow))
                {
                    foreach (var basket in basketArray)
                    {
                        repository.AddOrUpdate(basket);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IItemCache>(basketArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IItemCache"/> object
        /// </summary>
        /// <param name="itemCache">The <see cref="IItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IItemCache itemCache, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IItemCache>(itemCache), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateItemCacheRepository(uow))
                {
                    repository.Delete(itemCache);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IItemCache>(itemCache), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IItemCache"/> objects
        /// </summary>
        /// <param name="itemCaches">Collection of <see cref="IItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IItemCache> itemCaches, bool raiseEvents = true)
        {
            var caches = itemCaches as IItemCache[] ?? itemCaches.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IItemCache>(caches), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateItemCacheRepository(uow))
                {
                    foreach (var basket in caches)
                    {
                        repository.Delete(basket);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IItemCache>(caches), this);
        }

        /// <summary>
        /// Gets a Basket by its unique id - pk
        /// </summary>
        /// <param name="key">int Id for the Basket</param>
        /// <returns><see cref="IItemCache"/></returns>
        public IItemCache GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateItemCacheRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a list of Basket give a list of unique keys
        /// </summary>
        /// <param name="keys">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IItemCache> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateItemCacheRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Returns the customer item cache of a given type.  This method will not create an item cache if the cache does not exist.
        /// </summary>
        public IItemCache GetItemCacheByCustomer(ICustomerBase customer, ItemCacheType itemCacheType)
        {
            var typeKey = EnumTypeFieldConverter.ItemItemCache.GetTypeField(itemCacheType).TypeKey;
            return GetItemCacheByCustomer(customer, typeKey);
        }

        /// <summary>
        /// Returns a collection of item caches for the consumer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public IEnumerable<IItemCache> GetItemCacheByCustomer(ICustomerBase customer)
        {
            using (var repository = RepositoryFactory.CreateItemCacheRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IItemCache>.Builder.Where(x => x.EntityKey == customer.Key);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Returns the customer item cache of a given type. This method will not create an item cache if the cache does not exist.
        /// </summary>
        public IItemCache GetItemCacheByCustomer(ICustomerBase customer, Guid itemCacheTfKey)
        {
            using (var repository = RepositoryFactory.CreateItemCacheRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IItemCache>.Builder.Where(x => x.EntityKey == customer.Key && x.ItemCacheTfKey == itemCacheTfKey);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        public IEnumerable<IItemCache> GetAll()
        {
            using (var repository = RepositoryFactory.CreateItemCacheRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }      


        #region Event Handlers


        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IItemCacheService, Events.NewEventArgs<IItemCache>> Creating; 

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IItemCacheService, Events.NewEventArgs<IItemCache>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IItemCacheService, SaveEventArgs<IItemCache>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IItemCacheService, SaveEventArgs<IItemCache>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IItemCacheService, DeleteEventArgs<IItemCache>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IItemCacheService, DeleteEventArgs<IItemCache>> Deleted;



        #endregion




    }
}