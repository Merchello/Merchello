using System.Linq;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Fulfillment
{
    public class FulfillmentTestsBase : DatabaseIntegrationTestBase
    {
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var invoiceStatusDtos = PreTestDataWorker.Database.Query<InvoiceStatusDto>("SELECT * FROM merchInvoiceStatus");
            var orderStatusDtos = PreTestDataWorker.Database.Query<OrderStatusDto>("SELECT * FROM merchOrderStatus");

            if (!invoiceStatusDtos.Any() || !orderStatusDtos.Any())
            {
                Assert.Ignore("Default InvoiceStatuses and/or OrderStatues not installed.");
            }
        }
         
    }
}