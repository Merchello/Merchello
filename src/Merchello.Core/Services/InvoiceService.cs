using System;
using System.Collections.Generic;
using System.Threading;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.UnitOfWork;
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

        public IInvoice CreateInvoice(Guid invoiceStatusKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a single <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IInvoice invoice, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a collection of <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoices">The collection of <see cref="IInvoice"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IInvoice> invoices, bool raiseEvents = true)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="IInvoice"/> given it's unique 'InvoiceNumber'
        /// </summary>
        /// <param name="invoiceNumber">The invoice number of the <see cref="IInvoice"/> to be retrieved</param>
        /// <returns><see cref="IInvoice"/></returns>
        public IInvoice GetByInvoiceNumber(int invoiceNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets list of <see cref="IInvoice"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of guid 'key' for the invoices to retrieve</param>
        /// <returns>List of <see cref="IInvoice"/></returns>
        public IEnumerable<IInvoice> GetByKeys(IEnumerable<Guid> keys)
        {
            throw new NotImplementedException();
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
        public static event TypedEventHandler<IInvoiceService, StatusChangeEventArgs<IInvoiceService>> StatusChanging; 

        /// <summary>
        /// Occurs after an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IInvoiceService, StatusChangeEventArgs<IInvoiceService>> StatusChanged; 

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