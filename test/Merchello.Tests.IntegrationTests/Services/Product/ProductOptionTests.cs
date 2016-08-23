using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Product
{
    using Merchello.Core;
    using Merchello.Tests.Base.TestHelpers;

    [TestFixture]
    [Category("Service Integration")]
    public class ProductOptionTests : DatabaseIntegrationTestBase
    {
        private IProductService _productService;

        [SetUp]          
        public void Init()
        {
            PreTestDataWorker.DeleteAllProducts();
            _productService = PreTestDataWorker.ProductService;
        }

        /// <summary>
        /// Test to verify that adding an option to a product updated the product property "DefinesOptions" to true
        /// </summary>
        [Test]
        public void Can_Adding_An_Option_Changes_Product_Property_DefinesOptions_To_True()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();

            //// Act
            var option = new ProductOption("Color");
            product.ProductOptions.Add(option);
            product.ProductOptions.Add(new ProductOption("Size"));
            _productService.Save(product);

            //// Assert
            Assert.IsTrue(product.ProductOptions.Any());
            Assert.IsTrue(product.DefinesOptions);
        }

        /// <summary>
        /// Test to verify an option can be removed
        /// </summary>
        [Test]
        public void Can_Remove_An_Option_At_Index_2()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();
            product.ProductOptions.Add(new ProductOption("Option1"));
            product.ProductOptions.Add(new ProductOption("Option2"));
            
            var removeItem = new ProductOption("Option3");
            product.ProductOptions.Add(removeItem);

            product.ProductOptions.Add(new ProductOption("Option4"));
            _productService.Save(product);
            Assert.IsTrue(product.ProductOptions.Count == 4);
            
            //// Act
            product.ProductOptions.RemoveAt(2);
            _productService.Save(product);

            //// Assert
            Assert.IsTrue(product.ProductOptions.Count == 3);
        }

        /// <summary>
        /// Test to verify an option can be removed by name
        /// </summary>
        [Test]
        public void Can_Remove_An_Option_By_The_Option()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();
            product.ProductOptions.Add(new ProductOption("Option1"));
            product.ProductOptions.Add(new ProductOption("Option2"));

            var removeItem = new ProductOption("Option3");
            product.ProductOptions.Add(removeItem);

            product.ProductOptions.Add(new ProductOption("Option4"));
            _productService.Save(product);
            Assert.AreEqual(4, product.ProductOptions.Count, "There should be 4 options");

            //// Act
            product.ProductOptions.Remove(removeItem);

            //// Assert
            Assert.AreEqual(3, product.ProductOptions.Count, "There should be 3 options");
        }

        /// <summary>
        /// Test to verify that choices can be added to a product option
        /// </summary>
        [Test]
        public void Can_Add_Choices_To_An_Option()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();
            product.ProductOptions.Add(new ProductOption("Color"));
            
            //// Act
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Black"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Blue"));

            //// Assert
            Assert.IsTrue(product.ProductOptions.First(x => x.Name == "Color").Choices.Any());
        }

        /// <summary>
        /// Test to verify that choices can be added to a product option and persisted to the db
        /// </summary>
        [Test]
        public void Can_Add_And_Save_Choices_To_An_Option()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();
            product.ProductOptions.Add(new ProductOption("Color"));

            //// Act
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Black"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Blue"));
            _productService.Save(product);

            //// Assert
            Assert.IsTrue(product.ProductOptions.First(x => x.Name == "Color").Choices.Any());   
        }

        /// <summary>
        /// Test to verify that choices can be removed from an option
        /// </summary>
        [Test]
        public void Can_Remove_A_Choice_From_An_Option()
        {
            //// Arrange
            var catalog = PreTestDataWorker.WarehouseCatalog;
            var product = PreTestDataWorker.MakeExistingProduct();
            product.ProductOptions.Add(new ProductOption("Color"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Black"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blue"));
            var removeChoice = new ProductAttribute("Grey", "Grey");
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(removeChoice);
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Green", "Green"));

            product.AddToCatalogInventory(catalog);

            _productService.Save(product);

            Assert.AreEqual(1, product.ProductOptions.Count, "ProductOption count is not 1");
            Assert.AreEqual(4, product.ProductVariants.Count, "ProductVariant count is not 4");

            //// Act
            product.ProductOptions.First(x => x.Name == "Color").Choices.RemoveItem(removeChoice.Key);
            _productService.Save(product);

            // Assert
            Assert.IsTrue(product.ProductOptions.First(x => x.Name == "Color").Choices.Count == 3);   
            Assert.AreEqual(3, product.ProductVariants.Count, "ProductVariant count is not 3");
        }

        /// <summary>
        /// Test to verify that product options & variants can be manipulated correctly 
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Create_A_Product_Then_Add_Options_And_Modify_Choices1()
        {
            //// Arrange
            var catalog = PreTestDataWorker.WarehouseCatalog;
            var product = PreTestDataWorker.MakeExistingProduct();
            product.ProductOptions.Add(new ProductOption("Color"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Black"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blue"));
            var removeChoice = new ProductAttribute("Grey", "Grey");
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(removeChoice);
            product.ProductOptions.Add(new ProductOption("Size"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Large", "Lg"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Sm"));

            // this creates a record in the CatalogInventory as if the product was marked "shippable"
            // all variants generated with this option will also be marked shippable.
            product.AddToCatalogInventory(catalog);

            _productService.Save(product);

            Assert.AreEqual(2, product.ProductOptions.Count, "ProductOption count is not 2");
            Assert.AreEqual(6, product.ProductVariants.Count, "ProductVariant count is not 6");

            //// Remove a choice
            product.ProductOptions.First(x => x.Name == "Color").Choices.Remove(removeChoice);
            _productService.Save(product);

            // Assert the variants
            Assert.AreEqual(2, product.ProductOptions.Count, "ProductOption count is not 2");
            Assert.AreEqual(4, product.ProductVariants.Count, "ProductVariant count is not 4");

            //// Add a new size
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Medium", "Md"));

            _productService.Save(product);
            Assert.AreEqual(2, product.ProductOptions.Count, "ProductOption count is not 2");
            Assert.AreEqual(6, product.ProductVariants.Count, "ProductVariant count is not 6");

            //// Add a new product option
            product.ProductOptions.Add(new ProductOption("Material"));
            product.ProductOptions.First(x => x.Name == "Material").Choices.Add(new ProductAttribute("Wool", "Wool"));
            product.ProductOptions.First(x => x.Name == "Material").Choices.Add(new ProductAttribute("Cotton", "Cotton"));
            _productService.Save(product);

            Assert.AreEqual(3, product.ProductOptions.Count, "ProductOption count is not 3");
            Assert.AreEqual(12, product.ProductVariants.Count, "ProductVariant count is not 12");
        }
       
    }
}