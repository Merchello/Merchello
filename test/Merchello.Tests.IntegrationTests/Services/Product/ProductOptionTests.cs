using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Product
{
    [TestFixture]
    [Category("ProductService Integration")]
    public class ProductOptionTests : ServiceIntegrationTestBase
    {
        private IProductService _productService;

        [SetUp]          
        public void Init()
        {
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
            _productService.Save(product);

            //// Assert
            Assert.IsTrue(product.ProductOptions.Any());
            Assert.IsTrue(product.DefinesOptions);
        }

        /// <summary>
        /// Test to verify that choices can be added to a product option
        /// </summary>
        [Test]
        public void Can_Add_Choices_To_An_Option()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();
            //_productService.SaveProductOption(product, "Color");

            //// Act
            
        }

    }
}