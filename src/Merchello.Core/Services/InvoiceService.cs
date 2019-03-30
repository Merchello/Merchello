namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web.UI;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.Repositories;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        public InvoiceService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public InvoiceService(ILogger logger)
            : this(logger, ApplicationContext.Current.DatabaseContext.SqlSyntax)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        internal InvoiceService(ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : this(new Persistence.RepositoryFactory(logger, sqlSyntax), logger, new AppliedPaymentService(logger, sqlSyntax), new OrderService(logger, sqlSyntax), new StoreSettingService(logger, sqlSyntax))
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
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
            Persistence.RepositoryFactory repositoryFactory,
            ILogger logger,
            IAppliedPaymentService appliedPaymentService,
            IOrderService orderService,
            IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger, appliedPaymentService, orderService, storeSettingService)
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
        /// <param name="logger">
        /// The logger.
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
            Persistence.RepositoryFactory repositoryFactory,
            ILogger logger,
            IAppliedPaymentService appliedPaymentService,
            IOrderService orderService,
            IStoreSettingService storeSettingService)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory(), appliedPaymentService, orderService, storeSettingService)
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
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
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
            Persistence.RepositoryFactory repositoryFactory,
            ILogger logger,
            IEventMessagesFactory eventMessagesFactory,
            IAppliedPaymentService appliedPaymentService,
            IOrderService orderService,
            IStoreSettingService storeSettingService)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
            Mandate.ParameterNotNull(appliedPaymentService, "appliedPaymentService");
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");
            Mandate.ParameterNotNull(orderService, "orderService");

            _appliedPaymentService = appliedPaymentService;
            _orderService = orderService;
            _storeSettingService = storeSettingService;
        }

        #endregion

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

            var defaultCurrencyCode = this.GetDefaultCurrencyCode();

            var invoice = new Invoice(status)
            {
                VersionKey = Guid.NewGuid(),
                InvoiceNumber = invoiceNumber,
                InvoiceDate = DateTime.Now,
                CurrencyCode = defaultCurrencyCode
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
            if (!((Invoice)invoice).HasIdentity && invoice.InvoiceNumber <= 0)
            {
                // We have to generate a new 'unique' invoice number off the configurable value
                ((Invoice)invoice).InvoiceNumber = _storeSettingService.GetNextInvoiceNumber();
            }

            var includesStatusChange = ((Invoice)invoice).IsPropertyDirty("InvoiceStatus") &&
                                       ((Invoice)invoice).HasIdentity == true;

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
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateInvoiceRepository(uow))
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
            var newInvoiceCount = invoicesArray.Count(x => x.InvoiceNumber <= 0 && !((Invoice)x).HasIdentity);
            if (newInvoiceCount > 0)
            {
                var lastInvoiceNumber =
                    _storeSettingService.GetNextInvoiceNumber(newInvoiceCount);
                foreach (var newInvoice in invoicesArray.Where(x => x.InvoiceNumber <= 0 && !((Invoice)x).HasIdentity))
                {
                    ((Invoice)newInvoice).InvoiceNumber = lastInvoiceNumber;
                    lastInvoiceNumber = lastInvoiceNumber - 1;
                }
            }

            var existingInvoicesWithStatusChanges =
                invoicesArray.Where(
                    x => ((Invoice)x).HasIdentity == true && ((Invoice)x).IsPropertyDirty("InvoiceStatus"))
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
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateInvoiceRepository(uow))
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
                    ((Invoice)invoice).WasCancelled = true;
                    return;
                }

            DeleteAppliedPayments(invoice);

            DeleteOrders(invoice);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateInvoiceRepository(uow))
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
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateInvoiceRepository(uow))
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
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
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
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
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
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
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
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
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
            var appliedPayments = _appliedPaymentService.GetAppliedPaymentsByPaymentKey(paymentKey);
            if (appliedPayments.Any())
            {
                return GetByKeys(appliedPayments.Select(x => x.InvoiceKey));
            };
            return new IInvoice[] { };

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
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
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
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
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
        /// Gets the total count of all invoices within a date range.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> representing the count of invoices.
        /// </returns>
        public int CountInvoices(DateTime startDate, DateTime endDate)
        {
            var query =
                Persistence.Querying.Query<IInvoice>.Builder.Where(
                    x => x.InvoiceDate >= startDate && x.InvoiceDate <= endDate);

            return Count(query);
        }


        /// <summary>
        /// Gets the total count of all invoices within a date range and customer type
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="customerType">
        /// The customer Type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> representing the count of invoices.
        /// </returns>
        public int CountInvoices(DateTime startDate, DateTime endDate, CustomerType customerType)
        {
            var query = customerType == CustomerType.Anonymous ?
                Persistence.Querying.Query<IInvoice>.Builder.Where(
                    x => x.InvoiceDate >= startDate && x.InvoiceDate <= endDate && x.CustomerKey == null) :
                Persistence.Querying.Query<IInvoice>.Builder.Where(
                    x => x.InvoiceDate >= startDate && x.InvoiceDate <= endDate && x.CustomerKey != null);

            return Count(query);
        }

        /// <summary>
        /// Gets the totals of invoices in a date range for a specific currency code.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <returns>
        /// The sum of the invoice totals.
        /// </returns>
        public decimal SumInvoiceTotals(DateTime startDate, DateTime endDate, string currencyCode)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.SumInvoiceTotals(startDate, endDate, currencyCode);
            }
        }

        /// <summary>
        /// Gets the total of line items for a give SKU invoiced in a specific currency across the date range.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The total of line items for a give SKU invoiced in a specific currency across the date range.
        /// </returns>
        public decimal SumLineItemTotalsBySku(DateTime startDate, DateTime endDate, string currencyCode, string sku)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.SumLineItemTotalsBySku(startDate, endDate, currencyCode, sku);
            }
        }

        /// <summary>
        /// Gets distinct currency codes used in invoices.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public IEnumerable<string> GetDistinctCurrencyCodes()
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetDistinctCurrencyCodes();
            }
        }

        /// <summary>
        /// Gets an <see cref="IInvoiceStatus"/> by it's key
        /// </summary>
        /// <param name="key">The <see cref="IInvoiceStatus"/> key</param>
        /// <returns><see cref="IInvoiceStatus"/></returns>
        public IInvoiceStatus GetInvoiceStatusByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateInvoiceStatusRepository(UowProvider.GetUnitOfWork()))
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
            using (var repository = RepositoryFactory.CreateInvoiceStatusRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        #region Static Collections


        /// <summary>
        /// The add invoice to collection.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void AddToCollection(IInvoice invoice, IEntityCollection collection)
        {
            this.AddToCollection(invoice, collection.Key);
        }

        /// <summary>
        /// The add invoice to collection.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(IInvoice invoice, Guid collectionKey)
        {
            this.AddToCollection(invoice.Key, collectionKey);
        }

        /// <summary>
        /// The add invoice to collection.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(Guid invoiceKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                repository.AddToCollection(invoiceKey, collectionKey);
            }
        }

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void RemoveFromCollection(IInvoice invoice, IEntityCollection collection)
        {
            this.RemoveFromCollection(invoice, collection.Key);
        }

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void RemoveFromCollection(IInvoice invoice, Guid collectionKey)
        {
            this.RemoveFromCollection(invoice.Key, collectionKey);
        }

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void RemoveFromCollection(Guid invoiceKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                repository.RemoveFromCollection(invoiceKey, collectionKey);
            }
        }

        /// <summary>
        /// Determines if an invoice exists in a collection.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid invoiceKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.ExistsInCollection(invoiceKey, collectionKey);
            }
        }

        /// <summary>
        /// Returns true if the entity exists in the at least one of the static collections.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid entityKey, IEnumerable<Guid> collectionKeys)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.ExistsInCollection(entityKey, collectionKeys.ToArray());
            }
        }

        /// <summary>
        /// The get invoices from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetFromCollection(
                    collectionKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets distinct invoices from multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetProductsThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetEntitiesThatExistInAllCollections(
                    collectionKeys.ToArray(),
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
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
        public Page<IInvoice> GetFromCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetFromCollection(
                    collectionKey,
                    searchTerm,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets distinct filtered invoices from multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
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
        public Page<IInvoice> GetProductsThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetEntitiesThatExistInAllCollections(
                    collectionKeys.ToArray(),
                    searchTerm,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get invoice keys from static collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        internal Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysFromCollection(
                    collectionKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
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
        internal Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysFromCollection(
                    collectionKey,
                    searchTerm,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get invoice keys from static collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysNotInCollection(
                    collectionKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
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
        internal Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysNotInCollection(
                    collectionKey,
                    searchTerm,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        #endregion

        #region Filter Queries


        /// <summary>
        /// Gets invoices matching the search term and the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<IInvoice> GetInvoicesMatchingInvoiceStatus(
            string searchTerm,
            Guid invoiceStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoicesMatchingInvoiceStatus(
                    searchTerm,
                    invoiceStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoice keys matching the search term and the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<Guid> GetInvoiceKeysMatchingInvoiceStatus(
            string searchTerm,
            Guid invoiceStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoiceKeysMatchingInvoiceStatus(
                    searchTerm,
                    invoiceStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoices matching the search term but not the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<IInvoice> GetInvoicesMatchingTermNotInvoiceStatus(
            string searchTerm,
            Guid invoiceStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoicesMatchingTermNotInvoiceStatus(
                    searchTerm,
                    invoiceStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoice keys matching the search term but not the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<Guid> GetInvoiceKeysMatchingTermNotInvoiceStatus(
            string searchTerm,
            Guid invoiceStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoiceKeysMatchingTermNotInvoiceStatus(
                    searchTerm,
                    invoiceStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoices matching the search term and the order status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<IInvoice> GetInvoicesMatchingOrderStatus(
            string searchTerm,
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoicesMatchingOrderStatus(
                    searchTerm,
                    orderStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoice keys matching the search term and the order status key.
        /// </summary>
        /// <param name="orderStatusKey">
        /// The order status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetInvoiceKeysMatchingOrderStatus(
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoiceKeysMatchingOrderStatus(
                    orderStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoice keys matching the search term and the order status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetInvoiceKeysMatchingOrderStatus(
            string searchTerm,
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoiceKeysMatchingOrderStatus(
                    searchTerm,
                    orderStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoices matching the search term but not the order status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        internal Page<IInvoice> GetInvoicesMatchingTermNotOrderStatus(
            string searchTerm,
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoicesMatchingTermNotOrderStatus(
                    searchTerm,
                    orderStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoice keys matching the search term but not the order status key.
        /// </summary>
        /// <param name="orderStatusKey">
        /// The order status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetInvoiceKeysMatchingTermNotOrderStatus(
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoiceKeysMatchingTermNotOrderStatus(
                    orderStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets invoice keys matching the search term but not the order status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetInvoiceKeysMatchingTermNotOrderStatus(
            string searchTerm,
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetInvoiceKeysMatchingTermNotOrderStatus(
                    searchTerm,
                    orderStatusKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        #endregion

        /// <summary>
        /// Synchronizes invoice adjustments.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="adjustments">
        /// The adjustments.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool AdjustInvoice(IInvoice invoice, IEnumerable<IInvoiceLineItem> adjustments)
        {
            if (invoice != null)
            {
                var existing = invoice.Items.Where(x => x.LineItemType == LineItemType.Adjustment).ToArray();

                var invoiceLineItems = adjustments as IInvoiceLineItem[] ?? adjustments.ToArray();
                var goodKeys = invoiceLineItems.Where(z => z.Key != Guid.Empty).Select(y => y.Key);

                // remove existing adjustments not found
                var removers = existing.Any() && !invoiceLineItems.Any() ? existing : existing.Where(x => goodKeys.All(y => y != x.Key));
                foreach (var remove in removers)
                {
                    invoice.Items.Remove(remove.Sku);
                }

                // add new adjustments
                var adds = invoiceLineItems.Where(x => x.Key == Guid.Empty);
                foreach (var add in adds)
                {
                    invoice.Items.Add(add);
                }

                var charges = invoice.Items.Where(x => x.LineItemType != LineItemType.Discount).Sum(x => x.TotalPrice);
                var discounts = invoice.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.TotalPrice);
                decimal converted;
                invoice.Total = Math.Round(decimal.TryParse((charges - discounts).ToString(CultureInfo.InvariantCulture), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out converted) ? converted : 0, 2);
                Save(invoice);

                invoice.EnsureInvoiceStatus();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets list of all <see cref="IInvoice"/>
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IInvoice"/>.
        /// </returns>
        internal IEnumerable<IInvoice> GetAll()
        {
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
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
            using (var repository = RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Count(query);
            }
        }

        /// <summary>
        /// Gets the default currency code.
        /// </summary>
        /// <returns>
        /// The currency code saved in the store settings.
        /// </returns>
        internal string GetDefaultCurrencyCode()
        {
            return this._storeSettingService.GetByKey(Core.Constants.StoreSetting.CurrencyCodeKey).Value;
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
                RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()),
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
            using (var repository = (InvoiceRepository)RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.SearchKeys(searchTerm, page, itemsPerPage, ValidateSortByField(sortBy));
            }
        }

        /// <summary>
        /// Gets a page by search term and a date range
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
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
            DateTime startDate,
            DateTime endDate,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = (InvoiceRepository)RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.SearchKeys(searchTerm, startDate, endDate, page, itemsPerPage, ValidateSortByField(sortBy));
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
                RepositoryFactory.CreateInvoiceRepository(UowProvider.GetUnitOfWork()),
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