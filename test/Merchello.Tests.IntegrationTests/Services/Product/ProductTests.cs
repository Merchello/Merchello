using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Product
{
    [TestFixture]
    [Category("Service Integration")]
    public class ProductTests : DatabaseIntegrationTestBase
    {
        private IProductService _productService;

        [SetUp]
        public void Setup()
        {
            PreTestDataWorker.DeleteAllProducts();
            _productService = PreTestDataWorker.ProductService;
            Thread.Sleep(200); // pause for the index
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
        /// Verifies that a product can be retreived by it's unique sku
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Product_By_Sku()
        {
            //// Arrange
            var expected = PreTestDataWorker.MakeExistingProduct();
            var sku = expected.Sku;

            //// Act
            var retrieved = _productService.GetBySku(sku);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual(sku, retrieved.Sku);
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

            expected.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Black"));
            expected.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Red", "Red"));
            expected.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Grey", "Grey"));

            expected.ProductOptions.Add(new ProductOption("Size"));
            expected.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Sm"));
            expected.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Medium", "M"));
            expected.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Large", "Lg"));
            expected.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("X-Large", "XL"));

            expected.ProductOptions.Add(new ProductOption("Material"));
            expected.ProductOptions.First(x => x.Name == "Material").Choices.Add(new ProductAttribute("Cotton", "Cotton"));
            expected.ProductOptions.First(x => x.Name == "Material").Choices.Add(new ProductAttribute("Wool", "Wool"));

            _productService.Save(expected);

            //// Act
            var retrieved = _productService.GetByKey(key);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(3 == retrieved.ProductOptions.Count);
            Assert.IsTrue(3 == retrieved.ProductOptions.First(x => x.Name == "Color").Choices.Count);
            Assert.IsFalse(retrieved.IsDirty());
        }

        /// <summary>
        /// Verifies that a multiple products can be saved
        /// </summary>
        [Test]
        public void Can_Save_Multiple_Products()
        {
            //// Arrange
            var expected = 10;
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <remarks>
        ///// http://issues.merchello.com/youtrack/issue/M-561
        ///// M-866 makes this obsolete - changes behavior.
        ///// </remarks>
        //[Test]
        //public void Can_Verify_That_VariantsOnSale_Product_IsMarked_OnSale_Or_NotOnSale()
        //{
        //    //// Arrange
        //    var product = PreTestDataWorker.MakeExistingProduct();
        //    var key = product.Key;
        //    product.ProductOptions.Add(new ProductOption("Color"));

        //    product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Black"));
        //    product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Red", "Red"));
        //    product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Grey", "Grey"));


        //    _productService.Save(product);

        //    Assert.IsTrue(product.ProductVariants.Any());
        //    Assert.IsTrue(product.ProductVariants.All(x => !x.OnSale));
        //    Assert.IsFalse(product.OnSale);
            
        //    //// Act
        //    foreach (var variant in product.ProductVariants)
        //    {
        //        variant.OnSale = true;
        //    }
        //    _productService.Save(product);

        //    //// Assert
        //    Assert.IsTrue(product.ProductVariants.All(x => x.OnSale), "Not all variants are on sale");
        //    Assert.IsTrue(product.OnSale, "Product is not on sale");

        //    product.ProductVariants.First().OnSale = false;
        //    _productService.Save(product);
        //    Assert.IsFalse(product.ProductVariants.All(x => x.OnSale), "All variants are on sale");
        //    Assert.IsFalse(product.OnSale, "Product is on sale");

        //}

        /// <summary>
        /// Relates to http://issues.merchello.com/youtrack/issue/M-733
        /// </summary>
        [Test]
        public void Can_Update_Product_Options_Without_Breaking_Choices()
        {
            //// Arrange
            var product = MockProductDataMaker.MockProductForInserting(weight: 10M);
            product.ProductOptions.Add(new ProductOption("Color"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Color1", "Color1"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Color2", "Color2"));
            product.ProductOptions.Add(new ProductOption("Size"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Size1", "Size1"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Size2", "Size2"));

            //// Act
            _productService.Save(product);
            Assert.IsTrue(product.ProductVariants.Any());
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Color3", "Color3"));
            _productService.Save(product);

            Assert.IsTrue(product.ProductVariants.Any());
        }

        /// <summary>
        /// Relates to http://issues.merchello.com/youtrack/issue/M-733
        /// </summary>
        [Test]
        public void Simulates_IssueM733()
        {
            //// Arrange

            var newProduct = MockProductDataMaker.MockProductForInserting(weight: 10M);

            // add some dimensions
            newProduct.ProductOptions.Add(new ProductOption("Dimension"));
            newProduct.ProductOptions.First(x => x.Name == "Dimension").Choices.Add(new ProductAttribute("D1", "D1"));
            newProduct.ProductOptions.First(x => x.Name == "Dimension").Choices.Add(new ProductAttribute("D2", "D2"));
            newProduct.ProductOptions.First(x => x.Name == "Dimension").Choices.Add(new ProductAttribute("D3", "D3"));
            newProduct.ProductOptions.Add(new ProductOption("Fabric"));
            newProduct.ProductOptions.First(x => x.Name == "Fabric").Choices.Add(new ProductAttribute("F1", "F1"));
            newProduct.ProductOptions.First(x => x.Name == "Fabric").Choices.Add(new ProductAttribute("F2", "F2"));
            newProduct.Price = 18M;
            _productService.Save(newProduct);

            var sku = newProduct.Sku; // this is a partial Guid
            var variantSkus = newProduct.ProductVariants.Select(x => x.Sku).ToArray();  // An array of variant skus generated from the first save
            Console.WriteLine(string.Join(", ", variantSkus));

            newProduct.ProductOptions.First(x => x.Name == "Dimension").Choices.Add(new ProductAttribute("D4", "D4"));
            newProduct.Price = 20M;
            var removeChoice =
                newProduct.ProductOptions.First(x => x.Name == "Fabric").Choices.FirstOrDefault(x => x.Sku == "F1");
            newProduct.ProductOptions.First(x => x.Name == "Fabric").Choices.Remove(removeChoice);

            if (_productService.SkuExists(sku))
            {
                // Update product!
               Console.WriteLine("Update product!");
               var existingProduct = _productService.GetBySku(sku);

               // Check Dimensions
               var newDimensions = newProduct.ProductOptions.FirstOrDefault(x => x.Name == "Dimension").Choices;
               var existingDimensions = existingProduct.ProductOptions.FirstOrDefault(x => x.Name == "Dimension").Choices;
               foreach (var choice in newDimensions.Where(x => !existingDimensions.Contains(x.Sku)).ToList())
               {
                   existingDimensions.Add(choice);
               }
               foreach (var choice in existingDimensions.Where(x => !newDimensions.Contains(x.Sku)).ToList())
               {
                   existingDimensions.Remove(choice);
               }
               existingProduct.ProductOptions.FirstOrDefault(x => x.Name == "Dimension").Choices = existingDimensions;

               // Check Fabrics
               var newFabrics = newProduct.ProductOptions.FirstOrDefault(x => x.Name == "Fabric").Choices;
               var existingFabrics = existingProduct.ProductOptions.FirstOrDefault(x => x.Name == "Fabric").Choices;
               foreach (var choice in newFabrics.Where(x => !existingFabrics.Contains(x.Sku)).ToList())
               {
                   existingFabrics.Add(choice);
               }
               foreach (var choice in existingFabrics.Where(x => !newFabrics.Contains(x.Sku)).ToList())
               {
                   existingFabrics.Remove(choice);
               }

               existingProduct.Price = newProduct.Price;
               _productService.Save(existingProduct);
                Assert.IsTrue(existingProduct.ProductVariants.Any());
                Assert.AreEqual(20M, existingProduct.Price);
                var updatedVaraintSkus = existingProduct.ProductVariants.Select(x => x.Sku).ToArray();
                Console.WriteLine(string.Join(", ", updatedVaraintSkus));
            }
            else
            {
                Assert.Fail("The sku did not exist");
            }

        }
    }
}
