using System;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.UnitOfWork;


namespace Merchello.Core.Services
{
    /// <summary>
    /// The Merchello ServiceContext, which provides access to the following services:
    /// <see cref="ICustomerService"/>, <see cref="IItemCacheService"/>, <see cref="IGatewayProviderService"/>, <see cref="IProductService"/>, 
    /// <see cref="IProductVariantService"/>, <see cref="IStoreSettingService"/>, <see cref="IShipCountryService"/>, <see cref="IShipMethodService"/>
    /// and <see cref="IWarehouseService"/>
    /// </summary>
    public class ServiceContext : IServiceContext
    {
        private Lazy<IAppliedPaymentService> _appliedPaymentService;
        private Lazy<ITaxMethodService> _countryTaxRateService; 
        private Lazy<ICustomerService> _customerService;
        private Lazy<IInvoiceService> _invoiceService; 
        private Lazy<IItemCacheService> _itemCacheService;   
        private Lazy<IGatewayProviderService> _gatewayProviderService;
        private Lazy<IOrderService> _orderService; 
        private Lazy<IPaymentService> _paymentService; 
        private Lazy<IPaymentMethodService> _paymentMethodService; 
        private Lazy<IProductService> _productService;
        private Lazy<IProductVariantService> _productVariantService;
        private Lazy<IStoreSettingService> _storeSettingsService;
        private Lazy<IShipCountryService> _shipCountryService;
        private Lazy<IShipMethodService> _shipMethodService; 
        private Lazy<IShipRateTierService> _shipRateTierService; 
        private Lazy<IShipmentService> _shipmentService; 
        private Lazy<IWarehouseService> _warehouseService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbUnitOfWorkProvider"></param>
        internal ServiceContext(IDatabaseUnitOfWorkProvider dbUnitOfWorkProvider)
        {
            BuildServiceContext(dbUnitOfWorkProvider, new Lazy<RepositoryFactory>(() => new RepositoryFactory()));
        }

        /// <summary>
        /// Builds the various services
        /// </summary>
        /// <param name="dbDatabaseUnitOfWorkProvider">Database unit of work provider used by the various services</param>
        /// <param name="repositoryFactory"><see cref="RepositoryFactory"/></param>
        private void BuildServiceContext(IDatabaseUnitOfWorkProvider dbDatabaseUnitOfWorkProvider,
            Lazy<RepositoryFactory> repositoryFactory)
        {
            if(_appliedPaymentService == null)
                _appliedPaymentService = new Lazy<IAppliedPaymentService>(() => new AppliedPaymentService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_customerService == null)
                _customerService = new Lazy<ICustomerService>(() => new CustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_itemCacheService == null)
                _itemCacheService = new Lazy<IItemCacheService>(() => new ItemCacheService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));
            
            
            if(_paymentService == null)
                _paymentService = new Lazy<IPaymentService>(() => new PaymentService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _appliedPaymentService.Value));

            if(_paymentMethodService == null)
                _paymentMethodService = new Lazy<IPaymentMethodService>(() => new PaymentMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_productVariantService == null)
                _productVariantService = new Lazy<IProductVariantService>(() => new ProductVariantService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_productService == null)
                _productService = new Lazy<IProductService>(() => new ProductService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _productVariantService.Value));
            
            if(_storeSettingsService == null)
                _storeSettingsService = new Lazy<IStoreSettingService>(() => new StoreSettingService());

            if(_shipCountryService == null)
                _shipCountryService = new Lazy<IShipCountryService>(() => new ShipCountryService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _storeSettingsService.Value));

            if(_shipMethodService == null)
                _shipMethodService = new Lazy<IShipMethodService>(() => new ShipMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_shipRateTierService == null)
                _shipRateTierService = new Lazy<IShipRateTierService>(() => new ShipRateTierService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_shipmentService == null)
                _shipmentService = new Lazy<IShipmentService>(() => new ShipmentService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if (_orderService == null)
                _orderService = new Lazy<IOrderService>(() => new OrderService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _storeSettingsService.Value, _shipmentService.Value));


            if (_invoiceService == null)
                _invoiceService = new Lazy<IInvoiceService>(() => new InvoiceService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _appliedPaymentService.Value, _orderService.Value, _storeSettingsService.Value));

            if (_countryTaxRateService == null)
                _countryTaxRateService = new Lazy<ITaxMethodService>(() => new TaxMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _storeSettingsService.Value));
            
            if(_gatewayProviderService == null)
                _gatewayProviderService = new Lazy<IGatewayProviderService>(() => new GatewayProviderService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _shipMethodService.Value, _shipRateTierService.Value, _shipCountryService.Value, _invoiceService.Value, _orderService.Value, _countryTaxRateService.Value, _paymentService.Value, _paymentMethodService.Value));

            if(_warehouseService == null)
                _warehouseService = new Lazy<IWarehouseService>(() => new WarehouseService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));
        }


        #region IServiceContext Members

        /// <summary>
        /// Gets the <see cref="ITaxMethodService"/>
        /// </summary>
        internal ITaxMethodService TaxMethodService
        {
            get { return _countryTaxRateService.Value; }
        }
    
        /// <summary>
        /// Gets the <see cref="ICustomerService"/>
        /// </summary>
        public ICustomerService CustomerService
        {
            get { return _customerService.Value;  }
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
        /// Gets the <see cref="IPaymentService"/>
        /// </summary>
        public IPaymentService PaymentService
        {
            get { return _paymentService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IPaymentMethodService"/>
        /// </summary>
        internal IPaymentMethodService PaymentMethodService
        {
            get { return _paymentMethodService.Value; }    
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
     
        #endregion
    }
}
