using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class InvoiceServiceTests : ServiceIntegrationTestBase
    {

        private IInvoiceService _invoiceService;
        private ICustomer _customer;
        private IEnumerable<IInvoiceStatus> _statuses;
        private IInvoiceStatus _unpaid;
            
        [SetUp]
        public void Initialize()
        {            
            _statuses = PreTestDataWorker.DefaultInvoiceStatuses();
            _customer = PreTestDataWorker.MakeExistingCustomer();
            _invoiceService = PreTestDataWorker.InvoiceService;
            _unpaid = _statuses.FirstOrDefault(x => x.Alias == "unpaid");
        }

        /// <summary>
        /// Test to verify that an invoice can be created and saved
        /// </summary>
        [Test]
        public void Can_Create_And_Save_An_Invoice()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllInvoices();
            const int expected = 1;

            //// Act
            var invoice = _invoiceService.CreateInvoice(_customer, _unpaid, "test111", "name", "address1",
              "address2", "city", "state", "98225", "US", "test@test.com", string.Empty, string.Empty);
            _invoiceService.Save(invoice);

            //// Assert
            var all = ((InvoiceService) _invoiceService).GetAll();
            Assert.IsTrue(invoice.Id > 0);
            Assert.AreEqual(expected, all.Count());
        }

        /// <summary>
        /// Test to verify an invoice can be retrieved by id
        /// </summary>
        [Test]
        public void Can_Get_An_Invoice_By_Id()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllInvoices();
            var address = PreTestDataWorker.MakeExistingAddress(_customer, "Home");
            var invoice = PreTestDataWorker.MakeExistingInvoice(_customer, _unpaid, address);
            var id = invoice.Id;

            //// Act            
            var retrieved = _invoiceService.GetById(id);

            //// Assert
            Assert.NotNull(retrieved);
        }


        /// <summary>
        /// Test to verify an invoice can be deleted
        /// </summary>
        [Test]
        public void Can_Delete_An_Invoice()
        {
            //// Arrange
            var address = PreTestDataWorker.MakeExistingAddress(_customer, "Somewhere");
            var invoice = PreTestDataWorker.MakeExistingInvoice(_customer, _unpaid, address);          
            var id = invoice.Id;

            //// Act
            _invoiceService.Delete(invoice);
            var retrieved = _invoiceService.GetById(id);

            //// Assert
            Assert.IsNull(retrieved);
        }

        /// <summary>
        /// Test to verify that a collection of invoices can be retrieved for a given customer
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Collection_Of_Invoices_By_Customer()
        {
            //// Arrange
            var count = 3;
            var address = PreTestDataWorker.MakeExistingAddress(_customer, "Home");
            var expected = PreTestDataWorker.MakeExistingInvoiceCollection(_customer, _unpaid, address, count);

            //// Act
            var retrieved = _invoiceService.GetInvoicesForCustomer(_customer.Key);

            //// Assert
            Assert.IsTrue(retrieved.Any());
            Assert.AreEqual(count, retrieved.Count());
        }

        /// <summary>
        /// Test to verify that a collection of invoices can be retrieved for a given invoice status
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Collection_Of_Invoices_By_InvoiceStatus()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllInvoices();
            var count = 10;
            var address = PreTestDataWorker.MakeExistingAddress(_customer, "Home");
            var expected = PreTestDataWorker.MakeExistingInvoiceCollection(_customer, _unpaid, address, count);

            //// Act
            var retrieved = _invoiceService.GetInvoicesByInvoiceStatus(_unpaid.Id);

            //// Assert
            Assert.IsTrue(retrieved.Any());
            Assert.AreEqual(count, retrieved.Count());
        }

    }
}
