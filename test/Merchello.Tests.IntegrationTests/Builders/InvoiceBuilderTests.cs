using System.Linq;
using Merchello.Core;
using Merchello.Core.Builders;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Builders
{

    [TestFixture]
    [Category("Builders")]
    public class InvoiceBuilderTests : BuilderTestBase
    {


        /// <summary>
        /// Test verifies that the InvoiceBuilder can be instantiated with 3 tasks from the configuration file
        /// </summary>
        [Test]
        public void Can_Create_The_Default_Invoice_Builder_With_4_Tasks()
        {
            //// Arrange
            //// Confirm Test change OK - this changes to 5 in 1.9.0 with the addition of coupon task
            const int taskCount = 6;

            //// Act
            var invoiceBuild = new InvoiceBuilderChain(SalePreparationMock);

            //// Assert
            Assert.NotNull(invoiceBuild);
            Assert.AreEqual(taskCount, invoiceBuild.TaskCount);
        }

        /// <summary>
        /// Test confirms that an address can be added to an invoice
        /// </summary>
        [Test]
        public void Can_Add_An_Address_To_An_Invoice()
        {
            //// Arrange
            var expected = BillingAddress;
            var invoiceBuilder = new InvoiceBuilderChain(SalePreparationMock);

            //// Arrange
            var attempt = invoiceBuilder.Build();
            Assert.IsTrue(attempt.Success);
            var invoice = attempt.Result;

            //// Assert
            Assert.NotNull(invoice);
            Assert.IsTrue(((Address)expected).Equals(invoice.GetBillingAddress()));
        }

        /// <summary>
        /// Test verifies that an invoice contains product and shipment line items
        /// </summary>
        [Test]
        public void Can_Verify_Invoice_Contains_Product_And_Shipment_LineItems()
        {
            //// Arrange
            const decimal expectedProducts = ProductCount;
            const int expectedShipments = 1;
            var invoiceBuilder = new InvoiceBuilderChain(SalePreparationMock);

            //// Act
            var attempt = invoiceBuilder.Build();
            Assert.IsTrue(attempt.Success);
            var invoice = attempt.Result;

            //// Assert
            Assert.NotNull(invoice, "Invoice is null");
            Assert.IsTrue(invoice.Items.Any(), "Invoice does not have any items");
            Assert.AreEqual(expectedProducts, invoice.Items.Count(x => x.LineItemType == LineItemType.Product), "Count of products does not match expected");
            Assert.AreEqual(expectedShipments, invoice.Items.Count(x => x.LineItemType == LineItemType.Shipping), "Count of shipments does not match expected");
            
        }

    }
}