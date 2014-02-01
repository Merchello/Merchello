using System.Linq;
using Merchello.Core.Models.Rdbms;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Invoicing
{
    public class InvoicingTestsBase : ServiceIntegrationTestBase
    {
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var dtos = PreTestDataWorker.Database.Query<InvoiceStatusDto>("SELECT * FROM merchInvoiceStatus");
            if (!dtos.Any())
            {
                Assert.Ignore("Default InvoiceStatuses not installed.");
            }
        }
         
    }
}