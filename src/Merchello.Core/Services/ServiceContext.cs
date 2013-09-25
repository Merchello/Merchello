using System;
using System.ComponentModel;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Strategies.Payment;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// The Merchello ServiceContext, which provides access to the following services:
    /// <see cref="ICustomerService"/>
    /// </summary>
    public class ServiceContext : IServiceContext
    {
        private Lazy<AddressService> _addressService;
        private Lazy<CustomerService> _customerService;
        private Lazy<AnonymousCustomerService> _anonymousCustomerService;
        private Lazy<BasketService> _basketService;
        private Lazy<BasketItemService> _basketItemService; 
        private Lazy<InvoiceService> _invoiceService;
        private Lazy<InvoiceItemService> _invoiceItemService;
        private Lazy<InvoiceStatusService> _invoiceStatusService;
        private Lazy<PaymentService> _paymentService;
        private Lazy<ProductService> _productService;
        private Lazy<ShipmentService> _shipmentService;
        private Lazy<ShipMethodService> _shipMethodService;
        private Lazy<TransactionService> _transactionService;
        private Lazy<WarehouseService> _warehouseService;

        private Lazy<PaymentApplicationStrategyBase> _applyPaymentStrategy;
        
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
            
            if(_addressService == null)
                _addressService = new Lazy<AddressService>(() => new AddressService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_customerService == null)
                _customerService = new Lazy<CustomerService>(() => new CustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if (_anonymousCustomerService == null)
                _anonymousCustomerService = new Lazy<AnonymousCustomerService>(() => new AnonymousCustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _customerService.Value));

            if(_basketService == null)
                _basketService = new Lazy<BasketService>(() => new BasketService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_basketItemService == null)
                _basketItemService = new Lazy<BasketItemService>(() => new BasketItemService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_invoiceItemService == null)
                _invoiceItemService = new Lazy<InvoiceItemService>(() => new InvoiceItemService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_invoiceStatusService == null)
                _invoiceStatusService = new Lazy<InvoiceStatusService>(() => new InvoiceStatusService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_invoiceService == null)
                _invoiceService = new Lazy<InvoiceService>(() => new InvoiceService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _invoiceItemService.Value, _invoiceStatusService.Value));

            if (_transactionService == null)
                _transactionService = new Lazy<TransactionService>(() => new TransactionService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if (_applyPaymentStrategy == null)
            {
                // instantiate type configured in Merchello configuration section
                var paymentStrategyTypeName = MerchelloConfiguration.Current.DefaultApplyPaymentStrategy;

                // we have to find the ApplyPaymentStrategyBase with a specific constructor
                var constructorArgs = new[] { typeof(CustomerService), typeof(InvoiceService), typeof(TransactionService) };
                var constructorArgValues = new object[] { _customerService.Value, _invoiceService.Value, _transactionService.Value };
                
                _applyPaymentStrategy = new Lazy<PaymentApplicationStrategyBase>(() => ActivatorHelper.CreateInstance<PaymentApplicationStrategyBase>(Type.GetType(paymentStrategyTypeName), constructorArgs, constructorArgValues));
            }

            if(_paymentService == null)
                _paymentService = new Lazy<PaymentService>(() => new PaymentService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _applyPaymentStrategy.Value));

            if(_productService == null)
                _productService = new Lazy<ProductService>(() => new ProductService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_shipmentService == null)
                _shipmentService = new Lazy<ShipmentService>(() => new ShipmentService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if (_shipMethodService == null)
                _shipMethodService = new Lazy<ShipMethodService>(() => new ShipMethodService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_warehouseService == null)
                _warehouseService = new Lazy<WarehouseService>(() => new WarehouseService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));
        }


        #region IServiceContext Members

        /// <summary>
        /// Gets the <see cref="IAddressService"/>
        /// </summary>
        internal IAddressService AddressService
        {
            get { return _addressService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IAnonymousCustomerService"/>
        /// </summary>
        internal IAnonymousCustomerService AnonymousCustomerService
        {
            get { return _anonymousCustomerService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ICustomerService"/>
        /// </summary>
        public ICustomerService CustomerService
        {
            get { return _customerService.Value;  }
        }

        /// <summary>
        /// Gets the <see cref="IBasketService"/>
        /// </summary>
        public IBasketService BasketService
        {
            get { return _basketService.Value;  }
        }

        /// <summary>
        /// Gets the <see cref="IBasketItemService"/>
        /// </summary>
        internal IBasketItemService BasketItemService
        {
            get { return _basketItemService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IInvoiceService"/>
        /// </summary>
        public IInvoiceService InvoiceService
        {
            get { return _invoiceService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IInvoiceItemService"/>
        /// </summary>
        internal IInvoiceItemService InvoiceItemService
        {
            get { return _invoiceItemService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IInvoiceStatusService"/>
        /// </summary>
        internal IInvoiceStatusService InvoiceStatusService
        {
            get { return _invoiceStatusService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IPaymentService"/>
        /// </summary>
        public IPaymentService PaymentService
        {
            get { return _paymentService.Value;  }
        }

        /// <summary>
        /// Gets the <see cref="IProductService"/>
        /// </summary>
        public IProductService ProductService
        {
            get { return _productService.Value;  }
        }

        public IShipmentService ShipmentService
        {
            get { return _shipmentService.Value; }
        }

        internal IShipMethodService ShipMethodService
        {
            get { return _shipMethodService.Value; }
        }

        internal  ITransactionService TransactionService
        {
            get { return _transactionService.Value; }
        }

        public IWarehouseService WarehouseService
        {
            get { return _warehouseService.Value; }

        }

        

        #endregion
    }
}
