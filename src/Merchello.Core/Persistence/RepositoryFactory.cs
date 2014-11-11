namespace Merchello.Core.Persistence
{
    using Cache;
    using Repositories;    
    using Services;
    using Umbraco.Core.Cache;
    using UnitOfWork;

    /// <summary>
    /// Used to instantiate each repository type
    /// </summary>    
    public class RepositoryFactory
    {
        /// <summary>
        /// The disable all cache.
        /// </summary>
        private readonly bool _disableAllCache;

        /// <summary>
        /// The null cache provider.
        /// </summary>
        private readonly IRuntimeCacheProvider _nullCacheProvider;

        /// <summary>
        /// The runtime cache provider.
        /// </summary>
        private readonly IRuntimeCacheProvider _runtimeCacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class.
        /// </summary>
        public RepositoryFactory()
            : this(false, new NullCacheProvider(), new ObjectCacheRuntimeCacheProvider())
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
        public RepositoryFactory(bool disableAllCache, IRuntimeCacheProvider nullCacheProvider, IRuntimeCacheProvider runtimeCacheProvider)
        {
            _disableAllCache = disableAllCache;
            _nullCacheProvider = nullCacheProvider;
            _runtimeCacheProvider = runtimeCacheProvider;
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
            return new AppliedPaymentRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new AuditLogRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new TaxMethodRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new CustomerRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider, CreateCustomerAddressRepository(uow));
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
            return new AnonymousCustomerRepository(uow, _nullCacheProvider);                
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
            return new CustomerAddressRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IItemCacheRepository"/>
        /// </summary>
        /// <param name="uow">The <see cref="IDatabaseUnitOfWork"/></param>
        /// <returns>The <see cref="IItemCacheRepository"/></returns>        
        internal virtual IItemCacheRepository CreateItemCacheRepository(IDatabaseUnitOfWork uow)
        {
            return new ItemCacheRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider, CreateCacheLineItemRespository(uow));
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
            return new ItemCacheLineItemRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider,
                CreateInvoiceLineItemRepository(uow),
                CreateOrderRepository(uow));
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
            return new InvoiceLineItemRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new InvoiceStatusRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new GatewayProviderRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new NotificationMessageRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new NotificationMethodRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new OrderRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider, CreateOrderLineItemRepository(uow));
        }

        /// <summary>
        /// Gets an instance of teh <see cref="IOrderLineItemRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The database unit of work
        /// </param>
        /// <returns>
        /// The <see cref="IOrderLineItemRepository"/>.
        /// </returns>
        internal virtual IOrderLineItemRepository CreateOrderLineItemRepository(IDatabaseUnitOfWork uow)
        {
            return new OrderLineItemRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new OrderStatusRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new PaymentRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new PaymentMethodRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new ProductRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider, CreateProductVariantRepository(uow));
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
            return new ProductVariantRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new ShipCountryRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider, storeSettingService);
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
            return new ShipMethodRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new ShipRateTierRepository(uow, _nullCacheProvider);       
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
            return new ShipmentRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider, CreateOrderLineItemRepository(uow));
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipmentStatusRepository"/>
        /// </summary>
        /// <param name="uow">
        /// The uow.
        /// </param>
        /// <returns>
        /// The <see cref="IShipmentStatusRepository"/>.
        /// </returns>
        internal virtual IShipmentStatusRepository CreateShipmentStatusRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipmentStatusRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new StoreSettingRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
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
            return new WarehouseRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider, this.CreateWarehouseCatalogRepository(uow));
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
            return new WarehouseCatalogRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }
    }
}
