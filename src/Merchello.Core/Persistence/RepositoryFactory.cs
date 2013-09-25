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

        /// <summary>
        /// Returns an instance of the <see cref="ICustomerRepository"/>
        /// </summary>        
        internal virtual ICustomerRepository CreateCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAnonymousCustomerRepository"/>
        /// </summary>        
        internal virtual IAnonymousCustomerRepository CreateAnonymousCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new AnonymousCustomerRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAddressRepository"/>
        /// </summary>        
        internal virtual IAddressRepository CreateAddressRepository(IDatabaseUnitOfWork uow)
        {
            return new AddressRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IBasketRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IBasketRepository CreateBasketRepository(IDatabaseUnitOfWork uow)
        {
            return new BasketRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IBasketItemRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IBasketItemRepository CreateBasketItemRepository(IDatabaseUnitOfWork uow)
        {
            return new BasketItemRepository(uow, NullCacheProvider.Current);
        }
        
        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceStatusRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IInvoiceStatusRepository CreateInvoiceStatusRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceStatusRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IInvoiceRepository CreateInvoiceRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IInvoiceItemRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IInvoiceItemRepository CreateInvoiceItemRepository(IDatabaseUnitOfWork uow)
        {
            return new InvoiceItemRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IPaymentRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IPaymentRepository CreatePaymentRepository(IDatabaseUnitOfWork uow)
        {
            return new PaymentRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IAppliedPaymentRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IAppliedPaymentRepository CreateAppliedPaymentRepository(IDatabaseUnitOfWork uow)
        {
            return new AppliedPaymentRepository(uow, NullCacheProvider.Current);
        }


        /// <summary>
        /// Returns an instance of the <see cref="IShipmentRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IShipmentRepository CreateShipmentRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipmentRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IShipMethodRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IShipMethodRepository CreateShipMethodRepository(IDatabaseUnitOfWork uow)
        {
            return new ShipMethodRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IProductRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IProductRepository CreateProductRepository(IDatabaseUnitOfWork uow)
        {
            return new ProductRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns an instance of the <see cref="IWarehouseRepository"/>
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        internal virtual IWarehouseRepository CreateWarehouseRepository(IDatabaseUnitOfWork uow)
        {
            return new WarehouseRepository(uow, NullCacheProvider.Current);
        }

    }
}
