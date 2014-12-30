namespace Merchello.Tests.IntegrationTests.TestHelpers.JsonResultData
{
    using System;
    using System.Linq;

    using ClientDependency.Core;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class JsonDataWriter : MerchelloAllInTestBase
    {
        //[Test]
        public void OrderJsonResults()
        {
            var merchello = new MerchelloHelper();
            var notFulfilled = Core.Constants.DefaultKeys.OrderStatus.NotFulfilled;
            Console.WriteLine(notFulfilled);

            var orders = merchello.Query.Order.SearchByOrderStatus(
                Core.Constants.DefaultKeys.OrderStatus.NotFulfilled,
                1,
                10).Items;

            Console.Write(JsonConvert.SerializeObject(orders));
            
        }

        //[Test]
        public void InvoiceJsonResults()
        {
            var merchello = new MerchelloHelper();

            var invoices = merchello.Query.Invoice.Search(1, 10).Items;

            Console.WriteLine(JsonConvert.SerializeObject(invoices));
        }

        [Test]
        public void ShipmentJsonResults()
        {
            var shipmentService = MerchelloContext.Current.Services.ShipmentService;

            var shipments = ((ShipmentService)shipmentService).GetAll().Select(x => x.ToShipmentDisplay());

            Console.WriteLine(JsonConvert.SerializeObject(shipments));

        }
    }
}