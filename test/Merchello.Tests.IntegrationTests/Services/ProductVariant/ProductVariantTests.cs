using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.ProductVariant
{
    [TestFixture]
    [Category("Service Integration")]
    public class ProductVariantTests : ServiceIntegrationTestBase
    {
        private IProductService _productService;
        private IProductVariantService _productVariantService;
        private IProduct _product;
        private IWarehouse _warehouse;
        [SetUp]
        public void Init()
        {

            _warehouse = PreTestDataWorker.WarehouseService.GetDefaultWarehouse();

            PreTestDataWorker.DeleteAllProducts();
            _productService = PreTestDataWorker.ProductService;
            _productVariantService = PreTestDataWorker.ProductVariantService;

            _product = PreTestDataWorker.MakeExistingProduct();
            _product.ProductOptions.Add(new ProductOption("Color"));
            _product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Blk"));
            _product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blu"));
            _product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Red", "Red"));
            _product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Green", "Gre"));
            _product.ProductOptions.Add(new ProductOption("Size"));
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Sm"));
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Medium", "M"));
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Large", "Lg"));
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("X-Large", "XL"));
            _product.Height = 20;
            _product.Weight = 20;
            _product.Length = 20;
            _product.Width = 20;
            _product.Shippable = true;
            _productService.Save(_product);

        }

        /// <summary>
        /// Test verifies that a product variant can be created
        /// </summary>
        [Test]
        public void Can_Create_A_ProductVariant()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg" )
            };

            //// Act
            var variant = _productVariantService.CreateProductVariantWithKey(_product, attributes);

            //// Assert
            Assert.IsTrue(variant.HasIdentity);
            
        }

        /// <summary>
        /// Test verifies that a product variant cannot be created twice
        /// </summary>
        [Test]
        public void Can_Not_Create_A_Duplicate_ProductVariant()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg" )
            };
            var variant = _productVariantService.CreateProductVariantWithKey(_product, attributes);

            //// Act 
            
 
            //// Assert
            Assert.Throws<ArgumentException>(() => _productVariantService.CreateProductVariantWithKey(_product, attributes));
        }

        /// <summary>
        /// Test verifies that a variant can be retrieved by it's key
        /// </summary>
        [Test]
        public void Can_Retrieve_A_ProductVariant_by_Its_Key()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg" )
            };
            var expected = _productVariantService.CreateProductVariantWithKey(_product, attributes);
            var id = expected.Key;
            Assert.AreNotEqual(id, Guid.Empty);

            //// Act
            var retrieved = _productVariantService.GetByKey(id);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(id == retrieved.Key);
        }

        /// <summary>
        /// Test verifies that product variants can be retrieved for a particular product
        /// </summary>
        [Test]
        public void Can_Retrieve_All_Variants_For_A_Product()
        {
            //// Arrange
            var attributes1 = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg" )
            };
            var attributes2 = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "XL" )
            };
            _productVariantService.CreateProductVariantWithKey(_product, attributes1);
            _productVariantService.CreateProductVariantWithKey(_product, attributes2);

            Assert.IsTrue(_product.ProductVariants.Count == 2);

            //// Act
            var variants = _productVariantService.GetByProductKey(_product.Key);

            //// Assert
           Assert.IsTrue(variants.Any());
           Assert.IsTrue(2 == variants.Count());

        }

        /// <summary>
        /// Test verifies that a variant can be deleted
        /// </summary>
        [Test]
        public void Can_Delete_A_Variant()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
               _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg" )
            };
            var variant = _productVariantService.CreateProductVariantWithKey(_product, attributes);
            var key = variant.Key;
            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            _productVariantService.Delete(variant);

            //// Assert
            var retrieved = _productVariantService.GetByKey(key);

            Assert.IsNull(retrieved);
        }

        /// <summary>
        /// Test verifies deleting an option also deletes its corresponding variants
        /// </summary>
        [Test]
        public void Can_Verify_That_ProductVariant_Is_Deleted_When_An_Option_Is_Deleted()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg" )
            };
            _productVariantService.CreateProductVariantWithKey(_product, attributes);
            
            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            _product.ProductOptions.Remove(_product.ProductOptions.First(x => x.Name == "Size"));
            _productService.Save(_product);

            //// Assert
            Assert.IsFalse(_product.ProductVariants.Any());
        }

        /// <summary>
        /// Test verifies deleting an attribute also deletes variants that are assoicated with the attribute
        /// </summary>
        [Test]
        public void Can_Verify_Removing_An_Attribute_Deletes_Variants_That_Have_That_Attribute()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg")
            };
            _productVariantService.CreateProductVariantWithKey(_product, attributes);

            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Remove(_product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg"));
            _productService.Save(_product);

            //// Assert
            Assert.IsFalse(_product.ProductVariants.Any());
        }

        /// <summary>
        /// Test verifies that a product variant can be retrieved given a product and a collection of attribute Ids
        /// </summary>
        [Test]
        public void Can_Retrieve_A_ProductVariant_Given_A_Product_And_A_Collection_Of_AttributeIds()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg")
            };
            _productVariantService.CreateProductVariantWithKey(_product, attributes);
            
            Assert.IsTrue(_product.ProductVariants.Any());

            var ids = _product.ProductVariants.First().Attributes.Select(att => att.Key).ToArray();

            //// Act
            var retrieved = _productVariantService.GetProductVariantWithAttributes(_product, ids);

            //// Assert
            Assert.NotNull(retrieved);

        }

        /// <summary>
        /// Test verifies that a warehouse can be associated with a variant
        /// </summary>
        [Test]
        public void Can_Add_A_Warehouse_To_A_ProductVariant()
        {
            //// Arrange
            
            //// Act
            _product.AddToCatalogInventory(_warehouse.DefaultCatalog());

            //// Assert
            Assert.IsTrue(_product.CatalogInventories.Count() == 1);
        }

        /// <summary>
        /// Test verifies that a warehouse can be assoicated with a variant and saved
        /// </summary>
        [Test]
        public void Can_Add_And_Save_A_Warehouse_To_A_ProductVariant()
        {
            //// Arrange
            const int warehouseId = 1;

            //// Act
            _product.AddToCatalogInventory(_warehouse.DefaultCatalog());
            _productService.Save(_product);

            //// Assert
            Assert.IsTrue(_product.CatalogInventories.Count() == 1);
        }

        [Test]
        public void Can_Update_A_ProductVariants_Inventory_Count()
        {
            //// Arrange

            //// Act
            _product.AddToCatalogInventory(_warehouse.DefaultCatalog());
            _product.CatalogInventories.First().Count = 10;
            _productService.Save(_product);

            //// Assert
            Assert.IsTrue(_product.CatalogInventories.Count() == 1);
        }

        /// <summary>
        /// Test verifies that a CatalogInventory record can be updated once persisted
        /// </summary>
        [Test]
        public void Can_Update_A_ProductVariants_CatalogInventory_After_Its_Been_Saved()
        {
            //// Arrange
            var key = _product.Key;
            _product.AddToCatalogInventory(_warehouse.DefaultCatalog());
            _productService.Save(_product);
            Assert.IsTrue(_product.CatalogInventories.Any());
            
            //// Act
            _product.CatalogInventories.First().Count = 10;
            _productService.Save(_product);

            var retrieved = _productService.GetByKey(key);
            retrieved.CatalogInventories.First().Count = 9;
            _productService.Save(retrieved);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(retrieved.CatalogInventories.Any());
            Assert.AreEqual(9, retrieved.CatalogInventories.First().Count);

        }

        /// <summary>
        /// Quick test to verify the CatalogInventoryCollection is treating keys correctly
        /// </summary>
        [Test]
        public void Can_Add_A_ProductVariant_To_CategoryInventory_Twice_Without_Causing_An_Error()
        {
            //// Arrange
            var key = _product.Key;
            _product.AddToCatalogInventory(_warehouse.DefaultCatalog());
            _productService.Save(_product);
            Assert.IsTrue(_product.CatalogInventories.Any());

            //// Act
            _product.AddToCatalogInventory(_warehouse.DefaultCatalog());
            _productService.Save(_product);

            //// Assert
            Assert.IsTrue(_product.CatalogInventories.Any());
            Assert.IsTrue(1 == _product.CatalogInventories.Count());
        }

    }
}