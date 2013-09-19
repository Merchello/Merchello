using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    /// <summary>
    /// Assist with integration tests which require data to be present in the database
    /// </summary>
    public class DbPreTestDataCreator
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

        public DbPreTestDataCreator()
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

        //public ICustomer ExistingCustomer()
        //{
        //    var customer = CustomerData.CustomerForInserting();
        //}

        #endregion

    }
}
