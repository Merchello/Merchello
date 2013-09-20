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
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Basket Service 
    /// </summary>
    public class BasketService : IBasketService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public BasketService()
            : this(new RepositoryFactory())
        { }

        public BasketService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public BasketService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
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
        public IBasket CreateBasket(IConsumer consumer, BasketType basketType)
        {

            // determine if the consumer already has a basket of this type, if so return it.
            var basket = GetBasketByConsumer(consumer, basketType);
            if (basket != null) return basket;
            
            basket = new Basket(basketType)
            {
                ConsumerKey = consumer.Key,
            };

            Created.RaiseEvent(new NewEventArgs<IBasket>(basket), this);

            return basket;
        }


      

        /// <summary>
        /// Saves a single <see cref="IBasket"/> object
        /// </summary>
        /// <param name="basket">The <see cref="IBasket"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IBasket basket, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IBasket>(basket), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketRepository(uow))
                {
                    repository.AddOrUpdate(basket);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IBasket>(basket), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IBasket"/> objects.
        /// </summary>
        /// <param name="basketList">Collection of <see cref="Basket"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IBasket> basketList, bool raiseEvents = true)
        {
            var basketArray = basketList as IBasket[] ?? basketList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IBasket>(basketArray), this);

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

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IBasket>(basketArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IBasket"/> object
        /// </summary>
        /// <param name="basket">The <see cref="IBasket"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IBasket basket, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IBasket>(basket), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketRepository(uow))
                {
                    repository.Delete(basket);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IBasket>(basket), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IBasket"/> objects
        /// </summary>
        /// <param name="basketList">Collection of <see cref="IBasket"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IBasket> basketList, bool raiseEvents = true)
        {
            var basketArray = basketList as IBasket[] ?? basketList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IBasket>(basketArray), this);

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

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IBasket>(basketArray), this);
        }

        /// <summary>
        /// Gets a Basket by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Basket</param>
        /// <returns><see cref="IBasket"/></returns>
        public IBasket GetById(int id)
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
        public IEnumerable<IBasket> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }


        /// <summary>
        /// Returns the consumer's basket of a given type
        /// </summary>
        public IBasket GetBasketByConsumer(IConsumer consumer, BasketType basketType)
        {
            var typeKey = EnumTypeFieldConverter.Basket().GetTypeField(basketType).TypeKey;
            return GetBasketByConsumer(consumer, typeKey);
        }

        /// <summary>
        /// Returns the consumer's basket of a given type
        /// </summary>
        public IBasket GetBasketByConsumer(IConsumer consumer, Guid basketTypeFieldKey)
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IBasket>.Builder.Where(x => x.ConsumerKey == consumer.Key && x.BasketTypeFieldKey == basketTypeFieldKey);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IBasket"/> objects by teh <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public IEnumerable<IBasket> GetBasketsByConsumer(IConsumer consumer)
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IBasket>.Builder.Where(x => x.ConsumerKey == consumer.Key);
                return repository.GetByQuery(query);
            }
        }

        public IEnumerable<IBasket> GetAll()
        {
            using (var repository = _repositoryFactory.CreateBasketRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }      

        #endregion

      


        #region Event Handlers



        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IBasketService, NewEventArgs<IBasket>> Created;



        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IBasketService, SaveEventArgs<IBasket>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IBasketService, SaveEventArgs<IBasket>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IBasketService, DeleteEventArgs<IBasket>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IBasketService, DeleteEventArgs<IBasket>> Deleted;



        #endregion




    }
}