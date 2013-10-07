using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Product
{
    [TestFixture]
    [Category("ProductService Integration")]
    public class ProductTests : ServiceIntegrationTestBase
    {
        private IProductService _productService;

        [SetUp]
        public void Setup()
        {
            PreTestDataWorker.DeleteAllProducts();
            _productService = PreTestDataWorker.ProductService;
        }

        /// <summary>
        /// Test to verify that a product can be created
        /// </summary>
        [Test]
        public void Can_Create_A_Product()
        {
            //// Arrage
            var sku = MockDataMakerBase.MockSku();
            var expected = MockProductDataMaker.MockProductForInserting("fake product",sku, 15.00m);

            //// Act
            var product = _productService.CreateProduct("fake product", sku, 15.00m);

            //// Assert
            Assert.AreEqual(expected.Name, product.Name);
        }

        /// <summary>
        /// Test to verify that a product can be saved
        /// </summary>
        [Test]
        public void Can_Save_A_Product()
        {
            //// Arrange
            var product = MockProductDataMaker.MockProductForInserting();

            //// Act
            _productService.Save(product);

            //// Assert
            Assert.IsTrue(product.HasIdentity);
        }

        /// <summary>
        /// Verifies that a product can be retreived by Key
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Product_By_Key()
        {
            //// Arrange
            var expected = PreTestDataWorker.MakeExistingProduct();
            var key = expected.Key;

            //// Act
            var retrieved = _productService.GetByKey(key);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual(expected.Key, retrieved.Key);
        }

        /// <summary>
        /// Test to verify a product can be retrieved with 3 options
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Product_With_3_Options()
        {
            //// Arrange
            var expected = PreTestDataWorker.MakeExistingProduct();
            var key = expected.Key;
            expected.ProductOptions.Add(new ProductOption("Color"));
            expected.ProductOptions.Add(new ProductOption("Size"));
            expected.ProductOptions.Add(new ProductOption("Material"));
            _productService.Save(expected);


            //// Act
            var retrieved = _productService.GetByKey(key);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(3 == retrieved.ProductOptions.Count);
            Assert.IsFalse(retrieved.IsDirty());
        }

        /// <summary>
        /// Test to verify a product can be retrieved with 3 options each with choices
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Product_With_3_Options_With_Choices()
        {
            //// Arrange
            var expected = PreTestDataWorker.MakeExistingProduct();
            var key = expected.Key;
            expected.ProductOptions.Add(new ProductOption("Color"));

            expected.ProductOptions["Color"].Choices.Add(new ProductAttribute("Black", "Black"));
            expected.ProductOptions["Color"].Choices.Add(new ProductAttribute("Red", "Red"));
            expected.ProductOptions["Color"].Choices.Add(new ProductAttribute("Grey", "Grey"));

            expected.ProductOptions.Add(new ProductOption("Size"));
            expected.ProductOptions["Size"].Choices.Add(new ProductAttribute("Small", "Sm"));
            expected.ProductOptions["Size"].Choices.Add(new ProductAttribute("Medium", "M"));
            expected.ProductOptions["Size"].Choices.Add(new ProductAttribute("Large", "Lg"));
            expected.ProductOptions["Size"].Choices.Add(new ProductAttribute("X-Large", "XL"));

            expected.ProductOptions.Add(new ProductOption("Material"));
            expected.ProductOptions["Material"].Choices.Add(new ProductAttribute("Cotton", "Cotton"));
            expected.ProductOptions["Material"].Choices.Add(new ProductAttribute("Wool", "Wool"));

            _productService.Save(expected);

            //// Act
            var retrieved = _productService.GetByKey(key);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(3 == retrieved.ProductOptions.Count);
            Assert.IsTrue(3 == retrieved.ProductOptions["Color"].Choices.Count);
            Assert.IsFalse(retrieved.IsDirty());
        }

        /// <summary>
        /// Verifies that a multiple products can be saved
        /// </summary>
        [Test]
        public void Can_Save_Multiple_Products()
        {
            //// Arrange
            var expected = 3;
            var generated = MockProductDataMaker.MockProductCollectionForInserting(expected);

            //// Act
            _productService.Save(generated);

            //// Assert
            var retrieved = ((ProductService)_productService).GetAll();
            Assert.IsTrue(retrieved.Any());
            Assert.AreEqual(expected, retrieved.Count());
        }

        /// <summary>
        /// Test to verify a product can be deleted
        /// </summary>
        [Test]
        public void Can_Delete_A_Product()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();
            var key = product.Key;

            //// Act
            _productService.Delete(product);

            //// Assert
            var retrieved = _productService.GetByKey(key);
            Assert.IsNull(retrieved);
        }

        /// <summary>
        /// Can update a product
        /// </summary>
        [Test]
        public void Can_Update_A_Product()
        {
            //// Arrange
            var generated = PreTestDataWorker.MakeExistingProduct();
            var old = generated.Name;
            var newName = "Wizard's Hat";

            //// Act
            generated.Name = newName;
            _productService.Save(generated);

            //// Assert
            var retrieved = _productService.GetByKey(generated.Key);
            Assert.NotNull(retrieved);
            Assert.AreNotEqual(old, retrieved.Name);
            Assert.AreEqual(newName, retrieved.Name);
        }
        
        /// <summary>
        /// Test to assert multiple products can be updated
        /// </summary>
        [Test]
        public void Can_Update_Multiple_Products()
        {
            //// Arrange
            var count = 4;
            var generated = PreTestDataWorker.MakeExistingProductCollection(count);
            var keys = new List<Guid>();
            var changed = new List<IProduct>();
            var name = "Wizard's hat";
            //// Act
            foreach (var p in generated)
            {
                p.Name = name;
                keys.Add(p.Key);
                changed.Add(p);
            }
            _productService.Save(changed);
            var expected = _productService.GetByKeys(keys);

            //// Assert
            foreach (var p in expected)
            {
                Assert.IsTrue(name == p.Name);
            }

        }

        /// <summary>
        /// Test verifies service returns true for an existing sku
        /// </summary>
        [Test]
        public void Can_Verify_A_Sku_Exists()
        {
            //// Arrange
            var existing = PreTestDataWorker.MakeExistingProduct();
            var sku = existing.Sku;

            //// Act
            var skuExists = _productService.SkuExists(sku);

            //// Assert
            Assert.IsTrue(skuExists);
        }

        /// <summary>
        /// Test to verify the service returns false for a non existing sku
        /// </summary>
        [Test]
        public void Can_Verify_A_Sku_Do_Not_Exist()
        {
            //// Arrange
            var guidSku = Guid.NewGuid().ToString();

            //// Act
            var skuExists = _productService.SkuExists(guidSku);

            //// Assert
            Assert.IsFalse(skuExists);
        }

        /// <summary>
        /// Test to verify that attempting to insert a new product with an existing sku fails
        /// </summary>
        [Test]
        public void Can_Verify_Attempting_To_Save_A_New_Product_With_An_Existing_Sku_Fails()
        {
            //// Arrange
            var existing = PreTestDataWorker.MakeExistingProduct();
            var existingSku = existing.Sku;

            //// Act & Assert
            Assert.Throws<ArgumentException>(() => _productService.CreateProductWithKey("Same sku", existingSku, 19M));

        }
    }
}
