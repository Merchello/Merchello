using System;
using Merchello.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// The Merchello ServiceContext, which provides access to the following services:
    /// <see cref="ICustomerService"/>
    /// </summary>
    public class ServiceContext
    {
        private Lazy<CustomerService> _customerService;
        private Lazy<AnonymousCustomerService> _anonymousCustomerService;
        private Lazy<BasketService> _basketService;
        private Lazy<InvoiceService> _invoiceService;
        private Lazy<InvoiceItemService> _invoiceItemService;
        private Lazy<InvoiceStatusService> _invoiceStatusService; 
        
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

            if (_anonymousCustomerService == null)
                _anonymousCustomerService = new Lazy<AnonymousCustomerService>(() => new AnonymousCustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _customerService.Value));

            if(_basketService == null)
                _basketService = new Lazy<BasketService>(() => new BasketService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_invoiceItemService == null)
                _invoiceItemService = new Lazy<InvoiceItemService>(() => new InvoiceItemService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_invoiceStatusService == null)
                _invoiceStatusService = new Lazy<InvoiceStatusService>(() => new InvoiceStatusService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if(_invoiceService == null)
                _invoiceService = new Lazy<InvoiceService>(() => new InvoiceService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _invoiceItemService.Value, _invoiceStatusService.Value));
        }


        /// <summary>
        /// Gets the <see cref="ICustomerService"/>
        /// </summary>
        public ICustomerService CustomerService
        {
            get { return _customerService.Value;  }
        }

        /// <summary>
        /// Gets the <see cref="BasketService"/>
        /// </summary>
        public IBasketService BasketService
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
    }
}
