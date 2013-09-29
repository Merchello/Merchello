using System;
using System.Data;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence
{
    /// <summary>
    /// Used to instantiate each repository type
    /// </summary>
    public class RepositoryFactory
    {
        private readonly bool _disableAllCache;

        public RepositoryFactory()
            : this(false)
        { }

        public RepositoryFactory(bool disableAllCache)
        {
            _disableAllCache = disableAllCache;
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerRepository"/>
        /// </summary>        
        internal virtual ICustomerRepository CreateCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAnonymousCustomerRepository"/>
        /// </summary>        
        internal virtual IAnonymousCustomerRepository CreateAnonymousCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new AnonymousCustomerRepository(uow,
                 _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAddressRepository"/>
        /// </summary>        
        internal virtual IAddressRepository CreateAddressRepository(IDatabaseUnitOfWork uow)
        {
            return new AddressRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerRegistryRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual ICustomerRegistryRepository CreateBasketRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerRegistryRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerRegistryItemRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual ICustomerRegistryItemRepository CreateBasketItemRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerRegistryItemRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }
        
        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceStatusRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IInvoiceStatusRepository CreateInvoiceStatusRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceStatusRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : InMemoryCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IInvoiceRepository CreateInvoiceRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceItemRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IInvoiceItemRepository CreateInvoiceItemRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceItemRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IPaymentRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IPaymentRepository CreatePaymentRepository(IDatabaseUnitOfWork uow)
        {
            return new PaymentRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAppliedPaymentRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IAppliedPaymentRepository CreateAppliedPaymentRepository(IDatabaseUnitOfWork uow)
        {
            return new AppliedPaymentRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }


        /// <summary>
        /// Returns an instance of the <see cref="IShipmentRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IShipmentRepository CreateShipmentRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipmentRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipMethodRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IShipMethodRepository CreateShipMethodRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipMethodRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : InMemoryCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IProductRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IProductRepository CreateProductRepository(IDatabaseUnitOfWork uow)
        {
            return new ProductRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IWarehouseRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IWarehouseRepository CreateWarehouseRepository(IDatabaseUnitOfWork uow)
        {
            return new WarehouseRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : InMemoryCacheProvider.Current);
        }

    }
}
