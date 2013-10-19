using System;
using Merchello.Core.Models;
using Merchello.Tests.Base.DataMakers;
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
            var pvKeyExists = _extendedData.ContainsProductVariantId();

            //// Assert
            Assert.IsTrue(pKeyExists);
            Assert.IsTrue(pvKeyExists);
        }

        //[Test]

    }
}