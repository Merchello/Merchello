using Merchello.Tests.Base.TestHelpers;

namespace Merchello.Tests.IntegrationTests.Invoices
{
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.IntegrationTests.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class InvoiceTests : MerchelloAllInTestBase
    {
        /// <summary>
        /// Test verifies that shipment line items stores extended data values
        /// </summary>
        [Test]
        public void Can_Prove_Serialized_Shipment_Stores_ExtendedDataCollection_In_LineItems()
        {
            //// Arrange
            var invoice = MockInvoiceDataMaker.GetMockInvoiceForTaxation();
            Assert.NotNull(invoice, "Invoice is null");

            //// Act
            var shipmentLineItems = invoice.ShippingLineItems().ToArray();
            Assert.IsTrue(shipmentLineItems.Any());
            var shipment = shipmentLineItems.First().ExtendedData.GetShipment<OrderLineItem>();

            //// Assert
            Assert.NotNull(shipment, "Shipment was null");
            Assert.IsTrue(shipment.Items.Any(), "Shipment did not contain any line items");
            Assert.IsFalse(shipment.Items.All(x => x.ExtendedData.IsEmpty), "Extended data in one or more line items was empty");
            Assert.IsTrue(shipment.Items.Any(x => x.ExtendedData.GetTaxableValue()), "Shipment does not contain any taxable items");

            foreach (var item in shipment.Items)
            {
                Assert.IsTrue(invoice.Items.Any(x => x.Sku == item.Sku), "No item exists for sku " + item.Sku);
            }
        } 
    }
}