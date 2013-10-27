using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Invoice Service 
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public InvoiceService()
            : this(new RepositoryFactory())
        { }

        public InvoiceService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }


        public InvoiceService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IInvoiceService Members

        /// <summary>
        /// Creates an <see cref="IInvoice"/> object
        /// </summary>
        public IInvoice CreateInvoice(ICustomer customer, ICustomerAddress customerAddress, IInvoiceStatus invoiceStatus, string invoiceNumber)
        {
            var invoice = new Invoice(customer, customerAddress, invoiceStatus, 0)
            {
                InvoiceNumber =  invoiceNumber,
                InvoiceDate = DateTime.Now,
                Exported = false,
                Paid = false
            };

            Created.RaiseEvent(new Events.NewEventArgs<IInvoice>(invoice), this);
            return invoice;
        }


        /// <summary>
        /// Creates an <see cref="IInvoice"/> object
        /// </summary>
        public IInvoice CreateInvoice(ICustomer customer, IInvoiceStatus invoiceStatus, string invoiceNumber, string billToName, string billToAddress1, string billToAddress2, string billToLocality, string billToRegion, string billToPostalCode, string billToCountryCode, string billToEmail, string billToPhone, string billToCompany)
        {
            var invoice = new Invoice(customer, invoiceStatus, 0)
                {
                    InvoiceNumber = invoiceNumber, 
                    InvoiceDate = DateTime.Now, 
                    BillToName = billToName, 
                    BillToAddress1 = billToAddress1, 
                    BillToAddress2 = billToAddress2, 
                    BillToLocality = billToLocality, 
                    BillToRegion = billToRegion, 
                    BillToPostalCode = billToPostalCode, 
                    BillToCountryCode = billToCountryCode, 
                    BillToEmail = billToEmail, 
                    BillToPhone = billToPhone, 
                    BillToCompany = billToCompany, 
                    Exported = false, 
                    Paid = false
                };
                
            Created.RaiseEvent(new Events.NewEventArgs<IInvoice>(invoice), this);

            return invoice;
        }

        /// <summary>
        /// Saves a single <see cref="IInvoice"/> object
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IInvoice invoice, bool raiseEvents = true)
        {
            if (raiseEvents)
            {
                Saving.RaiseEvent(new SaveEventArgs<IInvoice>(invoice), this);
                if (invoice.IsPropertyDirty("InvoiceStatusId")) StatusChanging.RaiseEvent(new StatusChangeEventArgs<IInvoice>(invoice), this);
            }
           
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceRepository(uow))
                {
                    repository.AddOrUpdate(invoice);
                    uow.Commit();
                }

                if (raiseEvents)
                {
                    Saved.RaiseEvent(new SaveEventArgs<IInvoice>(invoice), this);
                    if (invoice.IsPropertyDirty("InvoiceStatusId")) StatusChanged.RaiseEvent(new StatusChangeEventArgs<IInvoice>(invoice), this);
                }
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IInvoice"/> objects.
        /// </summary>
        /// <param name="invoiceList">Collection of <see cref="Invoice"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IInvoice> invoiceList, bool raiseEvents = true)
        {
            var invoiceArray = invoiceList as IInvoice[] ?? invoiceList.ToArray();

            var statusChanged = invoiceArray.Where(x => x.IsPropertyDirty("InvoiceStatusId"));
            var statusChangedArray = statusChanged as IInvoice[] ?? statusChanged.ToArray();

            if (raiseEvents)
            {
                Saving.RaiseEvent(new SaveEventArgs<IInvoice>(invoiceArray), this);
                if (statusChangedArray.Any()) StatusChanging.RaiseEvent(new StatusChangeEventArgs<IInvoice>(statusChangedArray), this);
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceRepository(uow))
                {
                    foreach (var invoice in invoiceArray)
                    {
                        repository.AddOrUpdate(invoice);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents)
            {
                Saved.RaiseEvent(new SaveEventArgs<IInvoice>(invoiceArray), this);
                if (statusChangedArray.Any()) StatusChanged.RaiseEvent(new StatusChangeEventArgs<IInvoice>(statusChangedArray), this);
            }
        }

        /// <summary>
        /// Deletes a single <see cref="IInvoice"/> object
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IInvoice invoice, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IInvoice>( invoice), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceRepository(uow))
                {
                    repository.Delete( invoice);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IInvoice>( invoice), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IInvoice"/> objects
        /// </summary>
        /// <param name="invoiceList">Collection of <see cref="IInvoice"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IInvoice> invoiceList, bool raiseEvents = true)
        {
            var invoiceArray = invoiceList as IInvoice[] ?? invoiceList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IInvoice>(invoiceArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceRepository(uow))
                {
                    foreach (var invoice in invoiceArray)
                    {
                        repository.Delete(invoice);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IInvoice>(invoiceArray), this);
        }

        /// <summary>
        /// Gets a Invoice by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Invoice</param>
        /// <returns><see cref="IInvoice"/></returns>
        public IInvoice GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of Invoice give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IInvoice> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IInvoice"/> objects give a customer id
        /// </summary>
        /// <param name="id">Unique customer id</param>
        /// <returns>A collection of <see cref="IInvoice"/></returns>
        public IEnumerable<IInvoice> GetInvoicesByCustomer(int id)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IInvoice>.Builder.Where(x => x.CustomerId == id);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IInvoice"/> objects given an invoice status id
        /// </summary>
        /// <param name="invoiceStatusId">The id of the <see cref="IInvoiceStatus"/></param>
        /// <returns>A collection of <see cref="IInvoiceStatus"/></returns>
        public IEnumerable<IInvoice> GetInvoicesByInvoiceStatus(int invoiceStatusId)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IInvoice>.Builder.Where(x => x.InvoiceStatusId == invoiceStatusId);
                return repository.GetByQuery(query);
            }
        }

        #endregion

        public IEnumerable<IInvoice> GetAll()
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers


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
        /// Occurs before the status has been changed
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, StatusChangeEventArgs<IInvoice>> StatusChanging;

        /// <summary>
        /// Occurs after the status has been changed
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