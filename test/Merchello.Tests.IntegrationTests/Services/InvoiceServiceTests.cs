using System;
using System.Collections.Generic;
using System.Linq;
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
    public class InvoiceServiceTests : BaseUsingSqlServerSyntax
    {

        private IInvoiceService _invoiceService;
        private ICustomer _customer;
        private IEnumerable<IInvoiceStatus> _statuses;
            
            
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

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

     
        }

        [Test]
        public void Can_Create_And_Save_An_Invoice()
        {
            var all = ((InvoiceService)_invoiceService).GetAll().ToArray();
            _invoiceService.Delete(all);

            var unpaid = _statuses.FirstOrDefault(x => x.Alias == "unpaid");

            var invoice = _invoiceService.CreateInvoice(_customer, unpaid, "test111", "name", "address1",
              "address2", "city", "state", "98225", "US", "test@test.com", string.Empty, string.Empty);

            _invoiceService.Save(invoice);

            Assert.IsTrue(invoice.Id > 0);

        }

        [Test]
        public void Can_Get_An_Invoice_By_Id()
        {
            var all = ((InvoiceService)_invoiceService).GetAll().ToArray();
            _invoiceService.Delete(all);

            var unpaid = _statuses.FirstOrDefault(x => x.Alias == "unpaid");

            var random = new Random(15000);

            var invoice = _invoiceService.CreateInvoice(_customer, unpaid, "test" + random.Next().ToString(), "name", "address1",
              "address2", "city", "state", "98225", "US", "test@test.com", string.Empty, string.Empty);

            _invoiceService.Save(invoice);

            var id = invoice.Id;

            var retrieved = _invoiceService.GetById(id);

            Assert.NotNull(retrieved);
        }

        [Test]
        public void Can_Delete_An_Invoice()
        {
            var all = ((InvoiceService)_invoiceService).GetAll().ToArray();
            _invoiceService.Delete(all);

            var unpaid = _statuses.FirstOrDefault(x => x.Alias == "unpaid");

            var random = new Random(120);

            var invoice = _invoiceService.CreateInvoice(_customer, unpaid, "test" + random.Next().ToString(), "name", "address1",
              "address2", "city", "state", "98225", "US", "test@test.com", string.Empty, string.Empty);

            _invoiceService.Save(invoice);

            var id = invoice.Id;

            _invoiceService.Delete(invoice);


            var retrieved = _invoiceService.GetById(id);

            Assert.IsNull(retrieved);
        }
    }
}
