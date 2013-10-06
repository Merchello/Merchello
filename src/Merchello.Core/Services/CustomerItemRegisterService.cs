using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
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
    public class CustomerItemRegisterService : ICustomerItemRegisterService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public CustomerItemRegisterService()
            : this(new RepositoryFactory())
        { }

        public CustomerItemRegisterService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public CustomerItemRegisterService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
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
        public ICustomerItemRegister CreateCustomerItemRegister(IConsumer consumer, CustomerItemRegisterType customerItemRegisterType)
        {

            // determine if the consumer already has a registry of this type, if so return it.
            var registry = GetRegisterByConsumer(consumer, customerItemRegisterType);
            if (registry != null) return registry;

            registry = new CustomerItemRegister(consumer.Key, customerItemRegisterType);
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICustomerItemRegister>(registry), this))
            {
                //registry.WasCancelled = true;
                return registry;
            }

            registry.ConsumerKey = consumer.Key;
            

            Created.RaiseEvent(new Events.NewEventArgs<ICustomerItemRegister>(registry), this);

            return registry;
        }

        /// <summary>
        /// Saves a single <see cref="ICustomerItemRegister"/> object
        /// </summary>
        /// <param name="customerItemRegister">The <see cref="ICustomerItemRegister"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(ICustomerItemRegister customerItemRegister, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomerItemRegister>(customerItemRegister), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(uow))
                {
                    repository.AddOrUpdate(customerItemRegister);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomerItemRegister>(customerItemRegister), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="ICustomerItemRegister"/> objects.
        /// </summary>
        /// <param name="customerRegistries">Collection of <see cref="CustomerItemRegister"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ICustomerItemRegister> customerRegistries, bool raiseEvents = true)
        {
            var basketArray = customerRegistries as ICustomerItemRegister[] ?? customerRegistries.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomerItemRegister>(basketArray), this);

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

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomerItemRegister>(basketArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICustomerItemRegister"/> object
        /// </summary>
        /// <param name="customerItemRegister">The <see cref="ICustomerItemRegister"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ICustomerItemRegister customerItemRegister, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerItemRegister>(customerItemRegister), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(uow))
                {
                    repository.Delete(customerItemRegister);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerItemRegister>(customerItemRegister), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ICustomerItemRegister"/> objects
        /// </summary>
        /// <param name="customerRegistries">Collection of <see cref="ICustomerItemRegister"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<ICustomerItemRegister> customerRegistries, bool raiseEvents = true)
        {
            var basketArray = customerRegistries as ICustomerItemRegister[] ?? customerRegistries.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomerItemRegister>(basketArray), this);

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

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomerItemRegister>(basketArray), this);
        }

        /// <summary>
        /// Gets a Basket by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Basket</param>
        /// <returns><see cref="ICustomerItemRegister"/></returns>
        public ICustomerItemRegister GetById(int id)
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
        public IEnumerable<ICustomerItemRegister> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        /// <summary>
        /// Returns the consumer's basket of a given type
        /// </summary>
        public ICustomerItemRegister GetRegisterByConsumer(IConsumer consumer, CustomerItemRegisterType customerItemRegisterType)
        {
            var typeKey = EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(customerItemRegisterType).TypeKey;
            return GetRegisterByConsumer(consumer, typeKey);
        }

        /// <summary>
        /// Returns a collection of item registers for the consumer
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public IEnumerable<ICustomerItemRegister> GetRegisterByConsumer(IConsumer consumer)
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerItemRegister>.Builder.Where(x => x.ConsumerKey == consumer.Key);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Returns the consumer's basket of a given type
        /// </summary>
        public ICustomerItemRegister GetRegisterByConsumer(IConsumer consumer, Guid registerTfKey)
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerItemRegister>.Builder.Where(x => x.ConsumerKey == consumer.Key && x.RegisterTfKey == registerTfKey);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ICustomerItemRegister"/> objects by teh <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public IEnumerable<ICustomerItemRegister> GeByConsumer(IConsumer consumer)
        {
            using (var repository = _repositoryFactory.CreateCustomerItemRegisterRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomerItemRegister>.Builder.Where(x => x.ConsumerKey == consumer.Key);
                return repository.GetByQuery(query);
            }
        }

        public IEnumerable<ICustomerItemRegister> GetAll()
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
        public static event TypedEventHandler<ICustomerItemRegisterService, Events.NewEventArgs<ICustomerItemRegister>> Creating; 

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICustomerItemRegisterService, Events.NewEventArgs<ICustomerItemRegister>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICustomerItemRegisterService, SaveEventArgs<ICustomerItemRegister>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICustomerItemRegisterService, SaveEventArgs<ICustomerItemRegister>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICustomerItemRegisterService, DeleteEventArgs<ICustomerItemRegister>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICustomerItemRegisterService, DeleteEventArgs<ICustomerItemRegister>> Deleted;



        #endregion




    }
}