using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Events;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the InvoiceStatus Service 
    /// </summary>
    internal class InvoiceStatusService : IInvoiceStatusService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public InvoiceStatusService()
            : this(new RepositoryFactory())
        { }

        public InvoiceStatusService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public InvoiceStatusService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IInvoiceStatusService Members


        /// <summary>
        /// Creates an <see cref="IInvoiceStatus"/> object
        /// </summary>
        public IInvoiceStatus CreateInvoiceStatus(string name, string alias, bool reportable, bool active,  int sortOrder)
        {
            var invoiceStatus = new InvoiceStatus()
                {
                    Name = name, 
                    Alias = alias, 
                    Reportable = reportable, 
                    Active = active,  
                    SortOrder = sortOrder
                };
                
            Created.RaiseEvent(new NewEventArgs<IInvoiceStatus>(invoiceStatus), this);

            return invoiceStatus;
        }

        /// <summary>
        /// Saves a single <see cref="IInvoiceStatus"/> object
        /// </summary>
        /// <param name="invoiceStatus">The <see cref="IInvoiceStatus"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IInvoiceStatus invoiceStatus, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IInvoiceStatus>(invoiceStatus), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(uow))
                {
                    repository.AddOrUpdate(invoiceStatus);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IInvoiceStatus>(invoiceStatus), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IInvoiceStatus"/> objects.
        /// </summary>
        /// <param name="invoiceStatusList">Collection of <see cref="InvoiceStatus"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IInvoiceStatus> invoiceStatusList, bool raiseEvents = true)
        {
            var invoiceStatusArray = invoiceStatusList as IInvoiceStatus[] ?? invoiceStatusList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IInvoiceStatus>(invoiceStatusArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(uow))
                {
                    foreach (var invoiceStatus in invoiceStatusArray)
                    {
                        repository.AddOrUpdate(invoiceStatus);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IInvoiceStatus>(invoiceStatusArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IInvoiceStatus"/> object
        /// </summary>
        /// <param name="invoiceStatus">The <see cref="IInvoiceStatus"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IInvoiceStatus invoiceStatus, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IInvoiceStatus>( invoiceStatus), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(uow))
                {
                    repository.Delete( invoiceStatus);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IInvoiceStatus>( invoiceStatus), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IInvoiceStatus"/> objects
        /// </summary>
        /// <param name="invoiceStatusList">Collection of <see cref="IInvoiceStatus"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IInvoiceStatus> invoiceStatusList, bool raiseEvents = true)
        {
            var invoiceStatusArray = invoiceStatusList as IInvoiceStatus[] ?? invoiceStatusList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IInvoiceStatus>(invoiceStatusArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(uow))
                {
                    foreach (var invoiceStatus in invoiceStatusArray)
                    {
                        repository.Delete(invoiceStatus);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IInvoiceStatus>(invoiceStatusArray), this);
        }

        /// <summary>
        /// Gets a InvoiceStatus by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the InvoiceStatus</param>
        /// <returns><see cref="IInvoiceStatus"/></returns>
        public IInvoiceStatus GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of InvoiceStatus give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IInvoiceStatus> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        public IEnumerable<IInvoiceStatus> GetAll()
        {
            using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IInvoiceStatusService, SaveEventArgs<IInvoiceStatus>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IInvoiceStatusService, SaveEventArgs<IInvoiceStatus>> Saved;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IInvoiceStatusService, NewEventArgs<IInvoiceStatus>> Created;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IInvoiceStatusService, DeleteEventArgs<IInvoiceStatus>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IInvoiceStatusService, DeleteEventArgs<IInvoiceStatus>> Deleted;

        #endregion


     
    }
}