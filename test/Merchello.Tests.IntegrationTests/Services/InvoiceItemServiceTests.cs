using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class InvoiceItemServiceTests : ServiceIntegrationTestBase
    {
        private IInvoiceService _invoiceService;
        private ICustomer _customer;
        private IEnumerable<IInvoiceStatus> _statuses;
        private IInvoiceItemService _invoiceItemService;
        private IInvoice _invoice;

        [SetUp]
        public void Initialize()
        {
            var invoiceStatusService = new InvoiceStatusService();
            _statuses = invoiceStatusService.GetAll();
            if (!_statuses.Any())
            {
                // populate the table
                var creation = new BaseDataCreation(new PetaPocoUnitOfWorkProvider().GetUnitOfWork().Database);
                creation.InitializeBaseData("merchInvoiceStatus");
                _statuses = invoiceStatusService.GetAll();
            }

            _customer = CustomerData.CustomerForInserting();
            var customerService = new CustomerService();

            customerService.Save(_customer);

            _invoiceService = new InvoiceService();

            var all = ((InvoiceService)_invoiceService).GetAll().ToArray();
            _invoiceService.Delete(all);

            var unpaid = _statuses.FirstOrDefault(x => x.Alias == "unpaid");

            _invoice = _invoiceService.CreateInvoice(_customer, unpaid, "test111", "name", "address1",
              "address2", "city", "state", "98225", "US", "test@test.com", string.Empty, string.Empty);

            _invoiceService.Save(_invoice);


            _invoiceItemService = new InvoiceItemService();
        }

        [Test]
        public void Can_Create_And_Save_An_InvoiceItem()
        {
            var invoiceItem = _invoiceItemService.CreateInvoiceItem(_invoice, InvoiceItemType.Product, "temp", "test", 1, 1, 100m);
            _invoiceItemService.Save(invoiceItem);

            Assert.IsTrue(invoiceItem.Id > 0);
        }

        [Test]
        public void Can_Retrieve_An_Invoice_Item()
        {
            var invoiceItem = _invoiceItemService.CreateInvoiceItem(_invoice, InvoiceItemType.Product, "temp", "test", 1, 1, 100m);
            _invoiceItemService.Save(invoiceItem);

            var id = invoiceItem.Id;
            var retrieved = _invoiceItemService.GetById(id);

            Assert.NotNull(retrieved);
        }

        [Test]
        public void Can_Delete_An_Invoice_Item()
        {
            var invoiceItem = _invoiceItemService.CreateInvoiceItem(_invoice, InvoiceItemType.Product, "temp", "test", 1, 1, 100m);
            _invoiceItemService.Save(invoiceItem);

            var id = invoiceItem.Id;
            _invoiceItemService.Delete(invoiceItem);
            var retrieved = _invoiceItemService.GetById(id);

            Assert.IsNull(retrieved);
        }


    }
}
