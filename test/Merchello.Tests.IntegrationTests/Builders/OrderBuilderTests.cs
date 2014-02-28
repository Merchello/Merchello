using System;
using System.Linq;
using Merchello.Core.Builders;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Builders
{
    [TestFixture]
    [Category("Builders")]
    public class OrderBuilderTests : BuilderTestBase
    {
        /// <summary>
        /// Test verifies that the OrderBuilder can be instantiated with 3 tasks from the configuration file
        /// </summary>
        [Test]
        public void Can_Create_The_Default_Order_Builder_With_Tasks()
        {
            //// Arrange
            const int taskCount = 1;
            var invoice = SalePreparationMock.PrepareInvoice();
            PreTestDataWorker.InvoiceService.Save(invoice);

            //// Act
            var orderBuilder = new OrderBuilderChain(invoice);

            //// Assert
            Assert.NotNull(orderBuilder);
            Assert.AreEqual(taskCount, orderBuilder.TaskCount);
        }

        /// <summary>
        /// Test verifies that invoice items can be transfered to order items
        /// </summary>
        [Test]
        public void Can_Transfer_InvoiceItems_To_OrderItems()
        {
            //// Arrange
            var invoice = SalePreparationMock.PrepareInvoice();
            invoice.VersionKey = new Guid();
            PreTestDataWorker.InvoiceService.Save(invoice);

            //// Act
            var orderBuilder = new OrderBuilderChain(invoice);
            var attempt = orderBuilder.Build();
            Assert.IsTrue(attempt.Success, "The order builder failed to create an order");
            var order = attempt.Result;

            //// Assert
            Assert.IsTrue(order.Items.Any(), "The order does not contain any items");

            foreach (var item in order.Items)
            {
                Console.WriteLine("Product: {0} - Quantity: {1}", item.Name, item.Quantity);
            }

        }
    }
}