using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class InvoiceItemServiceTests : ServiceIntegrationTestBase
    {
        private ICustomer _customer;
        private IEnumerable<IInvoiceStatus> _statuses;
        private IInvoiceItemService _invoiceItemService;
        private IInvoice _invoice;

        [SetUp]
        public void Initialize()
        {
            _statuses = PreTestDataWorker.DefaultInvoiceStatuses();
            _customer = PreTestDataWorker.MakeExistingCustomer();
            
            var unpaid = _statuses.FirstOrDefault(x => x.Alias == "unpaid");
            var address = PreTestDataWorker.MakeExistingAddress(_customer, "Billing");

            PreTestDataWorker.DeleteAllInvoices();
            _invoice = PreTestDataWorker.MakeExistingInvoice(_customer, unpaid, address);
           
            _invoiceItemService = PreTestDataWorker.InvoiceItemService;
        }

        /// <summary>
        /// Test to verify an invoice can be created and saved
        /// </summary>
        [Test]
        public void Can_Create_And_Save_An_InvoiceItem()
        {
            //// Arrange
            var id = 0;

            //// Act
            var invoiceItem = _invoiceItemService.CreateInvoiceItem(_invoice, InvoiceItemType.Product, "temp", "test", 1, 1, 100m);
            _invoiceItemService.Save(invoiceItem);

            //// Assert
            Assert.IsTrue(id != invoiceItem.Id);
            Assert.IsTrue(invoiceItem.HasIdentity);
        }


        /// <summary>
        /// Test to verify that an invoice item can be retrieved from the database
        /// </summary>
        [Test]
        public void Can_Retrieve_An_Invoice_Item()
        {
            //// Arrange
            var expected = PreTestDataWorker.MakeExistingInvoiceItem(_invoice, InvoiceItemType.Product);
            var id = expected.Id;

            //// Act
            var retrieved = _invoiceItemService.GetById(id);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual(expected.Id, retrieved.Id);
        }

        /// <summary>
        /// Test to verify that a collection of invoice items can be saved
        /// </summary>
        [Test]
        public void Can_Save_A_Collection_Of_InvoiceItems()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllInvoiceItems();
            var invoiceItems = MockInvoiceItemDataMaker.InvoiceItemCollectionForInserting(_invoice, InvoiceItemType.Product, 10);
            var expected = 10;

            //// Act
            _invoiceItemService.Save(invoiceItems);

            //// Assert
            var all = ((InvoiceItemService) _invoiceItemService).GetAll();
            Assert.IsTrue(all.Any());
            Assert.AreEqual(10, all.Count());
        }

        /// <summary>
        /// Test to verify that an invoice item can be deleted
        /// </summary>
        [Test]
        public void Can_Delete_An_Invoice_Item()
        {
            //// Arrange
            var invoiceItem = PreTestDataWorker.MakeExistingInvoiceItem(_invoice, InvoiceItemType.Product);
            var id = invoiceItem.Id;

            //// Act
            _invoiceItemService.Delete(invoiceItem);

            //// Assert
            var retrieved = _invoiceItemService.GetById(id);
            Assert.IsNull(retrieved);
        }

        /// <summary>
        /// Test to verify that invoice items can be retrieved for a given invoice
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Collection_Of_InvoiceItems_By_Invoice() 
        {
            //// Arrange
            var expected = 12;
            var invoiceItems = PreTestDataWorker.MakeExistingInvoiceItemCollection(_invoice, InvoiceItemType.Product, expected);

            //// Act
            var items = _invoiceItemService.GetInvoiceItemsForInvoice(_invoice.Id);

            //// Assert
            Assert.IsTrue(items.Any());
            Assert.AreEqual(expected, items.Count());
        }
    }
}
