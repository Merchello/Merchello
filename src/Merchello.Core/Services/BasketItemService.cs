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
    /// Represents the BasketItem Service 
    /// </summary>
    public partial class BasketItemService : IBasketItemService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public BasketItemService()
            : this(new RepositoryFactory())
        { }

        public BasketItemService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public BasketItemService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IBasketItemService Members

        /// <summary>
        /// Creates an <see cref="IBasketItem"/> object
        /// </summary>
        public IBasketItem CreateBasketItem(IBasket basket, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount)
        {
            var invoiceItemType = TypeFieldProvider.InvoiceItem().GetTypeField(InvoiceItemType.Product);
            return CreateBasketItem(basket, invoiceItemType.TypeKey, sku, name, baseQuantity, unitOfMeasureMultiplier, amount);
        }

        /// <summary>
        /// Creates an <see cref="IBasketItem"/> object
        /// </summary>
        internal IBasketItem CreateBasketItem(IBasket basket, Guid invoiceItemTypeFieldKey, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount)
        {
            var basketItem = new BasketItem(basket.Id)
                {
                    InvoiceItemTypeFieldKey = invoiceItemTypeFieldKey, 
                    Sku = sku, 
                    Name = name, 
                    BaseQuantity = baseQuantity, 
                    UnitOfMeasureMultiplier = unitOfMeasureMultiplier, 
                    Amount = amount
                };
                
            Created.RaiseEvent(new NewEventArgs<IBasketItem>(basketItem), this);

            return basketItem;
        }

        /// <summary>
        /// Saves a single <see cref="IBasketItem"/> object
        /// </summary>
        /// <param name="basketItem">The <see cref="IBasketItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IBasketItem basketItem, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IBasketItem>(basketItem), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketItemRepository(uow))
                {
                    repository.AddOrUpdate(basketItem);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IBasketItem>(basketItem), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IBasketItem"/> objects.
        /// </summary>
        /// <param name="basketItemList">Collection of <see cref="BasketItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IBasketItem> basketItemList, bool raiseEvents = true)
        {
            var basketItemArray = basketItemList as IBasketItem[] ?? basketItemList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IBasketItem>(basketItemArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketItemRepository(uow))
                {
                    foreach (var basketItem in basketItemArray)
                    {
                        repository.AddOrUpdate(basketItem);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IBasketItem>(basketItemArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IBasketItem"/> object
        /// </summary>
        /// <param name="basketItem">The <see cref="IBasketItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IBasketItem basketItem, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IBasketItem>( basketItem), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketItemRepository(uow))
                {
                    repository.Delete( basketItem);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IBasketItem>( basketItem), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IBasketItem"/> objects
        /// </summary>
        /// <param name="basketItemList">Collection of <see cref="IBasketItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IBasketItem> basketItemList, bool raiseEvents = true)
        {
            var basketItemArray = basketItemList as IBasketItem[] ?? basketItemList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IBasketItem>(basketItemArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketItemRepository(uow))
                {
                    foreach (var basketItem in basketItemArray)
                    {
                        repository.Delete(basketItem);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IBasketItem>(basketItemArray), this);
        }

        /// <summary>
        /// Gets a BasketItem by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the BasketItem</param>
        /// <returns><see cref="IBasketItem"/></returns>
        public IBasketItem GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateBasketItemRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of BasketItem give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IBasketItem> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateBasketItemRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        public IEnumerable<IBasketItem> GetAll()
        {
            using (var repository = _repositoryFactory.CreateBasketItemRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IBasketItemService, DeleteEventArgs<IBasketItem>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IBasketItemService, DeleteEventArgs<IBasketItem>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IBasketItemService, SaveEventArgs<IBasketItem>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IBasketItemService, SaveEventArgs<IBasketItem>> Saved;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IBasketItemService, NewEventArgs<IBasketItem>> Created;

        #endregion


     
    }
}