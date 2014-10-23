using System;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Tests.Base.Mocks;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Shipping
{
    [TestFixture]
    public class ShipmentTests
    {
        private IAddress _origin;
        private IAddress _destination;
        private IShipment _shipment;

        [SetUp]
        public void Init()
        {
            _origin = new Address()
            {
                Name = "Mindfly Web Design Studio",
                Address1 = "114 W. Magnolia St.  Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US",
                IsCommercial = true
            };

            _destination = new Address()
                {
                    Name = "Stratosphere Casino, Hotel & Tower",
                    Address1 = "2000 S Las Vegas Blvd",
                    Locality = "Las Vegas",
                    Region = "NV",
                    PostalCode = "89104",
                    CountryCode = "US",
                    IsCommercial = true
                };

            var extendedData = new ExtendedDataCollection();

            extendedData.SetValue("merchProductKey", Guid.NewGuid().ToString());
            extendedData.SetValue("merchProductVariantKey", Guid.NewGuid().ToString());
            extendedData.SetValue("merchWeight", "12");
            
            var lineItemCollection = new LineItemCollection()
            {
                {new ItemCacheLineItem(LineItemType.Product, "Product1", "Sku1", 1, 10, extendedData)},
                {new ItemCacheLineItem(LineItemType.Product, "Product2", "Sku2", 2, 12, extendedData)},
                {new ItemCacheLineItem(LineItemType.Product, "Product3", "Sku3", 3, 14, extendedData)},
            };

            _shipment = new Shipment(new ShipmentStatusMock(), _origin, _destination, lineItemCollection);
             
        }

        /// <summary>
        /// Test verfies the extension method OriginAddress() returns the origin address
        /// </summary>
        [Test]
        public void Can_Get_The_Origin_Address_From_A_Shipment()
        {
            //// Arrange
            // handled by SetUp

            //// Act
            var origin = _shipment.GetOriginAddress();

            //// Arrange
            Assert.NotNull(origin);
            Assert.IsTrue(((Address)_origin).Equals(origin));
        }

        /// <summary>
        /// Test verifies the extension method DestinationAddress() returns the destination address
        /// </summary>
        [Test]
        public void Can_Get_The_Destination_Address_From_A_Shipment()
        {
            //// Arrange
            // handled by SetUp

            //// Act
            var destination = _shipment.GetDestinationAddress();

            //// Arrange
            Assert.NotNull(destination);
            Assert.IsTrue(((Address)_destination).Equals(destination));
        }
    }
}