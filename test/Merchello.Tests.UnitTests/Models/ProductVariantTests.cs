﻿using System;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Models
{
    [TestFixture]
    [Category("Models")]
    public class ProductVariantTests
    {
        private IProductVariant _productVariant;
        private ProductAttributeCollection _attributes;

        [SetUp]
        public void Init()
        {
            _attributes = new ProductAttributeCollection()
                {
                    new ProductAttribute("Att1", "Sku1") { Id = 1 },
                    new ProductAttribute("Att2", "Sku2") { Id = 2 },
                    new ProductAttribute("Att3", "Sku3") { Id = 3 }
                };

            _productVariant = new ProductVariant(Guid.NewGuid(), _attributes, new WarehouseInventoryCollection(), false, "Product1", "P1", 11M);
        }

        /// <summary>
        /// Test verifys that the ProductAttributeCollection "Equals" overide returns true for a simular list
        /// </summary>
        [Test]
        public void Can_Verify_ProductAttributeCollection_Equals_Returns_True_For_Same_Attributes()
        {
            //// Arrange - handled in Init
            
            //// Act / Assert
            Assert.IsTrue(_productVariant.Attributes.Equals(_attributes));
        }

        /// <summary>
        /// Test verifies that the "Equals" override returns false for a different collection of attributes with the same collection count
        /// </summary>
        [Test]
        public void Can_Verify_ProductAttributeCollection_Equals_Returns_False_For_Different_Attributes_SameCount()
        {
            //// Arrange
            var collection = new ProductAttributeCollection()
                {
                    new ProductAttribute("Att1", "Sku1") { Id = 1 },
                    new ProductAttribute("Att6", "Sku6") { Id = 6 },
                    new ProductAttribute("Att3", "Sku3") { Id = 3 }
                };
            
            //// Act / Assert
            Console.Write("Collections are equal: " + _productVariant.Attributes.Equals(collection));
            Assert.IsFalse(_productVariant.Attributes.Equals(collection));

        }

        /// <summary>
        /// Test verifies that the "Equals" override returns false for a different collection of attributes with a different collection count
        /// </summary>
        [Test]
        public void Can_Verify_ProductAttributeCollection_Equals_Returns_False_For_Different_Attributes_DifferentCount()
        {
            //// Arrange
            var collection = new ProductAttributeCollection()
                {
                    new ProductAttribute("Att1", "Sku1") { Id = 1 },
                    new ProductAttribute("Att2", "Sku2") { Id = 2 },
                    new ProductAttribute("Att3", "Sku3") { Id = 3 },
                    new ProductAttribute("Att4", "Sku4") { Id = 4 },
                };

            //// Act / Assert
            Console.Write("Collections are equal: " + _productVariant.Attributes.Equals(collection));
            Assert.IsFalse(_productVariant.Attributes.Equals(collection));
            
        }
    }
}