using System;
using Merchello.Core;
using Merchello.Tests.Base.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Product
{
    [TestFixture]
    public class ProductAllinTests : MerchelloAllInTestBase
    {
        [TestFixtureSetUp]
        public void Init()
        {
            DbPreTestDataWorker.DeleteAllProducts();
        }

        [Test]
        public void Can_Add_A_New_Product()
        {
            //// Arrange
            const string name = "Test 1";
            const string sku = "test-1-sku";
            const decimal price = 19M;

            //// Act
            var product = MerchelloContext.Current.Services.ProductService.CreateProductWithKey(name, sku, price);

            //// Assert
            Assert.IsTrue(product.HasIdentity);
            Console.WriteLine(product.Key.ToString());
        }


        [Test]
        public void Can_Add_A_Product_With_Addition_Properties()
        {
            //// Arrange
            const string name = "Test 2";
            const string sku = "test-2-sku";
            const decimal price = 19M;

            //// Act
            var productService = MerchelloContext.Current.Services.ProductService;

            var product = productService.CreateProduct(name, sku, price);
            Assert.IsFalse(product.HasIdentity, "The product was saved");

            product.TrackInventory = false;
            product.Shippable = true;
            product.Weight = 10M;
            product.Manufacturer = "Mindfly";

            productService.Save(product);

            //// Assert
            Assert.IsTrue(product.HasIdentity);
            Console.WriteLine(product.Key.ToString());

        }

    }
}