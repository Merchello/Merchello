using System;
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

        [SetUp]
        public void Init()
        {
            PreTestDataWorker.DeleteAllProducts();
            _productService = PreTestDataWorker.ProductService;
            _productVariantService = PreTestDataWorker.ProductVariantService;

            _product = PreTestDataWorker.MakeExistingProduct();
            _product.ProductOptions.Add(new ProductOption("Color"));
            _product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Black", "Blk"));
            _product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Blue", "Blu"));
            _product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Red", "Red"));
            _product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Green", "Gre"));
            _product.ProductOptions.Add(new ProductOption("Size"));
            _product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Small", "Sm"));
            _product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Medium", "M"));
            _product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Large", "Lg"));
            _product.ProductOptions["Size"].Choices.Add(new ProductAttribute("X-Large", "XL"));
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
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };

            //// Act
            var variant = _productVariantService.CreateVariantWithKey(_product, attributes);

            //// Assert
            Assert.IsTrue(variant.HasIdentity);
            
        }

        /// <summary>
        /// Test verifies that a product variant cannot be created twice because of sku
        /// </summary>
        [Test]
        public void Can_Not_Create_A_Duplicate_ProductVariant()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };
            var variant = _productVariantService.CreateVariantWithKey(_product, attributes);

            //// Act / Assert
            Assert.Throws<ArgumentException>(() => _productVariantService.CreateVariantWithKey(_product, attributes));
        }
    }
}