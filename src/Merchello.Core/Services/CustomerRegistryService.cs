using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Events;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Customer Registry Service 
    /// </summary>
    public class CustomerRegistryService : ICustomerRegistryService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public CustomerRegistryService()
            : this(new RepositoryFactory())
        { }

        public CustomerRegistryService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public CustomerRegistryService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IBasketService Members

        /// <summary>
        /// Creates a basket for a consumer with a given type
        /// </summary>
        public ICustomerRegistry CreateCustomerRegistry(IConsumer consumer, CustomerRegistryType customerRegistryType)
        {

            // determine if the consumer already has a registry of this type, if so return it.
            var registry = GetBasketByConsumer(consumer, customerRegistryType);
            if (registry != null) return registry;

            registry = new CustomerRegistry(consumer.Key, customerRegistryType);
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICustomerRegistry>(registry), this))
            {
                //registry.WasCancelled = true;
                return registry;
            }

            registry.ConsumerKey = consumer.Key;
            

            Created.RaiseEvent(new Events.NewEventArgs<ICustomerRegistry>(registry), this);

            return registry;
        }


      

        /// <summary>
        /// Saves a single <see cref="ICustomerRegistry"/> object
        /// </summary>
        /// <param name="customerRegistry">The <see cref="ICustomerRegistry"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(ICustomerRegistry customerRegistry, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomerRegistry>(customerRegistry), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketRepository(uow))
                {
                    repository.AddOrUpdate(customerRegistry);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomerRegistry>(customerRegistry), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="ICustomerRegistry"/> objects.
        /// </summary>
        /// <param name="customerRegistries">Collection of <see cref="CustomerRegistry"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ICustomerRegistry> customerRegistries, bool raiseEvents = true)
        {
            var basketArray = customerRegistries as ICustomerRegistry[] ?? customerRegistries.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomerRegistry>(basketArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketRepository(uow))
                {
                    foreach (var basket in basketArray)
                    {
                        repository.AddOrUpdate(basket);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomerRegistry>(basketArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICustomerRegistry"/> object
        /// </summary>
        /// <param name="customerRegistry">The <see cref="ICustomerRegistry"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ICustomerRegistry customerRegistry, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerRegistry>(customerRegistry), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketRepository(uow))
                {
                    repository.Delete(customerRegistry);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerRegistry>(customerRegistry), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ICustomerRegistry"/> objects
        /// </summary>
        /// <param name="customerRegistries">Collection of <see cref="ICustomerRegistry"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<ICustomerRegistry> customerRegistries, bool raiseEvents = true)
        {
            var basketArray = customerRegistries as ICustomerRegistry[] ?? customerRegistries.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerRegistry>(basketArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketRepository(uow))
                {
                    foreach (var basket in basketArray)
                    {
                        repository.Delete(basket);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerRegistry>(basketArray), this);
        }

        /// <summary>
        /// Gets a Basket by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Basket</param>
        /// <returns><see cref="ICustomerRegistry"/></returns>
        public ICustomerRegistry GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of Basket give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<ICustomerRegistry> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }


        /// <summary>
        /// Returns the consumer's basket of a given type
        /// </summary>
        public ICustomerRegistry GetBasketByConsumer(IConsumer consumer, CustomerRegistryType customerRegistryType)
        {
            var typeKey = EnumTypeFieldConverter.CustomerRegistry().GetTypeField(customerRegistryType).TypeKey;
            return GetBasketByConsumer(consumer, typeKey);
        }

        /// <summary>
        /// Returns the consumer's basket of a given type
        /// </summary>
        public ICustomerRegistry GetBasketByConsumer(IConsumer consumer, Guid customerRegistryBasketTfKey)
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerRegistry>.Builder.Where(x => x.ConsumerKey == consumer.Key && x.CustomerRegistryTfKey == customerRegistryBasketTfKey);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ICustomerRegistry"/> objects by teh <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public IEnumerable<ICustomerRegistry> GetBasketsByConsumer(IConsumer consumer)
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerRegistry>.Builder.Where(x => x.ConsumerKey == consumer.Key);
                return repository.GetByQuery(query);
            }
        }

        public IEnumerable<ICustomerRegistry> GetAll()
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }      

        #endregion

      


        #region Event Handlers


        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<ICustomerRegistryService, Events.NewEventArgs<ICustomerRegistry>> Creating; 

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICustomerRegistryService, Events.NewEventArgs<ICustomerRegistry>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICustomerRegistryService, SaveEventArgs<ICustomerRegistry>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICustomerRegistryService, SaveEventArgs<ICustomerRegistry>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICustomerRegistryService, DeleteEventArgs<ICustomerRegistry>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICustomerRegistryService, DeleteEventArgs<ICustomerRegistry>> Deleted;



        #endregion




    }
}