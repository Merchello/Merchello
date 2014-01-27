using System;
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
        private Lazy<CountryTaxRateService> _countryTaxRateService; 
        private Lazy<CustomerService> _customerService;
        private Lazy<ItemCacheService> _itemCacheService;   
        private Lazy<GatewayProviderService> _gatewayProviderService ;  
        private Lazy<ProductService> _productService;
        private Lazy<ProductVariantService> _productVariantService;
        private Lazy<StoreSettingService> _storeSettingsService;
        private Lazy<ShipCountryService> _shipCountryService;
        private Lazy<ShipMethodService> _shipMethodService;
        private Lazy<IShipRateTierService> _shipRateTierService; 
        private Lazy<ShipmentService> _shipmentService; 
        private Lazy<WarehouseService> _warehouseService;
        
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

            if(_customerService == null)
                _customerService = new Lazy<CustomerService>(() => new CustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_itemCacheService == null)
                _itemCacheService = new Lazy<ItemCacheService>(() => new ItemCacheService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));
            
            if(_productVariantService == null)
                _productVariantService = new Lazy<ProductVariantService>(() => new ProductVariantService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_productService == null)
                _productService = new Lazy<ProductService>(() => new ProductService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _productVariantService.Value));
            
            if(_storeSettingsService == null)
                _storeSettingsService = new Lazy<StoreSettingService>(() => new StoreSettingService());

            if(_shipCountryService == null)
                _shipCountryService = new Lazy<ShipCountryService>(() => new ShipCountryService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _storeSettingsService.Value));

            if(_shipMethodService == null)
                _shipMethodService = new Lazy<ShipMethodService>(() => new ShipMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_shipRateTierService == null)
                _shipRateTierService = new Lazy<IShipRateTierService>(() => new ShipRateTierService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_shipmentService == null)
                _shipmentService = new Lazy<ShipmentService>(() => new ShipmentService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if (_countryTaxRateService == null)
                _countryTaxRateService = new Lazy<CountryTaxRateService>(() => new CountryTaxRateService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _storeSettingsService.Value));
            
            if(_gatewayProviderService == null)
                _gatewayProviderService = new Lazy<GatewayProviderService>(() => new GatewayProviderService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _shipMethodService.Value, _shipRateTierService.Value, _shipCountryService.Value, _countryTaxRateService.Value));

            if(_warehouseService == null)
                _warehouseService = new Lazy<WarehouseService>(() => new WarehouseService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));
        }


        #region IServiceContext Members
    
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
        /// Gets the <see cref="IItemCacheService"/>
        /// </summary>
        public IItemCacheService ItemCacheService
        {
            get { return _itemCacheService.Value;  }
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
