using System;
using Merchello.Core.Models;
using Merchello.Core.Services;
using NUnit.Framework;
namespace Merchello.Tests.IntegrationTests.Services.Fulfillment
{
    using System.Linq;

    using ClientDependency.Core;

    using Merchello.Core.Persistence.Querying;

    [TestFixture]
    [Category("Service Integration")]
    public class InvoiceServiceTests : FulfillmentTestsBase
    {
        private IInvoiceService _invoiceService;



        [SetUp]
        public void Init()
        {
            _invoiceService = PreTestDataWorker.InvoiceService;
        }

        /// <summary>
        /// Test confirms that a new invoice can be created without being persisted
        /// </summary>
        [Test]
        public void Can_Create_An_Invoice()
        {
            //// Arrange
            
  
            //// Act
            var invoice = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            //// Assert
            Assert.NotNull(invoice);
            Assert.AreEqual(0, invoice.InvoiceNumber);
            Assert.IsFalse(((Invoice)invoice).HasIdentity);
        }
        
        /// <summary>
        /// Test confirms that an invoice can be Saved and an Invoice Number is generated
        /// </summary>
        [Test]
        public void Can_Save_An_Invoice_With_InvoiceNumber()
        {
            //// Arrange
            var invoice = _invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            Console.Write(invoice.InvoiceStatusKey);

            //// Act
            _invoiceService.Save(invoice);

            //// Assert
            Assert.NotNull(invoice);
            Assert.AreNotEqual(0, invoice.InvoiceNumber);
            Assert.IsTrue(((Invoice)invoice).HasIdentity);

        }

        public void Can_Get_Paged_Results_From_Query()
        {
            //// Arrange
            var statuses = new[] { Core.Constants.DefaultKeys.InvoiceStatus.Paid, Core.Constants.DefaultKeys.InvoiceStatus.Unpaid };

            //// Act
            var page = ((InvoiceService)_invoiceService).GetPagedKeys("5 10 17 space nee", 1, 25);
            //var page = ((InvoiceService)_invoiceService).GetPageByInvoiceDateRange(DateTime.Parse("1/1/2014"), DateTime.Now, 1, 25);

            //// Assert
            Assert.IsTrue(page.Items.Any());
            Console.WriteLine(page.TotalItems);
            Console.WriteLine(page.Items.Count);
        }
    }
}