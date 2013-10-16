using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.Services;
using Merchello.Web.Models;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.ItemCache
{
    [TestFixture]
    public class BasketTests : ServiceIntegrationTestBase
    {
        private IMerchelloContext _merchelloContext;
        private ICustomerBase _customer;
        private IBasket _basket; 

        [SetUp]
        public void Init()
        {
            _merchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new ObjectCacheRuntimeCacheProvider(),
                                    new StaticCacheProvider(),
                                    new NullCacheProvider()));
            _customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            _basket = Basket.GetBasket(_merchelloContext, _customer);
        }

        /// <summary>
        /// Test verifies that a product can be added to a basket
        /// </summary>
        [Test]
        public void Can_Add_A_Product_To_A_Basket()
        {
            //// Arrange
            var product = PreTestDataWorker.MakeExistingProduct();

            //// Act
            _basket.AddItem(product.DefaultVariant);

            //// Assert
            Assert.IsFalse(_basket.Items.IsEmpty);
        }
    }
}
