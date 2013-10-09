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
        private Lazy<CustomerService> _customerService;
        private Lazy<CustomerItemCacheService> _basketService;    
        private Lazy<InvoiceService> _invoiceService;
        private Lazy<ProductService> _productService;
        private Lazy<ProductVariantService> _productVariantService;
        private Lazy<ShippingService> _shipmentService;        
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

            if(_basketService == null)
                _basketService = new Lazy<CustomerItemCacheService>(() => new CustomerItemCacheService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));


            if(_invoiceService == null)
                _invoiceService = new Lazy<InvoiceService>(() => new InvoiceService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            //if (_appliedPaymentService == null)
            //    _appliedPaymentService = new Lazy<AppliedPaymentService>(() => new AppliedPaymentService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            //if (_applyPaymentStrategy == null)
            //{
            //    // instantiate type configured in Merchello configuration section
            //    var paymentStrategyTypeName = MerchelloConfiguration.Current.DefaultApplyPaymentStrategy;

            //    // we have to find the ApplyPaymentStrategyBase with a specific constructor
            //    var constructorArgs = new[] { typeof(CustomerService), typeof(InvoiceService), typeof(AppliedPaymentService) };
            //    var constructorArgValues = new object[] { _customerService.Value, _invoiceService.Value, _appliedPaymentService.Value };
                
            //    _applyPaymentStrategy = new Lazy<PaymentApplicationStrategyBase>(() => ActivatorHelper.CreateInstance<PaymentApplicationStrategyBase>(Type.GetType(paymentStrategyTypeName), constructorArgs, constructorArgValues));
            //}

            //if(_paymentService == null)
            //    _paymentService = new Lazy<PaymentService>(() => new PaymentService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _applyPaymentStrategy.Value));

            if(_productVariantService == null)
                _productVariantService = new Lazy<ProductVariantService>(() => new ProductVariantService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_productService == null)
                _productService = new Lazy<ProductService>(() => new ProductService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _productVariantService.Value));

            if(_shipmentService == null)
                _shipmentService = new Lazy<ShippingService>(() => new ShippingService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));


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
        /// Gets the <see cref="ICustomerItemCacheService"/>
        /// </summary>
        public ICustomerItemCacheService CustomerItemCacheService
        {
            get { return _basketService.Value;  }
        }

        /// <summary>
        /// Gets the <see cref="IInvoiceService"/>
        /// </summary>
        public IInvoiceService InvoiceService
        {
            get { return _invoiceService.Value; }
        }
    
        /// <summary>
        /// Gets the <see cref="IProductService"/>
        /// </summary>
        public IProductService ProductService
        {
            get { return _productService.Value;  }
        }

        /// <summary>
        /// Gets the <see cref="IProductVariantService"/>
        /// </summary>
        public IProductVariantService ProductVariantService
        {
            get { return _productVariantService.Value; }
        }

        public IShippingService ShippingService
        {
            get { return _shipmentService.Value; }
        }        

        public IWarehouseService WarehouseService
        {
            get { return _warehouseService.Value; }

        }
     
        #endregion
    }
}
