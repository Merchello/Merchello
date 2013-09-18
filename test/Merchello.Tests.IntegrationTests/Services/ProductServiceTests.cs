using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class ProductServiceTests  : BaseUsingSqlServerSyntax
    {
        private IProductService _productService;

        [SetUp]
        public void Setup()
        {
            _productService = new ProductService();
        }

        [Test]
        public void Can_Create_A_Product()
        {
            var sku = ProductData.MockSku();
            
            var expected = ProductData.MockProductForInserting(sku);

            var product = _productService.CreateProduct(sku, "fake product", 15.00m);

          Assert.AreEqual(expected.Key, product.Key);
        }

        [Test]
        public void Can_Save_A_Product()
        {
            var product = ProductData.MockProductForInserting(ProductData.MockSku());

            _productService.Save(product);

            Assert.IsTrue(product.HasIdentity);
        }


        [Test]
        public void Can_Save_Multiple_Products()
        {
            IEnumerable<IProduct> savedProducts = new List<IProduct>();
            ProductService.Saved += delegate(IProductService sender, SaveEventArgs<IProduct> args)
            {
                savedProducts = args.SavedEntities;
            };

            var products = new List<IProduct>()
            {
                ProductData.MockProductForInserting(ProductData.MockSku()),
                ProductData.MockProductForInserting(ProductData.MockSku()),
                ProductData.MockProductForInserting(ProductData.MockSku()),
                ProductData.MockProductForInserting(ProductData.MockSku())
            };

            _productService.Save(products);

            Assert.IsTrue(savedProducts.Any());
            
        }

        [Test]
        public void Can_Delete_A_Product()
        {
            IProduct deletedProduct = null;
            ProductService.Deleted += delegate(IProductService sender, DeleteEventArgs<IProduct> args)
            {
                deletedProduct = args.DeletedEntities.FirstOrDefault();
            };

            var product = ProductData.MockProductForInserting(ProductData.MockSku());
            _productService.Save(product);

            Assert.IsTrue(product.HasIdentity);

            var key = product.Key;

            _productService.Delete(product);

            Assert.NotNull(deletedProduct);
            Assert.AreEqual(key, deletedProduct.Key);
        }

        [Test]
        public void Can_Update_A_Product()
        {
            IProduct updatedProduct = null;
            ProductService.Saved += delegate(IProductService sender, SaveEventArgs<IProduct> args)
                {
                    updatedProduct = args.SavedEntities.FirstOrDefault();
                };

            var product = ProductData.MockProductForInserting(ProductData.MockSku());

            _productService.Save(product);

            Assert.IsTrue(product.HasIdentity);

            updatedProduct = null;

            var key = product.Key;

            product.Name = "Updated Product";

            product.CostOfGoods = 9.00m;
            _productService.Save(product);

            Assert.NotNull(updatedProduct);
            Assert.AreEqual(updatedProduct.Name, product.Name);
            Assert.AreEqual("Updated Product", product.Name);
            Assert.AreEqual(9.00m, product.CostOfGoods);

        }
        
    }
}
