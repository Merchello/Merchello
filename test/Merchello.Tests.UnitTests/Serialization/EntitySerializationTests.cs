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
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.Mocks;

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

            var lineItemCollection = new LineItemCollection()
            {
                {new ItemCacheLineItem(LineItemType.Product, "Product1", "Sku1", 1, 10, extendedData)},
                {new ItemCacheLineItem(LineItemType.Product, "Product2", "Sku2", 2, 12, extendedData)},
                {new ItemCacheLineItem(LineItemType.Product, "Product3", "Sku3", 3, 14, extendedData)},
            };

            _shipment = new Shipment(new ShipmentStatusMock(), _address, _address, lineItemCollection);
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

        /// <summary>
        /// Test shows that a LineItemCollection can be serialized and added to Extended Data
        /// </summary>
        [Test]
        public void Can_Serialize_A_LineItemCollection_And_Store_In_ExtendedData()
        {
            //// Arrange
            var extendedData = new ExtendedDataCollection();

            //// Act
            extendedData.AddLineItemCollection(_shipment.Items);
            Console.Write(extendedData.GetValue(Constants.ExtendedDataKeys.LineItemCollection));

            //// Assert
            Assert.DoesNotThrow(() => XDocument.Parse(extendedData.GetValue(Constants.ExtendedDataKeys.LineItemCollection)));
        }

        /// <summary>
        /// Test shows that a LineItemCollection can be deserialized from an ExtendedDataCollection
        /// </summary>
        [Test]
        public void Can_Deserialize_A_LineItemCollection_From_ExtendedDataCollection()
        {
            //// Arrange
            var extendedData = new ExtendedDataCollection();
            extendedData.AddLineItemCollection(_shipment.Items);
            Console.Write(extendedData.GetValue(Constants.ExtendedDataKeys.LineItemCollection));

            //// Act
            var lineItemCollection = extendedData.GetLineItemCollection<ItemCacheLineItem>();

            //// Assert
            Assert.NotNull(lineItemCollection);
            Assert.IsTrue(lineItemCollection.Any());
        }

        /// <summary>
        /// Test shows that a LineItemCollection can be deserialized from a serialized ExtendedDataCollection
        /// </summary>
        [Test]
        public void Can_Deserialize_A_LineItemCollection_Stored_In_A_Serialized_ExtendedDataCollection()
        {
            //// Arrange
            var extendedDataContainer = new ExtendedDataCollection();
            var extendedDataWrapper = new ExtendedDataCollection();
            extendedDataWrapper.AddLineItemCollection(_shipment.Items);
            extendedDataContainer.AddExtendedDataCollection(extendedDataWrapper);
            Console.Write(extendedDataContainer.SerializeToXml());

            //// Act
            var retrievedExtendedDataWrapper = extendedDataContainer.GetExtendedDataCollection();
            Assert.NotNull(retrievedExtendedDataWrapper);
            var lineItemCollection = retrievedExtendedDataWrapper.GetLineItemCollection<ItemCacheLineItem>();

            //// Assert
            Assert.NotNull(lineItemCollection);
            Assert.IsTrue(lineItemCollection.Any());
        }

        /// <summary>
        /// Test shows that an Address can be serialized and stored in an ExtendedDataCollection
        /// </summary>
        [Test]
        public void Can_Serialize_An_Address_And_Store_In_ExtendedDataCollection()
        {
            //// Arrange
            var extendedData = new ExtendedDataCollection();

            //// Act
            extendedData.AddAddress(_address, AddressType.Billing);
            Console.Write(extendedData.SerializeToXml());

            //// Assert
            Assert.DoesNotThrow(() => XDocument.Parse(extendedData.GetValue(Constants.ExtendedDataKeys.BillingAddress)));
        }

        /// <summary>
        /// Test shows that an Address can be retrieved from an ExtendedDataCollection and deserialized 
        /// </summary>
        [Test]
        public void Can_Deserialize_An_Address_Stored_in_ExtendedDataCollection()
        {
            //// Arrange
            var extendedData = new ExtendedDataCollection();
            extendedData.AddAddress(_address, AddressType.Shipping);

            //// Act
            var address = extendedData.GetAddress(AddressType.Shipping);

            //// Assert
            Assert.NotNull(address);
            Assert.AreEqual(typeof(Address), address.GetType());
        }

        /// <summary>
        /// Test shows that an address can be retrieved from a serialized ExtendedDataCollection and deserialized
        /// </summary>
        [Test]
        public void Can_Deserialize_An_Address_Stored_In_A_Serialized_ExtendedDataCollection()
        {
            //// Arrange
            var extendedDataContainer = new ExtendedDataCollection();
            var extendedDataWrapper = new ExtendedDataCollection();
            extendedDataWrapper.AddAddress(_address, AddressType.Shipping);
            extendedDataContainer.AddExtendedDataCollection(extendedDataWrapper);

            //// Act
            var wrapper = extendedDataContainer.GetExtendedDataCollection();
            var address = wrapper.GetAddress(AddressType.Shipping);

            //// Assert
            Assert.NotNull(address);
            Assert.AreEqual(typeof(Address), address.GetType());
        }

    }
}