using System;
using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Tests.Base.DataMakers;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.ExtendedData
{
    [TestFixture]
    public class ExtendedDataExtensionTests
    {

        private ExtendedDataCollection _extendedData;

        [SetUp]
        public void Init()
        {
            _extendedData = new ExtendedDataCollection();
            var product = MockProductDataMaker.MockProductComplete(Guid.NewGuid());
            _extendedData.AddProductVariantValues(((Product)product).MasterVariant); 
        }

        /// <summary>
        /// Can confirm that a product key and product variant key exists in a collection
        /// </summary>
        [Test]
        public void Can_Confirm_ProductKey_And_ProductVariantId_Exists_In_Collection()
        {
            //// Arrange - in Init
            
            //// Act
            var pKeyExists = _extendedData.ContainsProductKey();
            var pvKeyExists = _extendedData.ContainsProductVariantKey();

            //// Assert
            Assert.IsTrue(pKeyExists);
            Assert.IsTrue(pvKeyExists);
        }

        /// <summary>
        /// Test confirms that an ExtendedDataCollection can be converted to an IEnumerable and nicely serialized
        /// </summary>
        [Test]
        public void Can_Convert_ExtendedDataCollection_To_An_Enumerable_And_Serialize_ToJSON()
        {
            //// Arrange
            
            //// Act
            var collection = _extendedData.AsEnumerable();
            var json = JsonConvert.SerializeObject(collection);

            //// Assert
            Assert.NotNull(collection);            
            Assert.IsNotNullOrEmpty(json);
            Console.Write(json);

        }

        /// <summary>
        /// Simulates the ExtendedDataConversion with the custom AutoMapper extended data resolution
        /// </summary>
        [Test]
        public void Can_Deserialize_An_IEnumerableKeyValuePair_To_An_ExtendedDataCollection()
        {
            //// Arrange
            var json = JsonConvert.SerializeObject(_extendedData.AsEnumerable());

            //// Act
            var collection = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(json);
            Assert.NotNull(collection, "Collection could not be deserialized");

            var extendData = collection.AsExtendedDataCollection();

            //// Assert
            Assert.NotNull(extendData);

        }
    }
}