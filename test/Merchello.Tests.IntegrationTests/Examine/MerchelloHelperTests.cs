using System;
using System.Linq;
using Examine;
using Merchello.Examine.Providers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Examine
{
    [TestFixture]
    public class MerchelloHelperTests : DatabaseIntegrationTestBase
    {
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            var provider = (ProductIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];
            provider.RebuildIndex();
        }

        
        
        [Test]
        public void Can_GetAllProducts_From_Index()
        {

            //// Arrange
            var merchello = new MerchelloHelper();

            //// Act
            var products = merchello.AllProducts();

            //// Assert
            Assert.IsTrue(products.Any());
        }

        [Test]
        public void Can_GetGetIguanas_From_Index()
        {
            //// Arrange
            var merchello = new MerchelloHelper();

            //// Act
            var searched = merchello.SearchProducts("maple");
            var result = searched.FirstOrDefault();

            //// Assert
            Assert.IsTrue(searched.Any());
            Console.WriteLine(searched.Count());


        }
    }
}