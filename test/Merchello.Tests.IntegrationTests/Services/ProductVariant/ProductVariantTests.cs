using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;

using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.ProductVariant
{
    [TestFixture]
    [Category("Service Integration")]
    public class ProductVariantTests : DatabaseIntegrationTestBase
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
        /// Test confirms that a collection of all possible product attribute combinations can be created.
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Get_A_List_Of_All_Possible_Attribute_Combinations()
        {
            //// Arrange
            
            //// Act
            var combinations = ((ProductVariantService)_productVariantService).GetPossibleProductAttributeCombinations(_product).ToArray();

            //// Assert
            Assert.IsTrue(combinations.Any());
            Assert.AreEqual(16, combinations.Count());
        }

        /// <summary>
        /// Test confirms that a collection of ProductVariants that HAVE YET to be created can be retrieved for a product
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Get_A_List_Of_All_Possible_Variants_That_Can_Be_Created()
        {
            //// Arrange
            
            //// Act
            var variants = ((ProductVariantService) _productVariantService).GetProductVariantsThatCanBeCreated(_product);

            //// Assert
            Assert.IsTrue(variants.Any());
            Assert.AreEqual(16, variants.Count());
        }


        /// <summary>
        /// Test verifies that a product variant can be created
        /// </summary>
        [Obsolete("This is now handled by saving a product with options")]
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
        [Test, Category("LongRunning")]
        public void Can_Not_Create_A_Duplicate_ProductVariant()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg" )
            };

            //// Act 
            
 
            //// Assert
            Assert.Throws<ArgumentException>(() => _productVariantService.CreateProductVariantWithKey(_product, attributes));
        }

        /// <summary>
        /// Test verifies that a variant can be retrieved by it's key
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Retrieve_A_ProductVariant_by_Its_Key()
        {
            //// Arrange
            Assert.IsTrue(_product.ProductVariants.Any());
            var expected = _product.ProductVariants.First();
            var id = expected.Key;
            Assert.AreNotEqual(id, Guid.Empty);

            //// Act
            var retrieved = _productVariantService.GetByKey(id);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(id == retrieved.Key);
        }

        /// <summary>
        /// Test verifies that a variant can be retrieved by it's sku
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Retrieve_A_ProductVariant_By_Its_Sku()
        {
            //// Arrange
            Assert.IsTrue(_product.ProductVariants.Any());
            var expected = _product.ProductVariants.First();
            var sku = expected.Sku;

            //// Act
            var retrieved = _productVariantService.GetBySku(sku);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(sku == retrieved.Sku);
        }

        /// <summary>
        /// Test verifies that product variants can be retrieved for a particular product
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Retrieve_All_Variants_For_A_Product()
        {
            //// Arrange
            Assert.AreEqual(16, _product.ProductVariants.Count);

            //// Act
            var variants = _productVariantService.GetByProductKey(_product.Key).ToArray();

            //// Assert
           Assert.IsTrue(variants.Any());
           Assert.AreEqual(16, variants.Count());            
        }

        /// <summary>
        /// Test verifies that a variant can be deleted
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Delete_A_Variant()
        {
            //// Arrange
            Assert.IsTrue(_product.ProductVariants.Any());
            var variant = _product.ProductVariants.First();
            var key = variant.Key;

            //// Act
            _productVariantService.Delete(variant);

            //// Assert
            var retrieved = _productVariantService.GetByKey(key);

            Assert.IsNull(retrieved);
        }

        /// <summary>
        /// Test verifies deleting an option also deletes its corresponding variants
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Verify_That_ProductVariant_Is_Deleted_When_An_Option_Is_Deleted()
        {
            //// Arrange
            var remover = _product.ProductOptions.First(x => x.Name == "Size");
            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            _product.ProductOptions.Remove(remover);
            _productService.Save(_product);

            //// Assert
            Assert.AreEqual(1, _product.ProductOptions.Count);
            Assert.AreEqual(4, _product.ProductVariants.Count);
            Assert.IsFalse(_product.ProductOptions.Contains(remover));
        }

        /// <summary>
        /// Test verifies deleting an attribute also deletes variants that are assoicated with the attribute
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Verify_Removing_An_Attribute_Deletes_Variants_That_Have_That_Attribute()
        {
            //// Arrange
            var remover = _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg");
            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Remove(remover);
            _productService.Save(_product);

            //// Assert
            Assert.IsFalse(_product.ProductVariants.Any(x => x.Attributes.Contains(remover)));
        }

        /// <summary>
        /// Test verifies that a product variant can be retrieved given a product and a collection of attribute Ids
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Retrieve_A_ProductVariant_Given_A_Product_And_A_Collection_Of_AttributeIds()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blk"),
                _product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "Lg")
            };
            
            
            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            var attIds = attributes.Select(x => x.Key).ToArray();
            var retrieved = _productVariantService.GetProductVariantWithAttributes(_product, attIds);

            //// Assert
            Assert.NotNull(retrieved);

        }

        /// <summary>
        /// Test verifies that a warehouse can be associated with a variant
        /// </summary>
        [Test, Category("LongRunning")]
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
        [Test, Category("LongRunning")]
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

        [Test, Category("LongRunning")]
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
        [Test, Category("LongRunning")]
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
        [Test, Category("LongRunning")]
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