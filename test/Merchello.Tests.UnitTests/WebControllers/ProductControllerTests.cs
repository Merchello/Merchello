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

            var MockServiceContext = new Mock<IServiceContext>();
            MockServiceContext.SetupGet(sc => sc.ProductService).Returns(MockProductService.Object);

            MerchelloContext merchelloContext = new MerchelloContext(MockServiceContext.Object, null);

            ProductApiController ctrl = new ProductApiController(merchelloContext, tempUmbracoContext);

            //// Act
            var result = ctrl.GetProduct(productKey);

            //// Assert
            Assert.AreEqual(result, product);
        }

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
