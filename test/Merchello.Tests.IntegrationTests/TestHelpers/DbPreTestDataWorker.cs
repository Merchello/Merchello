using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    /// <summary>
    /// Assist with integration tests which require data to be present in the database
    /// </summary>
    public class DbPreTestDataWorker
    {
        private Lazy<AddressService> _addressService;
        private Lazy<CustomerService> _customerService;
        private Lazy<AnonymousCustomerService> _anonymousCustomerService;
        private Lazy<BasketService> _basketService;
        private Lazy<InvoiceService> _invoiceService;
        private Lazy<InvoiceItemService> _invoiceItemService;
        private Lazy<InvoiceStatusService> _invoiceStatusService;
        private Lazy<PaymentService> _paymentService;
        private Lazy<ProductService> _productService;

        public DbPreTestDataWorker()
        {
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            _addressService = new Lazy<AddressService>(() => new AddressService());
            _customerService = new Lazy<CustomerService>(() => new CustomerService());
            _anonymousCustomerService = new Lazy<AnonymousCustomerService>(() => new AnonymousCustomerService(_customerService.Value));
            _basketService = new Lazy<BasketService>(() => new BasketService());
            _invoiceService = new Lazy<InvoiceService>(() => new InvoiceService());
            _invoiceItemService = new Lazy<InvoiceItemService>(() => new InvoiceItemService());
            _invoiceStatusService = new Lazy<InvoiceStatusService>(() => new InvoiceStatusService());
            _paymentService = new Lazy<PaymentService>(() => new PaymentService());
            _productService = new Lazy<ProductService>(() => new ProductService());
        }

        #region ICustomer

        public ICustomer ExistingCustomer()
        {
            var customer = CustomerData.CustomerForInserting();
            CustomerService.Save(customer);
            return customer;
        }

        public IEnumerable<ICustomer> CollectionExistingCustomers(int count)
        {
            var customers = new List<ICustomer>();
            for(var i =0; i < count; i++) customers.Add(CustomerData.CustomerForInserting());
            CustomerService.Save(customers);
            return customers;
        }

        public void DeleteAllCustomers()
        {
            var all = ((CustomerService) CustomerService).GetAll();
            CustomerService.Delete(all);
        }


        public ICustomerService CustomerService
        {
            get { return _customerService.Value; }
        }

        #endregion

        #region IInvoiceStatus

        public IEnumerable<IInvoiceStatus> DefaultInvoiceStatuses()
        {            
            var statuses = InvoiceStatusService.GetAll();
            if (!statuses.Any())
            {
                // populate the table
                var creation = new BaseDataCreation(new PetaPocoUnitOfWorkProvider().GetUnitOfWork().Database);
                creation.InitializeBaseData("merchInvoiceStatus");               
            }
            return InvoiceStatusService.GetAll();
        }

        public IInvoiceStatusService InvoiceStatusService
        {
            get { return _invoiceStatusService.Value; }
        }
        #endregion

        #region IInvoice

        #endregion

        #region IShipment
        #endregion

        

    }
}
