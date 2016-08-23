namespace Merchello.Tests.IntegrationTests.Examine
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;

    using NUnit.Framework;

    using umbraco.webservices;

    [TestFixture]
    public class CachedQueryTests : MerchelloAllInTestBase
    {
        private MerchelloHelper _merchello;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var invoices = ((InvoiceService)MerchelloContext.Current.Services.InvoiceService).GetAll();
            MerchelloContext.Current.Services.InvoiceService.Delete(invoices);

            // add 60 invoices starting 60 days back
            var start = DateTime.Today.AddDays(-30);
            var end = DateTime.Today;

            while (start != end)
            {
                var inv = MockInvoiceDataMaker.GetMockInvoiceForTaxation();
                inv.InvoiceDate = start;
                MerchelloContext.Current.Services.InvoiceService.Save(inv);
                start = start.AddDays(1);
            }

            _merchello = new MerchelloHelper(MerchelloContext.Current, false);
        }

        [Test]
        public void Can_Query_Invoices_By_Term_And_DateRange()
        {
            var name = "Space";
            var startDate = DateTime.Today.AddDays(-10);
            var endDate = DateTime.Today;

            var results = _merchello.Query.Invoice.Search(name, startDate, endDate, 1, long.MaxValue);

            Assert.NotNull(results);
            Assert.IsTrue(results.Items.Any());
        }

    }
}