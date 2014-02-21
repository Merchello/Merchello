using Merchello.Core.Cache;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Repositories;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Umbraco.Core.Cache;


namespace Merchello.Core.Persistence
{
    /// <summary>
    /// Used to instantiate each repository type
    /// </summary>
    public class RepositoryFactory
    {
        private readonly bool _disableAllCache;
        private readonly IRuntimeCacheProvider _nullCacheProvider;
        private readonly IRuntimeCacheProvider _runtimeCacheProvider;

        public RepositoryFactory()
            : this(false, new NullCacheProvider(), new ObjectCacheRuntimeCacheProvider())
        { }

        public RepositoryFactory(bool disableAllCache, IRuntimeCacheProvider nullCacheProvider, IRuntimeCacheProvider runtimeCacheProvider)
        {
            _disableAllCache = disableAllCache;
            _nullCacheProvider = nullCacheProvider;
            _runtimeCacheProvider = runtimeCacheProvider;
        }

        /// <summary>
        /// Returns an instance of the <see cref="ITaxMethodRepository"/>
        /// </summary>
        internal virtual ITaxMethodRepository CreateTaxMethodRepository(IDatabaseUnitOfWork uow)
        {
            return new TaxMethodRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerRepository"/>
        /// </summary>        
        internal virtual ICustomerRepository CreateCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAnonymousCustomerRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IAnonymousCustomerRepository CreateAnonymousCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new AnonymousCustomerRepository(uow, _nullCacheProvider);                
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerAddressRepository"/>
        /// </summary>        
        internal virtual ICustomerAddressRepository CreateCustomerAddressRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerAddressRepository(uow, _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IItemCacheRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IItemCacheRepository CreateItemCacheRepository(IDatabaseUnitOfWork uow)
        {
            return new ItemCacheRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider,
                CreateLineItemRepository<ItemCacheItemDto>(uow));
        }

        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IInvoiceRepository CreateInvoiceRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider,
                CreateLineItemRepository<InvoiceItemDto>(uow)
                );
        }

        internal virtual IGatewayProviderRepository CreateGatewayProviderRepository(IDatabaseUnitOfWork uow)
        {
            return new GatewayProviderRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="ILineItemRepository"/>
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual ILineItemRepository CreateLineItemRepository<TDto>(IDatabaseUnitOfWork uow)
            where TDto : ILineItemDto
        {
            return new LineItemRepository<TDto>(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IPaymentRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IPaymentRepository CreatePaymentRepository(IDatabaseUnitOfWork uow)
        {
            return new PaymentRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IProductRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IProductRepository CreateProductRepository(IDatabaseUnitOfWork uow)
        {
            return new ProductRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider,
                CreateProductVariantRepository(uow));
        }

        /// <summary>
        /// Returns an instance of the <see cref="IProductVariantRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IProductVariantRepository CreateProductVariantRepository(IDatabaseUnitOfWork uow)
        {
            return new ProductVariantRepository(uow,
               _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipCountryRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="storeSettingService"></param>
        /// <returns></returns>
        internal virtual IShipCountryRepository CreateShipCountryRepository(IDatabaseUnitOfWork uow, IStoreSettingService storeSettingService)
        {
            return new ShipCountryRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider,
                storeSettingService);
        }


        /// <summary>
        /// Returns an instance of the <see cref="IShipMethodRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IShipMethodRepository CreateShipMethodRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipMethodRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipRateTierRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IShipRateTierRepository CreateShipRateTierRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipRateTierRepository(uow, _nullCacheProvider);
       
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipmentRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IShipmentRepository CreateShipmentRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipmentRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IStoreSettingRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IStoreSettingRepository CreateStoreSettingRepository(IDatabaseUnitOfWork uow)
        {
            return new StoreSettingRepository(uow,
                _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IWarehouseRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IWarehouseRepository CreateWarehouseRepository(IDatabaseUnitOfWork uow)
        {
            return new WarehouseRepository(uow,
               _disableAllCache ? _nullCacheProvider : _runtimeCacheProvider);
        }

    }
}
