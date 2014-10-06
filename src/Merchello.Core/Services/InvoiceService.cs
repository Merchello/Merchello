namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web.UI;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.Repositories;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

    using RepositoryFactory = Merchello.Core.Persistence.RepositoryFactory;

    /// <summary>
    /// Represents the InvoiceService
    /// </summary>
    public class InvoiceService : PageCachedServiceBase<IInvoice>, IInvoiceService
    {
        #region Fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// The valid sort fields.
        /// </summary>
        private static readonly string[] ValidSortFields = { "invoicenumber", "invoicedate", "billtoname", "billtoemail" };

        /// <summary>
        /// The unit of work provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        /// <summary>
        /// The applied payment service.
        /// </summary>
        private readonly IAppliedPaymentService _appliedPaymentService;

        /// <summary>
        /// The order service.
        /// </summary>
        private readonly IOrderService _orderService;

        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;
        
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        public InvoiceService()
            : this(new RepositoryFactory(), new AppliedPaymentService(), new OrderService(),  new StoreSettingService())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="appliedPaymentService">
        /// The applied payment service.
        /// </param>
        /// <param name="orderService">
        /// The order service.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        internal InvoiceService(RepositoryFactory repositoryFactory, IAppliedPaymentService appliedPaymentService, IOrderService orderService, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, appliedPaymentService, orderService, storeSettingService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="appliedPaymentService">
        /// The applied payment service.
        /// </param>
        /// <param name="orderService">
        /// The order service.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        internal InvoiceService(
            IDatabaseUnitOfWorkProvider provider, 
            RepositoryFactory repositoryFactory, 
            IAppliedPaymentService appliedPaymentService, 
            IOrderService orderService,
            IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(appliedPaymentService, "appliedPaymentService");
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");
            Mandate.ParameterNotNull(orderService, "orderService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _appliedPaymentService = appliedPaymentService;
            _orderService = orderService;
            _storeSettingService = storeSettingService;
        }

        #region Event Handlers

        /// <summary>
        /// Occurs before the Create
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

        /// <summary>
        /// Creates a <see cref="IInvoice"/> without saving it to the database
        /// </summary>
        /// <param name="invoiceStatusKey">The <see cref="IInvoiceStatus"/> key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IInvoice"/></returns>
        public IInvoice CreateInvoice(Guid invoiceStatusKey, bool raiseEvents = true)
        {
            return CreateInvoice(invoiceStatusKey, 0, raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="IInvoice"/> with an assigned invoice number without saving it to the database
        /// </summary>
        /// <param name="invoiceStatusKey">
        /// The <see cref="IInvoiceStatus"/> key
        /// </param>
        /// <param name="invoiceNumber">
        /// The invoice Number
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// <see cref="IInvoice"/>
        /// </returns>
        /// <remarks>
        /// Invoice number must be a positive integer value or zero
        /// </remarks>
        public IInvoice CreateInvoice(Guid invoiceStatusKey, int invoiceNumber, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(Guid.Empty != invoiceStatusKey, "invoiceStatusKey");
            Mandate.ParameterCondition(invoiceNumber >= 0, "invoiceNumber must be greater than or equal to 0");

            var status = GetInvoiceStatusByKey(invoiceStatusKey);

            var invoice = new Invoice(status)
            {
                VersionKey = Guid.NewGuid(),
                InvoiceNumber = invoiceNumber,
                InvoiceDate = DateTime.Now
            };

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IInvoice>(invoice), this))
                {
                    invoice.WasCancelled = true;
                    return invoice;
                }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IInvoice>(invoice), this);

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
                ((Invoice) invoice).InvoiceNumber = _storeSettingService.GetNextInvoiceNumber();
            }

            var includesStatusChange = ((Invoice) invoice).IsPropertyDirty("InvoiceStatusKey") &&
                                       ((Invoice) invoice).HasIdentity == false;

            if (raiseEvents)
            {
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IInvoice>(invoice), this))
                {
                    ((Invoice)invoice).WasCancelled = true;
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
            if (includesStatusChange) StatusChanged.RaiseEvent(new StatusChangeEventArgs<IInvoice>(invoice), this);
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
                var lastInvoiceNumber =
                    _storeSettingService.GetNextInvoiceNumber(newInvoiceCount);
                foreach (var newInvoice in invoicesArray.Where(x => x.InvoiceNumber <= 0 && !((Invoice) x).HasIdentity))
                {
                    ((Invoice) newInvoice).InvoiceNumber = lastInvoiceNumber;
                    lastInvoiceNumber = lastInvoiceNumber - 1;
                }
            }

            var existingInvoicesWithStatusChanges =
                invoicesArray.Where(
                    x => ((Invoice) x).HasIdentity == false && ((Invoice) x).IsPropertyDirty("InvoiceStatusKey"))
                    .ToArray();

            if (raiseEvents)
            {
                Saving.RaiseEvent(new SaveEventArgs<IInvoice>(invoicesArray), this);
                if (existingInvoicesWithStatusChanges.Any())
                    StatusChanging.RaiseEvent(
                        new StatusChangeEventArgs<IInvoice>(existingInvoicesWithStatusChanges),
                        this);
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
                if (existingInvoicesWithStatusChanges.Any())
                    StatusChanged.RaiseEvent(new StatusChangeEventArgs<IInvoice>(existingInvoicesWithStatusChanges), this);
            }
        }

        /// <summary>
        /// Deletes a single <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IInvoice invoice, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IInvoice>(invoice), this))
                {
                    ((Invoice) invoice).WasCancelled = true;
                    return;
                }

            DeleteAppliedPayments(invoice);

            DeleteOrders(invoice);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceRepository(uow))
                {
                    repository.Delete(invoice);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IInvoice>(invoice), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoices">The collection of <see cref="IInvoice"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IInvoice> invoices, bool raiseEvents = true)
        {
            var invoicesArray = invoices as IInvoice[] ?? invoices.ToArray();
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IInvoice>(invoicesArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateInvoiceRepository(uow))
                {
                    foreach (var invoice in invoicesArray)
                    {
                        DeleteAppliedPayments(invoice);

                        DeleteOrders(invoice);

                        repository.Delete(invoice);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IInvoice>(invoicesArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IInvoice"/> given it's unique 'key' (GUID)
        /// </summary>
        /// <param name="key">The <see cref="IInvoice"/>'s unique 'key' (GUID)</param>
        /// <returns><see cref="IInvoice"/></returns>
        public override IInvoice GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a <see cref="Page{IInvoice}"/>
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public override Page<IInvoice> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IInvoice>.Builder.Where(x => x.Key != Guid.Empty);

                return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
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
                var query = Persistence.Querying.Query<IInvoice>.Builder.Where(x => x.InvoiceNumber == invoiceNumber);

                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets list of <see cref="IInvoice"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of GUID 'key' for the invoices to retrieve</param>
        /// <returns>List of <see cref="IInvoice"/></returns>
        public IEnumerable<IInvoice> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IInvoice"/> objects that are associated with a <see cref="IPayment"/> by the payments 'key'
        /// </summary>
        /// <param name="paymentKey">The <see cref="IPayment"/> key (GUID)</param>
        /// <returns>A collection of <see cref="IInvoice"/></returns>
        public IEnumerable<IInvoice> GetInvoicesByPaymentKey(Guid paymentKey)
        {
            var invoiceKeys = _appliedPaymentService.GetAppliedPaymentsByPaymentKey(paymentKey).Select(x => x.InvoiceKey).ToArray();

            return GetByKeys(invoiceKeys);
        }

        /// <summary>
        /// Get invoices by a customer key.
        /// </summary>
        /// <param name="customeryKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IInvoice"/>.
        /// </returns>
        public IEnumerable<IInvoice> GetInvoicesByCustomerKey(Guid customeryKey)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IInvoice>.Builder.Where(x => x.CustomerKey == customeryKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets the count of invoice by date range.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The count the invoices.
        /// </returns>
        public IEnumerable<IInvoice> GetInvoicesByDateRange(DateTime startDate, DateTime endDate)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IInvoice>.Builder.Where(x => x.InvoiceDate >= startDate && x.InvoiceDate <= endDate);

                return repository.GetByQuery(query);
            }
        }


        /// <summary>
        /// Gets the total count of all <see cref="IInvoice"/>
        /// </summary>
        /// <returns>The count of <see cref="IInvoice"/></returns>
        public int CountInvoices()
        {
            return this.Count(Persistence.Querying.Query<IInvoice>.Builder.Where(x => x.Key != Guid.Empty));
        }


        /// <summary>
        /// Gets an <see cref="IInvoiceStatus"/> by it's key
        /// </summary>
        /// <param name="key">The <see cref="IInvoiceStatus"/> key</param>
        /// <returns><see cref="IInvoiceStatus"/></returns>
        public IInvoiceStatus GetInvoiceStatusByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Returns a collection of all <see cref="IInvoiceStatus"/>
        /// </summary>
        /// <returns>
        /// The collection of invoice statuses.
        /// </returns>
        /// <remarks>
        /// TODO move this to an internal InvoiceStatusService
        /// </remarks>
        public IEnumerable<IInvoiceStatus> GetAllInvoiceStatuses()
        {
            using (var repository = _repositoryFactory.CreateInvoiceStatusRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Gets list of all <see cref="IInvoice"/>
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IInvoice"/>.
        /// </returns>
        internal IEnumerable<IInvoice> GetAll()
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            } 
        }

        /// <summary>
        /// The count of invoices.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal override int Count(IQuery<IInvoice> query)
        {
            using (var repository = _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Count(query);
            }
        }


        #region Key Queries

        /// <summary>
        /// Gets a page of Keys
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The order by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        /// <remarks>
        /// This is used by large back office collections usually backed by Examine (Lucene) backed cache
        /// </remarks>
        internal override Page<Guid> GetPagedKeys(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            return GetPagedKeys(
                _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()),
                Persistence.Querying.Query<IInvoice>.Builder.Where(x => x.Key != Guid.Empty),
                page,
                itemsPerPage,
                ValidateSortByField(sortBy),
                sortDirection);
        }

        /// <summary>
        /// Gets a page by search term
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        /// <remarks>
        /// The search is prefabricated in the repository
        /// </remarks>
        internal Page<Guid> GetPagedKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = (InvoiceRepository)_repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.SearchKeys(searchTerm, page, itemsPerPage, ValidateSortByField(sortBy));
            }
        }       

        /// <summary>
        /// Gets a page by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<Guid> GetPagedKeys(
            IQuery<IInvoice> query,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetPagedKeys(
                _repositoryFactory.CreateInvoiceRepository(_uowProvider.GetUnitOfWork()),
                query,
                page,
                itemsPerPage,
                ValidateSortByField(sortBy),
                sortDirection);
        }

        /// <summary>
        /// Validates the sort by string is a valid sort by field
        /// </summary>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// A validated database field name.
        /// </returns>
        protected override string ValidateSortByField(string sortBy)
        {
            return ValidSortFields
                .Contains(sortBy.ToLowerInvariant()) ? sortBy : "invoiceNumber";
        }

        #endregion

        /// <summary>
        /// Deletes orders associated with the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        private void DeleteOrders(IInvoice invoice)
        {
            var orders = _orderService.GetOrdersByInvoiceKey(invoice.Key).ToArray();

            if (orders.Any()) _orderService.Delete(orders);
        }

        /// <summary>
        /// The delete applied payments.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        private void DeleteAppliedPayments(IInvoice invoice)
        {
            var appliedPayments = _appliedPaymentService.GetAppliedPaymentsByInvoiceKey(invoice.Key).ToArray();

            if (appliedPayments.Any()) _appliedPaymentService.Delete(appliedPayments);
        }
    }
}