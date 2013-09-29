using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the BasketItem Service 
    /// </summary>
    public class BasketItemService : IBasketItemService
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
        /// Creates an <see cref="IPurchaseLineItem"/> object
        /// </summary>
        public IPurchaseLineItem CreateBasketItem(ICustomerRegistry customerRegistry, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount)
        {
            var invoiceItemType = EnumTypeFieldConverter.InvoiceItem().GetTypeField(InvoiceItemType.Product);
            return CreateBasketItem(customerRegistry, invoiceItemType.TypeKey, sku, name, baseQuantity, unitOfMeasureMultiplier, amount);
        }

        /// <summary>
        /// Creates an <see cref="IPurchaseLineItem"/> object
        /// </summary>
        internal IPurchaseLineItem CreateBasketItem(ICustomerRegistry customerRegistry, Guid invoiceItemTypeFieldKey, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount)
        {
            var basketItem = new PurchaseLineItemContainer(customerRegistry.Id)
                {
                    LineItemTfKey = invoiceItemTypeFieldKey, 
                    Sku = sku, 
                    Name = name, 
                    BaseQuantity = baseQuantity, 
                    UnitOfMeasureMultiplier = unitOfMeasureMultiplier, 
                    Amount = amount
                };
                
            Created.RaiseEvent(new Events.NewEventArgs<IPurchaseLineItem>(basketItem), this);

            return basketItem;
        }

        /// <summary>
        /// Saves a single <see cref="IPurchaseLineItem"/> object
        /// </summary>
        /// <param name="purchaseLineItem">The <see cref="IPurchaseLineItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IPurchaseLineItem purchaseLineItem, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IPurchaseLineItem>(purchaseLineItem), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketItemRepository(uow))
                {
                    repository.AddOrUpdate(purchaseLineItem);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IPurchaseLineItem>(purchaseLineItem), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IPurchaseLineItem"/> objects.
        /// </summary>
        /// <param name="basketItemList">Collection of <see cref="PurchaseLineItemContainer"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IPurchaseLineItem> basketItemList, bool raiseEvents = true)
        {
            var basketItemArray = basketItemList as IPurchaseLineItem[] ?? basketItemList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IPurchaseLineItem>(basketItemArray), this);

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

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IPurchaseLineItem>(basketItemArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IPurchaseLineItem"/> object
        /// </summary>
        /// <param name="purchaseLineItem">The <see cref="IPurchaseLineItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IPurchaseLineItem purchaseLineItem, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IPurchaseLineItem>( purchaseLineItem), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateBasketItemRepository(uow))
                {
                    repository.Delete( purchaseLineItem);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IPurchaseLineItem>( purchaseLineItem), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IPurchaseLineItem"/> objects
        /// </summary>
        /// <param name="basketItemList">Collection of <see cref="IPurchaseLineItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IPurchaseLineItem> basketItemList, bool raiseEvents = true)
        {
            var basketItemArray = basketItemList as IPurchaseLineItem[] ?? basketItemList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IPurchaseLineItem>(basketItemArray), this);

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

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IPurchaseLineItem>(basketItemArray), this);
        }

        /// <summary>
        /// Gets a BasketItem by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the BasketItem</param>
        /// <returns><see cref="IPurchaseLineItem"/></returns>
        public IPurchaseLineItem GetById(int id)
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
        public IEnumerable<IPurchaseLineItem> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateBasketItemRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        public IEnumerable<IPurchaseLineItem> GetAll()
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
        public static event TypedEventHandler<IBasketItemService, DeleteEventArgs<IPurchaseLineItem>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IBasketItemService, DeleteEventArgs<IPurchaseLineItem>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IBasketItemService, SaveEventArgs<IPurchaseLineItem>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IBasketItemService, SaveEventArgs<IPurchaseLineItem>> Saved;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IBasketItemService, Events.NewEventArgs<IPurchaseLineItem>> Created;

        #endregion


     
    }
}