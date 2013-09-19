using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using Moq;
using Merchello.Core;
using Merchello.Core.Services;
using Merchello.Core.Models;
using Merchello.Web.Editors;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Tests.TestHelpers;
using System.Web.Http;
using System.Net.Http;

namespace Merchello.Tests.UnitTests.WebControllers
{
    [TestFixture]
    class ProductControllerTests : BaseRoutingTest
    {
        UmbracoContext tempUmbracoContext;

        protected override DatabaseBehavior DatabaseTestBehavior
        {
            get { return DatabaseBehavior.NoDatabasePerFixture; }
        }

        [SetUp]
        public void Setup()
        {
            tempUmbracoContext = GetRoutingContext("/test", 1234).UmbracoContext;
        }

        /// <summary>
        /// Test to verify that the API gets the correct Product by Key
        /// </summary>
        [Test]
        public void GetProductByKeyReturnsCorrectItemFromRepository()
        {
            //// Arrange
            Guid productKey = new Guid();
            Product product = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.GetByKey(productKey)).Returns(product);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            var result = ctrl.GetProduct(productKey);

            //// Assert
            Assert.AreEqual(result, product);
        }

        /// <summary>
        /// Test to verify that the API throws an exception when param is not found
        /// </summary>
        [Test]
        public void GetProductThrowsWhenRepositoryReturnsNull()
        {
            //// Arrange
            Guid productKey = new Guid();

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.GetByKey(productKey)).Returns((Product)null);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act & Assert
            var ex = Assert.Throws<HttpResponseException>(() => ctrl.GetProduct(Guid.Empty));
        }

        /// <summary>
        /// Test to verify that the API gets the correct Products from the passed in Keys
        /// </summary>
        [Test]
        public void GetProductByKeysReturnsCorrectItemsFromRepository()
        {
            //// Arrange
            Guid productKey = new Guid();
            Product product = CreateFakeProduct(productKey, 20.0M);

            Guid productKey2 = new Guid();
            Product product2 = CreateFakeProduct(productKey2, 30.0M);

            Guid productKey3 = new Guid();
            Product product3 = CreateFakeProduct(productKey3, 40.0M);

            List<Product> productsList = new List<Product>();
            productsList.Add(product);
            productsList.Add(product3);

            var productKeys = new[] { productKey, productKey3 };

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.GetByKeys(productKeys)).Returns(productsList);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            var result = ctrl.GetProducts(productKeys);

            //// Assert
            Assert.AreEqual(result, productsList);
        }
        
        /// <summary>
        /// Test to verify that the repository is updated on a PUT
        /// </summary>
        [Test]
        public void PutProductUpdatesRepository()
        {
            //// Arrange
            bool wasCalled = false;
            Guid productKey = new Guid();
            Product product = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.Save(product, It.IsAny<bool>())).Callback(() => wasCalled = true);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            HttpResponseMessage response = ctrl.SaveProduct(product);

            //// Assert
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);

            Assert.True(wasCalled);
        }

        /// <summary>
        /// Test to verify that the proper error response is returned on an error
        /// </summary>
        [Test]
        public void PutProductReturns500WhenRepositoryUpdateReturnsError()
        {
            //// Arrange
            Guid productKey = new Guid();
            Product product = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.Save(product, It.IsAny<bool>())).Throws<InvalidOperationException>();

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            HttpResponseMessage response = ctrl.SaveProduct(product);

            //// Assert
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Test to verify that the delete is called
        /// </summary>
        [Test]
        public void DeleteProductCallsRepositoryRemove()
        {
            //// Arrange
            Guid removedKey = Guid.Empty;

            Guid productKey = new Guid();
            Product product = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.Delete(product, It.IsAny<bool>())).Callback<IProduct, bool>((p, b) => removedKey = p.Key);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            HttpResponseMessage response = ctrl.Delete(productKey);

            //// Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);

            Assert.AreEqual(productKey, removedKey);
        }

        /// <summary>
        /// Test to verify that the product is created
        /// </summary>
        [Test]
        public void NewProductReturnsCorrectProduct()
        {
            //// Arrange
            bool wasCalled = false;
            Guid productKey = new Guid();
            Product product = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.CreateProduct(product.Sku, product.Name, product.Price)).Returns(product).Callback(() => wasCalled = true);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            Product result = ctrl.NewProduct(product.Sku, product.Name, product.Price);

            //// Assert
            Assert.AreEqual(product, result);
            Assert.True(wasCalled);
        }

        #region ServicesSetup

        /// <summary>
        /// Setup the Mocks and get a MerchelloContext
        /// </summary>
        /// <param name="mockProductService"></param>
        /// <returns>MerchelloContext</returns>
        private MerchelloContext GetMerchelloContext(IProductService mockProductService)
        {
            var MockServiceContext = new Mock<IServiceContext>();
            MockServiceContext.SetupGet(sc => sc.ProductService).Returns(mockProductService);

            return new MerchelloContext(MockServiceContext.Object, null);
        }

        #endregion

        #region ProductSetup

        /// <summary>
        /// Create a fake product with fake data for testing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Product CreateFakeProduct(Guid key, Decimal price)
        {
            return new Product
            {
                Key = key,
                Name = String.Format("Product {0}", key),
                Weight = 5.0M, Width = 15.0M, Length = 22.0M,
                Price = price,
                CostOfGoods = 5.0M,
                SalePrice = 18.0M,
                Taxable = false, Shippable = true, Download = false, Template = true };
        }
		 
	    #endregion
        
    }
}
