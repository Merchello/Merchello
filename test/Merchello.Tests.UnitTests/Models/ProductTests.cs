using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Models
{
    [TestFixture]
    public class ProductTests
    {
        private IProduct _product;

        [SetUp]
        public void Init()
        {
            var variant = new ProductVariant(Guid.NewGuid(), new ProductAttributeCollection(),
                new CatalogInventoryCollection(), true, false, "Product Name", "TestSku", 10M);
            _product = new Product(variant);
        }

        ///// <summary>
        ///// Test verifies a ware house can be associated with a product
        ///// </summary>
        //[Test]
        //public void Can_Add_A_Warehouse_To_A_Product()
        //{
        //    //// Arrange
            
        //    //// Act
        //    _product.AddToWarehouseCatalog(new WarehouseCatalog(Guid.NewGuid()));

        //    //// Assert
        //    Assert.IsTrue(_product.Warehouses.Any());
        //}

        /// <summary>
        /// Test verifies that a purchasable variant can be retrieved for a product with no options
        /// </summary>
        [Test]
        public void Can_Retrieve_Purchasable_Variant_For_A_Product_With_No_Options()
        {
            //// Arrange
            var expected = ((Product) _product).MasterVariant;

            //// Act
            var variant = _product.GetProductVariantForPurchase();

            //// Assert
            Assert.NotNull(variant);
            Assert.AreEqual(expected.Key, variant.Key);
        }

        /// <summary>
        /// Test verifies that if a product has options, then the parameterless GetVariantForPurchase method returns null
        /// </summary>
        [Test]
        public void Can_verify_null_Is_Returned_For_Default_GetVariantForPurchase_For_A_Product_With_Options()
        {
            //// Arrange
            _product.ProductOptions.Add(new ProductOption("Option1") { Key = Guid.NewGuid() });

            //// Act
            var variant = _product.GetProductVariantForPurchase();

            //// Assert
            Assert.IsNull(variant);
        }

        /// <summary>
        /// Test verifies that a product variant can be retrieved from GetVariantForPurchase method given a list of attributes
        /// </summary>
        [Test]
        public void Can_Get_A_ProductVariant_From_GetVariantForPurchase_Method_Given_A_List_Of_Attributes()
        {            
            //// Arrange
            var att = new ProductAttribute("Choice1", "choice") { Key = Guid.NewGuid() };
            var attCollection = new ProductAttributeCollection();
            attCollection.Add(att);
            var expected = new ProductVariant(Guid.NewGuid(), attCollection, new CatalogInventoryCollection(), false, false,
                "Variant", "variant", 5M);
            _product.ProductOptions.Add(new ProductOption("Option1") { Key = Guid.NewGuid() });
            _product.ProductOptions.First(x => x.Name == "Option1").Choices.Add(att);
            _product.ProductVariants.Add(expected);

            //// Act
            var variant = _product.GetProductVariantForPurchase(attCollection);

            //// Assert
            Assert.NotNull(variant);
            Assert.AreEqual(expected.Key, variant.Key);
        }


        /// <summary>
        /// Test verifies that a product variant can be retrieved from GetVariantForPurchase method given a list of ids
        /// </summary>
        [Test]
        public void Can_Get_A_ProductVariant_From_GetVariantForPurchase_Method_Given_A_List_Of_AttributeKeys()
        {
            //// Arrange
            var att = new ProductAttribute("Choice1", "choice") { Key = Guid.NewGuid() };
            var key = att.Key;
            var attCollection = new ProductAttributeCollection {att};
            var expected = new ProductVariant(Guid.NewGuid(), attCollection, new CatalogInventoryCollection(), false, false,
                "Variant", "variant", 5M);
            _product.ProductOptions.Add(new ProductOption("Option1") { Key = Guid.NewGuid() });
            _product.ProductOptions.First(x => x.Name == "Option1").Choices.Add(att);
            _product.ProductVariants.Add(expected);

            //// Act
            var variant = _product.GetProductVariantForPurchase(new [] { key });

            //// Assert
            Assert.NotNull(variant);
            Assert.AreEqual(expected.Key, variant.Key);
        }

        [Test]
        public void Can_Serialize_A_Product_To_Xml()
        {
            //// Arrange
            var att = new ProductAttribute("Choice1", "choice") { Key = Guid.NewGuid() };
            var attCollection = new ProductAttributeCollection();
            attCollection.Add(att);
            var expected = new ProductVariant(Guid.NewGuid(), attCollection, new CatalogInventoryCollection(), false, false,
                "Variant", "variant", 5M);
            _product.ProductOptions.Add(new ProductOption("Option1") { Key = Guid.NewGuid() });
            _product.ProductOptions.First(x => x.Name == "Option1").Choices.Add(att);
            _product.ProductVariants.Add(expected);

            //// Act
            var xml = _product.SerializeToXml();
            Console.Write(xml.ToString());

            //// Assert
            Assert.NotNull(xml);
        }

    }
}