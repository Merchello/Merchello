namespace Merchello.Tests.IntegrationTests.Services.Shipping
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.Models.ContentEditing;

    using NUnit.Framework;

    [TestFixture]
    public class ShippingServiceTests : MerchelloAllInTestBase
    {
        private IShipmentService _shipmentService;

        [SetUp]
        public void Init()
        {
            _shipmentService = MerchelloContext.Current.Services.ShipmentService;
        }

        /// <summary>
        /// Test asserts that all shipment statuses can be queried
        /// </summary>
        [Test]
        public void Can_Query_All_ShipmentStatuses()
        {
            //// Arrange
            
            //// Act
            var statuses = _shipmentService.GetAllShipmentStatuses().ToArray();

            //// Assert
            Assert.AreEqual(5, statuses.Count());
            foreach (var status in statuses.Select(x => x.ToShipmentStatusDisplay()))
            {
                Console.WriteLine(status.Name + ' ' + status.SortOrder);
            }

        }
         
    }
}