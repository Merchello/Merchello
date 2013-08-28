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
    /// Represents the InvoiceItem Service 
    /// </summary>
    internal class InvoiceItemService : IInvoiceItemService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public InvoiceItemService()
            : this(new RepositoryFactory())
        { }

        public InvoiceItemService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public InvoiceItemService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IInvoiceItemService Members

        /// <summary>
        /// Creates an <see cref="IInvoiceItem"/> object
        /// </summary>
        public IInvoiceItem CreateInvoiceItem(IInvoice invoice, InvoiceItemType invoiceItemType, string sku, string name, int baseQuantity, int unitOfMeasure, decimal amount, int? parentId = null)
        {
            var invoiceItemTypeFieldKey = TypeFieldProvider.InvoiceItem().GetTypeField(invoiceItemType).TypeKey;
            return CreateInvoiceItem(invoice, invoiceItemTypeFieldKey, sku, name, baseQuantity, unitOfMeasure, amount, false, parentId);
        }

        /// <summary>
        /// Creates an <see cref="IInvoiceItem"/> object
        /// </summary>
        internal IInvoiceItem CreateInvoiceItem(IInvoice invoice, Guid invoiceItemTypeFieldKey, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount, bool exported, int? parentId = null)
        {
            var invoiceItem = new InvoiceItem(invoice.Id, invoiceItemTypeFieldKey, parentId)
                {
                    InvoiceItemTypeFieldKey = invoiceItemTypeFieldKey, 
                    Sku = sku, 
                    Name = name, 
                    BaseQuantity = baseQuantity, 
                    UnitOfMeasureMultiplier = unitOfMeasureMultiplier, 
                    Amount = amount, 
                    Exported = exported
                };
                
            Created.RaiseEvent(new NewEventArgs<IInvoiceItem>(invoiceItem), this);

            return invoiceItem;
        }

        /// <summary>
        /// Saves a single <see cref="IInvoiceItem"/> object
        /// </summary>
        /// <param name="invoiceItem">The <see cref="IInvoiceItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IInvoiceItem invoiceItem, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IInvoiceItem>(invoiceItem), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceItemRepository(uow))
                {
                    repository.AddOrUpdate(invoiceItem);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IInvoiceItem>(invoiceItem), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IInvoiceItem"/> objects.
        /// </summary>
        /// <param name="invoiceItemList">Collection of <see cref="InvoiceItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IInvoiceItem> invoiceItemList, bool raiseEvents = true)
        {
            var invoiceItemArray = invoiceItemList as IInvoiceItem[] ?? invoiceItemList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IInvoiceItem>(invoiceItemArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceItemRepository(uow))
                {
                    foreach (var invoiceItem in invoiceItemArray)
                    {
                        repository.AddOrUpdate(invoiceItem);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IInvoiceItem>(invoiceItemArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IInvoiceItem"/> object
        /// </summary>
        /// <param name="invoiceItem">The <see cref="IInvoiceItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IInvoiceItem invoiceItem, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IInvoiceItem>( invoiceItem), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceItemRepository(uow))
                {
                    repository.Delete( invoiceItem);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IInvoiceItem>( invoiceItem), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IInvoiceItem"/> objects
        /// </summary>
        /// <param name="invoiceItemList">Collection of <see cref="IInvoiceItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IInvoiceItem> invoiceItemList, bool raiseEvents = true)
        {
            var invoiceItemArray = invoiceItemList as IInvoiceItem[] ?? invoiceItemList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IInvoiceItem>(invoiceItemArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceItemRepository(uow))
                {
                    foreach (var invoiceItem in invoiceItemArray)
                    {
                        repository.Delete(invoiceItem);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IInvoiceItem>(invoiceItemArray), this);
        }

        /// <summary>
        /// Gets a InvoiceItem by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the InvoiceItem</param>
        /// <returns><see cref="IInvoiceItem"/></returns>
        public IInvoiceItem GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateInvoiceItemRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of InvoiceItem give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IInvoiceItem> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateInvoiceItemRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        public IEnumerable<IInvoiceItem> GetAll()
        {
            using (var repository = _repositoryFactory.CreateInvoiceItemRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IInvoiceItemService, DeleteEventArgs<IInvoiceItem>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IInvoiceItemService, DeleteEventArgs<IInvoiceItem>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IInvoiceItemService, SaveEventArgs<IInvoiceItem>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IInvoiceItemService, SaveEventArgs<IInvoiceItem>> Saved;

        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IInvoiceItemService, NewEventArgs<IInvoiceItem>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IInvoiceItemService, NewEventArgs<IInvoiceItem>> Created;

        #endregion


     
    }
}