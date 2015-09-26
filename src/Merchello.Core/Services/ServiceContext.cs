namespace Merchello.Core.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The Merchello ServiceContext, which provides access to the following services:
    /// <see cref="ICustomerService"/>, <see cref="IGatewayProviderService"/>, <see cref="IInvoiceService"/>, <see cref="IItemCacheService"/> 
    /// <see cref="IOrderService"/>, <see cref="IPaymentService"/>, <see cref="IProductService"/>, <see cref="IProductVariantService"/>,
    /// <see cref="IStoreSettingService"/>, <see cref="IShipmentService"/>, and <see cref="IWarehouseService"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
    public class ServiceContext : IServiceContext
    {
        #region Field

        /// <summary>
        /// The anonymous customer service.
        /// </summary>
        private Lazy<IAnonymousCustomerService> _anonymousCustomerService; 

        /// <summary>
        /// The applied payment service.
        /// </summary>
        private Lazy<IAppliedPaymentService> _appliedPaymentService;

        /// <summary>
        /// The audit log service.
        /// </summary>
        private Lazy<IAuditLogService> _auditLogService;

        /// <summary>
        /// The country tax rate service.
        /// </summary>
        private Lazy<ITaxMethodService> _countryTaxRateService;

        /// <summary>
        /// The customer address service.
        /// </summary>
        private Lazy<ICustomerAddressService> _customerAddressService;

        /// <summary>
        /// The customer service.
        /// </summary>
        private Lazy<ICustomerService> _customerService;

        /// <summary>
        /// The detached content type service.
        /// </summary>
        private Lazy<IDetachedContentTypeService> _detachedContentTypeService; 

        /// <summary>
        /// The digital media service.
        /// </summary>
        private Lazy<IDigitalMediaService> _digitalMediaService;

        /// <summary>
        /// The entity collection service.
        /// </summary>
        private Lazy<IEntityCollectionService> _entityCollectionService; 

        /// <summary>
        /// The invoice service.
        /// </summary>
        private Lazy<IInvoiceService> _invoiceService;

        /// <summary>
        /// The item cache service.
        /// </summary>
        private Lazy<IItemCacheService> _itemCacheService;

        /// <summary>
        /// The gateway provider service.
        /// </summary>
        private Lazy<IGatewayProviderService> _gatewayProviderService;

        /// <summary>
        /// The order service.
        /// </summary>
        private Lazy<IOrderService> _orderService;

        /// <summary>
        /// The offer settings service.
        /// </summary>
        private Lazy<IOfferSettingsService> _offerSettingsService;

        /// <summary>
        /// The offer redeemed service.
        /// </summary>
        private Lazy<IOfferRedeemedService> _offerRedeemedService; 

        /// <summary>
        /// The notification method service.
        /// </summary>
        private Lazy<INotificationMethodService> _notificationMethodService;

        /// <summary>
        /// The notification message service.
        /// </summary>
        private Lazy<INotificationMessageService> _notificationMessageService;

        /// <summary>
        /// The payment service.
        /// </summary>
        private Lazy<IPaymentService> _paymentService;

        /// <summary>
        /// The payment method service.
        /// </summary>
        private Lazy<IPaymentMethodService> _paymentMethodService;

        /// <summary>
        /// The product service.
        /// </summary>
        private Lazy<IProductService> _productService;

        /// <summary>
        /// The product variant service.
        /// </summary>
        private Lazy<IProductVariantService> _productVariantService;

        /// <summary>
        /// The store settings service.
        /// </summary>
        private Lazy<IStoreSettingService> _storeSettingsService;

        /// <summary>
        /// The ship country service.
        /// </summary>
        private Lazy<IShipCountryService> _shipCountryService;

        /// <summary>
        /// The ship method service.
        /// </summary>
        private Lazy<IShipMethodService> _shipMethodService;

        /// <summary>
        /// The ship rate tier service.
        /// </summary>
        private Lazy<IShipRateTierService> _shipRateTierService;

        /// <summary>
        /// The shipment service.
        /// </summary>
        private Lazy<IShipmentService> _shipmentService;

        /// <summary>
        /// The warehouse service.
        /// </summary>
        private Lazy<IWarehouseService> _warehouseService;

        /// <summary>
        /// The _warehouse catalog service.
        /// </summary>
        private Lazy<IWarehouseCatalogService> _warehouseCatalogService;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContext"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository Factory.
        /// </param>
        /// <param name="dbUnitOfWorkProvider">
        /// The database unit of work provider.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event Messages Factory.
        /// </param>
        public ServiceContext(
            RepositoryFactory repositoryFactory,
            IDatabaseUnitOfWorkProvider dbUnitOfWorkProvider,
            ILogger logger,
            IEventMessagesFactory eventMessagesFactory)
        {
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(dbUnitOfWorkProvider, "dbUnitOfWorkProvider");
            Mandate.ParameterNotNull(logger, "logger");
            Mandate.ParameterNotNull(eventMessagesFactory, "eventMessagesFactory");

            DatabaseUnitOfWorkProvider = dbUnitOfWorkProvider;
            BuildServiceContext(dbUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory);
        }

        #region IServiceContext Members

        /// <summary>
        /// Gets the <see cref="IAuditLogService"/>
        /// </summary>
        public IAuditLogService AuditLogService
        {
            get { return _auditLogService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ICustomerService"/>
        /// </summary>
        public ICustomerService CustomerService
        {
            get { return _customerService.Value;  }
        }

        /// <summary>
        /// Gets the <see cref="IDigitalMediaService"/>.
        /// </summary>
        public IDigitalMediaService DigitalMediaService
        {
            get
            {
                return _digitalMediaService.Value; 
            }
        }

        /// <summary>
        /// Gets the <see cref="IEntityCollectionService"/>.
        /// </summary>
        public IEntityCollectionService EntityCollectionService
        {
            get { return _entityCollectionService.Value; }
        }


        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        public IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IInvoiceService"/>
        /// </summary>
        public IInvoiceService InvoiceService
        {
            get { return _invoiceService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IItemCacheService"/>
        /// </summary>
        public IItemCacheService ItemCacheService
        {
            get { return _itemCacheService.Value;  }
        }

        /// <summary>
        /// Gets the <see cref="IOrderService"/>
        /// </summary>
        public IOrderService OrderService
        {
            get { return _orderService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IOfferSettingsService"/>.
        /// </summary>
        public IOfferSettingsService OfferSettingsService
        {
            get
            {
                return _offerSettingsService.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IPaymentService"/>
        /// </summary>
        public IPaymentService PaymentService
        {
            get { return _paymentService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IProductService"/>
        /// </summary>
        public IProductService ProductService
        {
            get { return _productService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IProductVariantService"/>
        /// </summary>
        public IProductVariantService ProductVariantService
        {
            get { return _productVariantService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IStoreSettingService"/>
        /// </summary>
        public IStoreSettingService StoreSettingService
        {
            get { return _storeSettingsService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IShipmentService"/>
        /// </summary>
        public IShipmentService ShipmentService
        {
            get { return _shipmentService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IWarehouseService"/>
        /// </summary>
        public IWarehouseService WarehouseService
        {
            get { return _warehouseService.Value; }
        }

        /// <summary>
        /// Gets the database unit of work provider.
        /// </summary>
        /// <remarks>Used for testing</remarks>
        internal IDatabaseUnitOfWorkProvider DatabaseUnitOfWorkProvider { get; private set; }

        /// <summary>
        /// Gets the <see cref="IAppliedPaymentService"/>.
        /// </summary>
        internal IAppliedPaymentService AppliedPaymentService
        {
            get
            {
                return _appliedPaymentService.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IAnonymousCustomerService"/>.
        /// </summary>
        internal IAnonymousCustomerService AnonymousCustomerService
        {
            get { return _anonymousCustomerService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IDetachedContentTypeService"/>.
        /// </summary>
        internal IDetachedContentTypeService DetachedContentTypeService
        {
            get
            {
                return _detachedContentTypeService.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITaxMethodService"/>
        /// </summary>
        internal ITaxMethodService TaxMethodService
        {
            get { return _countryTaxRateService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ICustomerAddressService"/>
        /// </summary>
        internal ICustomerAddressService CustomerAddressService
        {
            get { return _customerAddressService.Value; }
        }
    
        /// <summary>
        /// Gets the <see cref="INotificationMessageService"/>
        /// </summary>
        internal INotificationMessageService NotificationMessageService
        {
            get { return _notificationMessageService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="INotificationMethodService"/>
        /// </summary>
        internal INotificationMethodService NotificationMethodService
        {
            get { return _notificationMethodService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IOfferRedeemedService"/>
        /// </summary>
        internal IOfferRedeemedService OfferRedeemedService
        {
            get
            {
                return _offerRedeemedService.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IPaymentMethodService"/>
        /// </summary>
        internal IPaymentMethodService PaymentMethodService
        {
            get { return _paymentMethodService.Value; }    
        }

        /// <summary>
        /// Gets the <see cref="IShipCountryService"/>
        /// </summary>
        internal IShipCountryService ShipCountryService
        {
            get { return _shipCountryService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IShipMethodService"/>
        /// </summary>
        internal IShipMethodService ShipMethodService
        {
            get { return _shipMethodService.Value; }
        }

        /// <summary>
        /// Gets the warehouse catalog service.
        /// </summary>
        internal IWarehouseCatalogService WarehouseCatalogService
        {
            get { return _warehouseCatalogService.Value; }
        }

        #endregion

        /// <summary>
        /// Builds the various services
        /// </summary>
        /// <param name="dbDatabaseUnitOfWorkProvider">
        /// Database unit of work provider used by the various services
        /// </param>
        /// <param name="repositoryFactory">
        /// The <see cref="RepositoryFactory"/>
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event Messages Factory.
        /// </param>
        private void BuildServiceContext(IDatabaseUnitOfWorkProvider dbDatabaseUnitOfWorkProvider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
        {
            if (_anonymousCustomerService == null)
                _anonymousCustomerService = new Lazy<IAnonymousCustomerService>(() => new AnonymousCustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_appliedPaymentService == null)
                _appliedPaymentService = new Lazy<IAppliedPaymentService>(() => new AppliedPaymentService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_auditLogService == null)
                _auditLogService = new Lazy<IAuditLogService>(() => new AuditLogService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_customerAddressService == null)
                _customerAddressService = new Lazy<ICustomerAddressService>(() => new CustomerAddressService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_customerService == null)
                _customerService = new Lazy<ICustomerService>(() => new CustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _anonymousCustomerService.Value, _customerAddressService.Value, _invoiceService.Value, _paymentService.Value));

            if (_detachedContentTypeService == null)
                _detachedContentTypeService = new Lazy<IDetachedContentTypeService>(() => new DetachedContentTypeService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_digitalMediaService == null)
                _digitalMediaService = new Lazy<IDigitalMediaService>(() => new DigitalMediaService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_entityCollectionService == null)
                _entityCollectionService = new Lazy<IEntityCollectionService>(() => new EntityCollectionService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_itemCacheService == null)
                _itemCacheService = new Lazy<IItemCacheService>(() => new ItemCacheService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_notificationMethodService == null)
                _notificationMethodService = new Lazy<INotificationMethodService>(() => new NotificationMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_notificationMessageService == null)
                _notificationMessageService = new Lazy<INotificationMessageService>(() => new NotificationMessageService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_offerSettingsService == null)
                _offerSettingsService = new Lazy<IOfferSettingsService>(() => new OfferSettingsService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_offerRedeemedService == null)
                _offerRedeemedService = new Lazy<IOfferRedeemedService>(() => new OfferRedeemedService(DatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_paymentService == null)
                _paymentService = new Lazy<IPaymentService>(() => new PaymentService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _appliedPaymentService.Value));

            if (_paymentMethodService == null)
                _paymentMethodService = new Lazy<IPaymentMethodService>(() => new PaymentMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_productVariantService == null)
                _productVariantService = new Lazy<IProductVariantService>(() => new ProductVariantService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_productService == null)
                _productService = new Lazy<IProductService>(() => new ProductService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _productVariantService.Value));

            if (_storeSettingsService == null)
                _storeSettingsService = new Lazy<IStoreSettingService>(() => new StoreSettingService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_shipCountryService == null)
                _shipCountryService = new Lazy<IShipCountryService>(() => new ShipCountryService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _storeSettingsService.Value));

            if (_shipMethodService == null)
                _shipMethodService = new Lazy<IShipMethodService>(() => new ShipMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_shipRateTierService == null)
                _shipRateTierService = new Lazy<IShipRateTierService>(() => new ShipRateTierService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));

            if (_shipmentService == null)
                _shipmentService = new Lazy<IShipmentService>(() => new ShipmentService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _storeSettingsService.Value));

            if (_orderService == null)
                _orderService = new Lazy<IOrderService>(() => new OrderService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _storeSettingsService.Value, _shipmentService.Value));

            if (_invoiceService == null)
                _invoiceService = new Lazy<IInvoiceService>(() => new InvoiceService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _appliedPaymentService.Value, _orderService.Value, _storeSettingsService.Value));

            if (_countryTaxRateService == null)
                _countryTaxRateService = new Lazy<ITaxMethodService>(() => new TaxMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _storeSettingsService.Value));

            if (_gatewayProviderService == null)
                _gatewayProviderService = new Lazy<IGatewayProviderService>(() => new GatewayProviderService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _shipMethodService.Value, _shipRateTierService.Value, _shipCountryService.Value, _invoiceService.Value, _orderService.Value, _countryTaxRateService.Value, _paymentService.Value, _paymentMethodService.Value, _notificationMethodService.Value, _notificationMessageService.Value, _warehouseService.Value));

            if (_warehouseCatalogService == null)
            {
                _warehouseCatalogService = new Lazy<IWarehouseCatalogService>(() => new WarehouseCatalogService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _productVariantService.Value));
            }

            if (_warehouseService == null)
                _warehouseService = new Lazy<IWarehouseService>(() => new WarehouseService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory, _warehouseCatalogService.Value));

            if (_notificationMessageService == null)
                _notificationMessageService = new Lazy<INotificationMessageService>(() => new NotificationMessageService(dbDatabaseUnitOfWorkProvider, repositoryFactory, logger, eventMessagesFactory));
        }
    }
}
