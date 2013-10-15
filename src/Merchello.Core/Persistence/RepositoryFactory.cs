using System;
using System.Data;
using Merchello.Core.Models.Rdbms;
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
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IAnonymousCustomerRepository CreateAnonymousCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new AnonymousCustomerRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerAddressRepository"/>
        /// </summary>        
        internal virtual ICustomerAddressRepository CreateCustomerAddressRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerAddressRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerItemCacheRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual ICustomerItemCacheRepository CreateCustomerItemCacheRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerItemCacheRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : NullCacheProvider.Current,
                CreateLineItemRepository<CustomerItemCacheItemDto>(uow));
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
        /// Returns an instance of the <see cref="ILineItemRepository"/>
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual ILineItemRepository CreateLineItemRepository<TDto>(IDatabaseUnitOfWork uow)
            where TDto : ILineItemDto
        {
            return new LineItemRepository<TDto>(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : NullCacheProvider.Current);
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
        /// Returns an instance of the <see cref="IProductRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IProductRepository CreateProductRepository(IDatabaseUnitOfWork uow)
        {
            return new ProductRepository(uow,
                _disableAllCache ? (IRepositoryCacheProvider)NullCacheProvider.Current : RuntimeCacheProvider.Current, 
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
