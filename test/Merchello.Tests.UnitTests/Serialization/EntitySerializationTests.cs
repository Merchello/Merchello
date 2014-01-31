using System;
using System.Linq;
using System.Xml.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;
using Rhino.Mocks;
using umbraco;
using Umbraco.Tests.PublishedCache;

namespace Merchello.Tests.UnitTests.Serialization
{
    [TestFixture]
    public class EntitySerializationTests
    {
        private IAddress _address;
        private IShipment _shipment;

        [SetUp]
        public void Init()
        {
            _address = new Address()
            {
                Name = "Mindfly Web Design Studio",
                Address1 = "114 W. Magnolia St.  Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US"
            };

            var extendedData = new ExtendedDataCollection();

            extendedData.SetValue("merchProductKey", Guid.NewGuid().ToString());
            extendedData.SetValue("merchProductVariantKey", Guid.NewGuid().ToString());
            extendedData.SetValue("merchWeight", "12");

            var containerKey = Guid.NewGuid();
            var lineItemCollection = new LineItemCollection()
            {
                {new ItemCacheLineItem(containerKey, LineItemType.Product, "Product1", "Sku1", 1, 10, extendedData)},
                {new ItemCacheLineItem(containerKey, LineItemType.Product, "Product2", "Sku2", 2, 12, extendedData)},
                {new ItemCacheLineItem(containerKey, LineItemType.Product, "Product3", "Sku3", 3, 14, extendedData)},
            };

            _shipment = new Shipment(_address, _address, lineItemCollection);
        }

        /// <summary>
        /// Test verifies that an Address can be serialized
        /// </summary>
        [Test]
        public void Can_Serialize_IAddress_To_Xml()
        {
            //// Arrange

            //// Act
            var xml = SerializationHelper.SerializeToXml(_address as Address);
            Console.Write(xml);
            //// Assert
            Assert.IsFalse(string.IsNullOrEmpty(xml));
            Assert.DoesNotThrow(() => XDocument.Parse(xml));
        }

        /// <summary>
        /// Test verifies that an address can be deserialized
        /// </summary>
        [Test]
        public void Can_Deserialize_XmlAddress_To_Address()
        {
            //// Arrange
          
            var xml = SerializationHelper.SerializeToXml(_address as Address);

            //// Act
            var attempt = SerializationHelper.DeserializeXml<Address>(xml);

            //// Assert
            Assert.NotNull(attempt);
            Assert.IsTrue(attempt.Success);
        }

        /// <summary>
        /// Test shows that an extendeddatacollection can be serialized
        /// </summary>
        [Test]
        public void Can_Serialize_An_ExtendedDataCollection()
        {
            //// Arrange
            
            //// Act
            var xml = SerializationHelper.SerializeToXml(_shipment.Items.First().ExtendedData);
            Console.Write(xml);

            //// Assert
            Assert.IsFalse(string.IsNullOrEmpty(xml));
            Assert.DoesNotThrow(() => XDocument.Parse(xml)); 
        }

        /// <summary>
        /// Test shows that a ExtendedDataCollection can be deserialized
        /// </summary>
        [Test]
        public void Can_Deserialize_An_ExtendedDataCollection()
        {
            //// Arrange
            var xml = SerializationHelper.SerializeToXml(_shipment.Items.First().ExtendedData);

            //// Act
            var attempt = SerializationHelper.DeserializeXml<ExtendedDataCollection>(xml);
            if(!attempt.Success) Console.Write(attempt.Exception.Message);
            
            //// Assert
            Assert.IsTrue(attempt.Success);            
        }

        ///// <summary>
        ///// Test shows that a LineItem can be serialized
        ///// </summary>
        //[Test]
        //public void Can_Serialize_A_LineItem()
        //{
        //    //// Arrange
            
        //    //// Act
        //    var xml = SerializationHelper.SerializeToXml(_shipment.Items.First() as ItemCacheLineItem);
        //    Console.Write(xml);

        //    //// Assert
        //    Assert.IsFalse(string.IsNullOrEmpty(xml));
        //    Assert.DoesNotThrow(() => XDocument.Parse(xml)); 
        //}

        ///// <summary>
        ///// Test shows that a LineItemCollection can be deserialized
        ///// </summary>
        //[Test]
        //public void Can_Deserialize_A_LineItemCollection()
        //{
        //    //// Arrange
        //    var xml = SerializationHelper.SerializeToXml(_shipment.Items); ;
        //    Console.Write(xml);

        //    //// Act
        //    var attempt = SerializationHelper.DeserializeXml<LineItemCollection>(xml);
        //    if (!attempt.Success) Console.Write(attempt.Exception.Message);

        //    //// Assert
        //    Assert.IsTrue(attempt.Success); 
        //}
    }
}