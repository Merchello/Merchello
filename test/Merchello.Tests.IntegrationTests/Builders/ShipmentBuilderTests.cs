using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Builders;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Builders
{

    [TestFixture]
    [Category("Builders")]
    public class ShipmentBuilderTests : BuilderTestBase
    {
        private IOrder _order;

        [SetUp]
        public override void Init()
        {
            base.Init();

            PreTestDataWorker.DeleteAllOrders();
            PreTestDataWorker.DeleteAllShipments();
            var invoice = SalePreparationMock.PrepareInvoice();
            PreTestDataWorker.InvoiceService.Save(invoice);
            _order = invoice.PrepareOrder(MerchelloContext);
            PreTestDataWorker.OrderService.Save(_order);

        }

        /// <summary>
        /// Test verifies that the ShipmentBuilder can be instantiated with 3 tasks from the configuration file
        /// </summary>
        [Test]
        public void Can_Create_The_Default_Shipment_Builder_With_Tasks()
        {
            //// Arrange
            const int taskCount = 3;

            //// Act
            var builder = new ShipmentBuilderChain(MerchelloContext, _order, _order.Items.Select(x => x.Key));

            //// Assert
            Assert.NotNull(builder);
            Assert.AreEqual(taskCount, builder.TaskCount);
        }

        /// <summary>
        /// Test confirms that the shipmentbuilder creates and saves a shipment and updates the order line items with the shipmentKey
        /// </summary>
        [Test]
        public void ShipmentBuilder_Creates_And_Saves_A_Shipment_And_OrderLineItems_Are_Updated()
        {
            //// Arrange
            var builder = new ShipmentBuilderChain(MerchelloContext, _order, _order.Items.Select(x => x.Key));

            //// Act
            var attempt = builder.Build();
            if (!attempt.Success) Console.WriteLine(attempt.Exception.Message);

            //// Assert
            Assert.IsTrue(attempt.Success);
        }

        [Test]
        public void Can_Save_A_Shipment_After_Its_Built()
        {
            //// Arrange
            var shipmentService = MerchelloContext.Services.ShipmentService;
            var builder = new ShipmentBuilderChain(MerchelloContext, _order, _order.Items.Select(x => x.Key));
            var attempt = builder.Build();
            Assert.IsTrue(attempt.Success, "Failed to build shipment");

            //// Act
            var shipment = attempt.Result;
            shipmentService.Save(shipment);

            //// Assert
            Assert.IsTrue(shipment.HasIdentity);

        }
    }
}