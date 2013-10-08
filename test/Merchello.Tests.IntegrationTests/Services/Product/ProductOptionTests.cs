using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Product
{
    [TestFixture]
    [Category("Service Integration")]
    public class ProductOptionTests : ServiceIntegrationTestBase
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
            Assert.IsTrue(product.ProductOptions.Count == 4);

            //// Act
            product.ProductOptions.Remove(removeItem);

            //// Assert
            Assert.IsTrue(product.ProductOptions.Count == 3);
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
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Black", "Black"));
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Black", "Blue"));

            //// Assert
            Assert.IsTrue(product.ProductOptions["Color"].Choices.Any());
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
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Black", "Black"));
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Black", "Blue"));
            _productService.Save(product);

            //// Assert
            Assert.IsTrue(product.ProductOptions["Color"].Choices.Any());   
        }

        /// <summary>
        /// Test to verify that choices can be removed from an option
        /// </summary>
        [Test]
        public void Can_Remove_A_Choice_From_An_Option()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();
            product.ProductOptions.Add(new ProductOption("Color"));
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Black", "Black"));
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Blue", "Blue"));
            var removeChoice = new ProductAttribute("Grey", "Grey");
            product.ProductOptions["Color"].Choices.Add(removeChoice);
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Green", "Green"));
            _productService.Save(product);

            //// Act
            product.ProductOptions["Color"].Choices.Remove(removeChoice);
            _productService.Save(product);
           
            //// Assert
            Assert.IsTrue(product.ProductOptions["Color"].Choices.Count == 3);   
            
        }
       
    }
}