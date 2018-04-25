namespace Merchello.Core.Persistence
{
    using System;

    using Cache;
    using Repositories;    
    using Services;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.SqlSyntax;

    using UnitOfWork;

    /// <summary>
    /// Used to instantiate each repository type
    /// </summary>    
    public class RepositoryFactory
    {
        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="CacheHelper"/>.
        /// </summary>
        private readonly CacheHelper _cacheHelper;

        /// <summary>
        /// The <see cref="ISqlSyntaxProvider"/>.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntax;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class.
        /// </summary>
        public RepositoryFactory()
            : this(ApplicationContext.Current.ApplicationCache)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public RepositoryFactory(ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : this(ApplicationContext.Current.ApplicationCache, logger, sqlSyntax)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class.
        /// </summary>
        /// <param name="disableAllCache">
        /// The disable all cache.
        /// </param>
        /// <param name="nullCacheProvider">
        /// The null cache provider.
        /// </param>
        /// <param name="runtimeCacheProvider">
        /// The runtime cache provider.
        /// </param>
        [Obsolete("Use constructor that passes CacheHelper")]
        public RepositoryFactory(bool disableAllCache, IRuntimeCacheProvider nullCacheProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : this(disableAllCache, nullCacheProvider, runtimeCacheProvider, Logger.CreateWithDefaultLog4NetConfiguration(), ApplicationContext.Current.DatabaseContext.SqlSyntax)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class.
        /// </summary>
        /// <param name="cacheHelper">
        /// The <see cref="CacheHelper"/>.
        /// </param>
        public RepositoryFactory(CacheHelper cacheHelper)
            : this(cacheHelper, Logger.CreateWithDefaultLog4NetConfiguration(), ApplicationContext.Current.DatabaseContext.SqlSyntax)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class.
        /// </summary>
        /// <param name="disableAllCache">
        /// The disable all cache.
        /// </param>
        /// <param name="nullCacheProvider">
        /// The null cache provider.
        /// </param>
        /// <param name="runtimeCacheProvider">
        /// The runtime cache provider.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntaxProvider">
        /// The SQL syntax provider.
        /// </param>
        [Obsolete("Use constructor that includes the CacheHelper")]
        public RepositoryFactory(
            bool disableAllCache,
            IRuntimeCacheProvider nullCacheProvider,
            IRuntimeCacheProvider runtimeCacheProvider,
            ILogger logger,
            ISqlSyntaxProvider sqlSyntaxProvider)
            : this(ApplicationContext.Current.ApplicationCache, logger, sqlSyntaxProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntaxProvider">
        /// The sql syntax provider.
        /// </param>
        internal RepositoryFactory(CacheHelper cache, ILogger logger, ISqlSyntaxProvider sqlSyntaxProvider)
        {
            _cacheHelper = cache;
            _logger = logger;
            _sqlSyntax = sqlSyntaxProvider;
        }

        /// <summary>
        /// Gets the <see cref="ISqlSyntaxProvider"/>.
        /// </summary>
        internal ISqlSyntaxProvider SqlSyntax
        {
            get
            {
                return _sqlSyntax;
            }
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAppliedPaymentRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IAppliedPaymentRepository"/>.
        /// </returns>
        internal virtual IAppliedPaymentRepository CreateAppliedPaymentRepository(IDatabaseUnitOfWork uow)
        {
            return new AppliedPaymentRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Creates an instance of the <see cref="IAuditLogRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLogRepository"/>.
        /// </returns>
        internal virtual IAuditLogRepository CreateAuditLogRepository(IDatabaseUnitOfWork uow)
        {
            return new AuditLogRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Creates an instance of the <see cref="INoteRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="INoteRepository"/>.
        /// </returns>
        internal virtual INoteRepository CreateNoteRepository(IDatabaseUnitOfWork uow)
        {
            return new NoteRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerRepository"/>.
        /// </returns>
        internal virtual ICustomerRepository CreateCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerRepository(uow, CreateCustomerAddressRepository(uow), CreateNoteRepository(uow), _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAnonymousCustomerRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IAnonymousCustomerRepository"/>.
        /// </returns>
        internal virtual IAnonymousCustomerRepository CreateAnonymousCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new AnonymousCustomerRepository(uow, _logger, _sqlSyntax);                
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerAddressRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddressRepository"/>.
        /// </returns>
        internal virtual ICustomerAddressRepository CreateCustomerAddressRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerAddressRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// The create digital media repository.
        /// </summary>
        /// <param name="uow">
        /// The <see cref="IDatabaseUnitOfWork"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMediaRepository"/>.
        /// </returns>
        internal virtual IDigitalMediaRepository CreateDigitalMediaRepository(IDatabaseUnitOfWork uow)
        {
            return new DigitalMediaRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DetachedContentTypeRepository"/>.
        /// </summary>
        /// <param name="uow">
        /// The <see cref="IDatabaseUnitOfWork"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentTypeRepository"/>.
        /// </returns>
        internal virtual IDetachedContentTypeRepository CreateDetachedContentTypeRepository(IDatabaseUnitOfWork uow)
        {
            return new DetachedContentTypeRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// The create entity collection repository.
        /// </summary>
        /// <param name="uow">
        /// The database unit of work.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollectionRepository"/>.
        /// </returns>
        internal virtual IEntityCollectionRepository CreateEntityCollectionRepository(IDatabaseUnitOfWork uow)
        {
            return new EntityCollectionRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IItemCacheRepository"/>
        /// </summary>
        /// <param name="uow">The <see cref="IDatabaseUnitOfWork"/></param>
        /// <returns>The <see cref="IItemCacheRepository"/></returns>        
        internal virtual IItemCacheRepository CreateItemCacheRepository(IDatabaseUnitOfWork uow)
        {
            return new ItemCacheRepository(uow, CreateCacheLineItemRespository(uow), _logger, _sqlSyntax);
        }

        /// <summary>
        /// Gets an instance of the <see cref="IItemCacheLineItemRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IItemCacheLineItemRepository"/>.
        /// </returns>
        internal virtual IItemCacheLineItemRepository CreateCacheLineItemRespository(IDatabaseUnitOfWork uow)
        {
            return new ItemCacheLineItemRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IInvoiceRepository"/>.
        /// </returns>
        internal virtual IInvoiceRepository CreateInvoiceRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceRepository(
                uow,
                CreateInvoiceLineItemRepository(uow),
                CreateOrderRepository(uow),
                CreateNoteRepository(uow), 
                _logger, 
                _sqlSyntax);
        }

        /// <summary>
        /// Gets an instance of the <see cref="IInvoiceLineItemRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IInvoiceLineItemRepository"/>.
        /// </returns>
        internal virtual IInvoiceLineItemRepository CreateInvoiceLineItemRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceLineItemRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceStatusRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IInvoiceStatusRepository"/>.
        /// </returns>
        internal virtual IInvoiceStatusRepository CreateInvoiceStatusRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceStatusRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// The create gateway provider repository.
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IGatewayProviderRepository"/>.
        /// </returns>
        internal virtual IGatewayProviderRepository CreateGatewayProviderRepository(IDatabaseUnitOfWork uow)
        {
            return new GatewayProviderRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns and instance of the <see cref="INotificationMessageRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="INotificationMessageRepository"/>.
        /// </returns>
        internal virtual INotificationMessageRepository CreateNotificationMessageRepository(IDatabaseUnitOfWork uow)
        {
            return new NotificationMessageRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="INotificationMethodRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="INotificationMethodRepository"/>.
        /// </returns>
        internal virtual INotificationMethodRepository CreateNotificationMethodRepository(IDatabaseUnitOfWork uow)
        {
            return new NotificationMethodRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// The create redeemed repository.
        /// </summary>
        /// <param name="uow">
        /// The database unit of work.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferRedeemedRepository"/>.
        /// </returns>
        internal virtual IOfferRedeemedRepository CreateOfferRedeemedRepository(IDatabaseUnitOfWork uow)
        {
            return new OfferRedeemedRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// The create offer settings repository.
        /// </summary>
        /// <param name="uow">
        /// The database unit of work.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettingsRepository"/>.
        /// </returns>
        internal virtual IOfferSettingsRepository CreateOfferSettingsRepository(IDatabaseUnitOfWork uow)
        {
            return new OfferSettingsRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IOrderRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IOrderRepository"/>.
        /// </returns>
        internal virtual IOrderRepository CreateOrderRepository(IDatabaseUnitOfWork uow)
        {
            return new OrderRepository(uow, CreateOrderLineItemRepository(uow), _logger, _sqlSyntax);
        }

        /// <summary>
        /// Gets an instance of the <see cref="IOrderLineItemRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IOrderLineItemRepository"/>.
        /// </returns>
        internal virtual IOrderLineItemRepository CreateOrderLineItemRepository(IDatabaseUnitOfWork uow)
        {
            return new OrderLineItemRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IOrderStatusRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IOrderStatusRepository"/>.
        /// </returns>
        internal virtual IOrderStatusRepository CreateOrderStatusRepository(IDatabaseUnitOfWork uow)
        {
            return new OrderStatusRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IPaymentRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentRepository"/>.
        /// </returns>
        internal virtual IPaymentRepository CreatePaymentRepository(IDatabaseUnitOfWork uow)
        {
            return new PaymentRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IPaymentMethodRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentMethodRepository"/>.
        /// </returns>
        internal virtual IPaymentMethodRepository CreatePaymentMethodRepository(IDatabaseUnitOfWork uow)
        {
            return new PaymentMethodRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IProductRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IProductRepository"/>.
        /// </returns>
        internal virtual IProductRepository CreateProductRepository(IDatabaseUnitOfWork uow)
        {
            return new ProductRepository(uow, _logger, _sqlSyntax, CreateProductVariantRepository(uow), CreateProductOptionRepository(uow));
        }

        /// <summary>
        /// Returns an instance of the <see cref="IProductOptionRepository"/>.
        /// </summary>
        /// <param name="uow">
        /// The database unit of work.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOptionRepository"/>.
        /// </returns>
        internal virtual IProductOptionRepository CreateProductOptionRepository(IDatabaseUnitOfWork uow)
        {
            return new ProductOptionRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IProductVariantRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariantRepository"/>.
        /// </returns>
        internal virtual IProductVariantRepository CreateProductVariantRepository(IDatabaseUnitOfWork uow)
        {
            return new ProductVariantRepository(uow, _logger, _sqlSyntax, CreateProductOptionRepository(uow));
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipCountryRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <param name="storeSettingService">
        /// The store Setting Service.
        /// </param>
        /// <returns>
        /// The <see cref="IShipCountryRepository"/>.
        /// </returns>
        internal virtual IShipCountryRepository CreateShipCountryRepository(IDatabaseUnitOfWork uow, IStoreSettingService storeSettingService)
        {
            return new ShipCountryRepository(uow, storeSettingService, _logger, _sqlSyntax);
        }


        /// <summary>
        /// Returns an instance of the <see cref="IShipMethodRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IShipMethodRepository"/>.
        /// </returns>
        internal virtual IShipMethodRepository CreateShipMethodRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipMethodRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipRateTierRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IShipRateTierRepository"/>.
        /// </returns>
        internal virtual IShipRateTierRepository CreateShipRateTierRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipRateTierRepository(uow, _logger, _sqlSyntax);       
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipmentRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IShipmentRepository"/>.
        /// </returns>
        internal virtual IShipmentRepository CreateShipmentRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipmentRepository(uow, CreateOrderLineItemRepository(uow), _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipmentStatusRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The unit of work.
        /// </param>
        /// <returns>
        /// The <see cref="IShipmentStatusRepository"/>.
        /// </returns>
        internal virtual IShipmentStatusRepository CreateShipmentStatusRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipmentStatusRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IStoreSettingRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IStoreSettingRepository"/>.
        /// </returns>
        internal virtual IStoreSettingRepository CreateStoreSettingRepository(IDatabaseUnitOfWork uow)
        {
            return new StoreSettingRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="ITaxMethodRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="ITaxMethodRepository"/>.
        /// </returns>
        internal virtual ITaxMethodRepository CreateTaxMethodRepository(IDatabaseUnitOfWork uow)
        {
            return new TaxMethodRepository(uow, _logger, _sqlSyntax);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IWarehouseRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouseRepository"/>.
        /// </returns>
        internal virtual IWarehouseRepository CreateWarehouseRepository(IDatabaseUnitOfWork uow)
        {
            return new WarehouseRepository(uow, this.CreateWarehouseCatalogRepository(uow), _logger, _sqlSyntax);
        }

        /// <summary>
        /// Creates an instance of the warehouse catalog repository.
        /// </summary>
        /// <param name="uow">
        /// The database unit of work.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouseCatalogRepository"/>.
        /// </returns>
        internal virtual IWarehouseCatalogRepository CreateWarehouseCatalogRepository(IDatabaseUnitOfWork uow)
        {
            return new WarehouseCatalogRepository(uow, _logger, _sqlSyntax);
        }
    }
}
