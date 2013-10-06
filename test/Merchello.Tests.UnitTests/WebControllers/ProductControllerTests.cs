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
using System.Web.Http.Hosting;
using Merchello.Tests.Base.DataMakers;

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
			Guid productKey = Guid.NewGuid();
            ProductActual productActual = MockProductDataMaker.MockProductComplete(productKey) as ProductActual;

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.GetByKey(productKey)).Returns(productActual);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            var result = ctrl.GetProduct(productKey);

            //// Assert
			Assert.AreEqual(productActual, result);
        }

        /// <summary>
        /// Test to verify that the API throws an exception when param is not found
        /// </summary>
        [Test]
        public void GetProductThrowsWhenRepositoryReturnsNull()
        {
            //// Arrange
			Guid productKey = Guid.NewGuid();

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.GetByKey(productKey)).Returns((ProductActual)null);

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
			Guid productKey = Guid.NewGuid();
            ProductActual productActual = CreateFakeProduct(productKey, 20.0M);

			Guid productKey2 = Guid.NewGuid();
            ProductActual product2 = CreateFakeProduct(productKey2, 30.0M);

			Guid productKey3 = Guid.NewGuid();
            ProductActual product3 = CreateFakeProduct(productKey3, 40.0M);

            List<ProductActual> productsList = new List<ProductActual>();
            productsList.Add(productActual);
            productsList.Add(product3);

            var productKeys = new[] { productKey, productKey3 };

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.GetByKeys(productKeys)).Returns(productsList);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            var result = ctrl.GetProducts(productKeys);

            //// Assert
			Assert.AreEqual(productsList, result);
        }
        
        /// <summary>
        /// Test to verify that the repository is updated on a PUT
        /// </summary>
        [Test]
        public void PutProductUpdatesRepository()
        {
            //// Arrange
            bool wasCalled = false;
			Guid productKey = Guid.NewGuid();
            ProductActual productActual = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.Save(productActual, It.IsAny<bool>())).Callback(() => wasCalled = true);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);
            ctrl.Request = new HttpRequestMessage();
            ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            //// Act
            HttpResponseMessage response = ctrl.PutProduct(productActual);

            //// Assert
			Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            Assert.True(wasCalled);
        }

        /// <summary>
        /// Test to verify that the proper error response is returned on an error
        /// </summary>
        [Test]
        public void PutProductReturns500WhenRepositoryUpdateReturnsError()
        {
            //// Arrange
			Guid productKey = Guid.NewGuid();
            ProductActual productActual = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.Save(productActual, It.IsAny<bool>())).Throws<InvalidOperationException>();

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);
            ctrl.Request = new HttpRequestMessage();
            ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            //// Act
            HttpResponseMessage response = ctrl.PutProduct(productActual);

            //// Assert
			Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
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
            ProductActual productActual = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.Delete(productActual, It.IsAny<bool>())).Callback<IProductActual, bool>((p, b) => removedKey = p.Key);
            MockProductService.Setup(cs => cs.GetByKey(productKey)).Returns(productActual);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);
            ctrl.Request = new HttpRequestMessage();
            ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            //// Act
            HttpResponseMessage response = ctrl.Delete(productKey);

            //// Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

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
			Guid productKey = Guid.NewGuid();
            ProductActual productActual = CreateFakeProduct(productKey, 20.0M);

            var MockProductService = new Mock<IProductService>();
            MockProductService.Setup(cs => cs.CreateProduct(productActual.Sku, productActual.Name, productActual.Price)).Returns(productActual).Callback(() => wasCalled = true);

            MerchelloContext merchelloContext = GetMerchelloContext(MockProductService.Object);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            ProductActual result = ctrl.NewProduct(productActual.Sku, productActual.Name, productActual.Price);

            //// Assert
            Assert.AreEqual(productActual, result);
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

            return new MerchelloContext(MockServiceContext.Object);
        }

        #endregion

        #region ProductSetup

        /// <summary>
        /// Create a fake product with fake data for testing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ProductActual CreateFakeProduct(Guid key, Decimal price)
        {
            return new ProductActual
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
