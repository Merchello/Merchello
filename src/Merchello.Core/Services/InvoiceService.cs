using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the InvoiceService
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IStoreSettingService _storeSettingService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public InvoiceService()
            : this(new RepositoryFactory(), new StoreSettingService())
        { }

        public InvoiceService(RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, storeSettingService)
        { }

        public InvoiceService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _storeSettingService = storeSettingService;
        }

        /// <summary>
        /// Creates a <see cref="IInvoice"/> without saving it to the database
        /// </summary>
        /// <param name="invoiceStatusKey">The <see cref="IInvoiceStatus"/> key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IInvoice"/></returns>
        public IInvoice CreateInvoice(Guid invoiceStatusKey, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(Guid.Empty != invoiceStatusKey, "invoiceStatusKey");

            var invoice = new Invoice(invoiceStatusKey);

            if(raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IInvoice>(invoice), this))
            {
                invoice.WasCancelled = true;
                return invoice;
            }

            if(raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IInvoice>(invoice), this);

            return invoice;
        }

   

        /// <summary>
        /// Saves a single <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IInvoice invoice, bool raiseEvents = true)
        {
            if (!((Invoice) invoice).HasIdentity && invoice.InvoiceNumber <= 0)
            {
                // We have to generate a new 'unique' invoice number off the configurable value
                ((Invoice) invoice).InvoiceNumber = ((StoreSettingService) _storeSettingService).GetNextInvoiceNumber();
            }

            var includesStatusChange = ((Invoice) invoice).IsPropertyDirty("InvoiceStatusKey") && ((Invoice)invoice).HasIdentity == false;

            if(raiseEvents)
            {
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IInvoice>(invoice), this))
                {
                    ((Invoice) invoice).WasCancelled = true;
                    return;
                }

                if (includesStatusChange) StatusChanging.RaiseEvent(new StatusChangeEventArgs<IInvoice>(invoice), this);
             
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceRepository(uow))
                {
                    repository.AddOrUpdate(invoice);
                    uow.Commit();
                }
            }

            if (!raiseEvents) return;

            Saved.RaiseEvent(new SaveEventArgs<IInvoice>(invoice), this);
            if(includesStatusChange) StatusChanged.RaiseEvent(new StatusChangeEventArgs<IInvoice>(invoice), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoices">The collection of <see cref="IInvoice"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IInvoice> invoices, bool raiseEvents = true)
        {
            // Generate Invoice Number for new Invoices in the collection
            var invoicesArray = invoices as IInvoice[] ?? invoices.ToArray();
            var newInvoiceCount = invoicesArray.Count(x => x.InvoiceNumber <= 0 && !((Invoice) x).HasIdentity);
            if (newInvoiceCount > 0)
            {
                var lastInvoiceNumber = ((StoreSettingService)_storeSettingService).GetNextInvoiceNumber(newInvoiceCount);
                foreach (var newInvoice in invoicesArray.Where(x => x.InvoiceNumber <= 0 && !((Invoice)x).HasIdentity))
                {
                    ((Invoice)newInvoice).InvoiceNumber = lastInvoiceNumber;
                    lastInvoiceNumber = lastInvoiceNumber - 1;
                }
            }

            var existingInvoicesWithStatusChanges =
                invoicesArray.Where(x => ((Invoice) x).HasIdentity == false && ((Invoice) x).IsPropertyDirty("InvoiceStatusKey")).ToArray();

            if (raiseEvents)
            {
                Saving.RaiseEvent(new SaveEventArgs<IInvoice>(invoicesArray), this);
                if(existingInvoicesWithStatusChanges.Any()) StatusChanging.RaiseEvent(new StatusChangeEventArgs<IInvoice>(existingInvoicesWithStatusChanges), this);
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceRepository(uow))
                {
                    foreach (var invoice in invoicesArray)
                    {
                        
                        repository.AddOrUpdate(invoice);    
                    }                    
                    uow.Commit();
                }
            }

            if (raiseEvents)
            {
                Saved.RaiseEvent(new SaveEventArgs<IInvoice>(invoicesArray), this);
                if (existingInvoicesWithStatusChanges.Any()) StatusChanged.RaiseEvent(new StatusChangeEventArgs<IInvoice>(existingInvoicesWithStatusChanges), this);
            }
                       
        }

        /// <summary>
        /// Deletes a single <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IInvoice invoice, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a collection <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoices">The collection of <see cref="IInvoice"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IInvoice> invoices, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="IInvoice"/> given it's unique 'key' (Guid)
        /// </summary>
        /// <param name="key">The <see cref="IInvoice"/>'s unique 'key' (Guid)</param>
        /// <returns><see cref="IInvoice"/></returns>
        public IInvoice GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a <see cref="IInvoice"/> given it's unique 'InvoiceNumber'
        /// </summary>
        /// <param name="invoiceNumber">The invoice number of the <see cref="IInvoice"/> to be retrieved</param>
        /// <returns><see cref="IInvoice"/></returns>
        public IInvoice GetByInvoiceNumber(int invoiceNumber)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IInvoice>.Builder.Where(x => x.InvoiceNumber == invoiceNumber);

                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets list of <see cref="IInvoice"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of guid 'key' for the invoices to retrieve</param>
        /// <returns>List of <see cref="IInvoice"/></returns>
        public IEnumerable<IInvoice> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, Events.NewEventArgs<IInvoice>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, Events.NewEventArgs<IInvoice>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, SaveEventArgs<IInvoice>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, SaveEventArgs<IInvoice>> Saved;

        /// <summary>
        /// Occurs before an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, StatusChangeEventArgs<IInvoice>> StatusChanging; 

        /// <summary>
        /// Occurs after an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, StatusChangeEventArgs<IInvoice>> StatusChanged; 

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IInvoiceService, DeleteEventArgs<IInvoice>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, DeleteEventArgs<IInvoice>> Deleted;

        #endregion

    }
}