using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Examine;
using Examine.SearchCriteria;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine.Providers;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.Services;
using NUnit.Framework;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Tests.IntegrationTests.Examine.Provider
{
    [TestFixture]
    public class ProductProviderTests : ServiceIntegrationTestBase
    {
        private const int ProductCount = 10;
        private IProductService _productService;
        
        [SetUp]
        public void Init()
        {
            _productService = PreTestDataWorker.ProductService;
        }

        [Test]
        public void Can_Index_Products()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllProducts();
            var products = MockProductDataMaker.MockProductCollectionForInserting(ProductCount);
            _productService.Save(products);

            //// Act
            BaseIndexer.DisableInitializationCheck = true;
            var timer = new Stopwatch();
            timer.Start();
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();            
            timer.Stop();
            Console.Write("Time to index: " + timer.Elapsed.ToString());
            
            //// Assert
            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];
            var criteria = searcher.CreateSearchCriteria(Merchello.Examine.IndexTypes.ProductVariant);
            criteria.Field("allDocs", "1");
            var results = searcher.Search(criteria);

            Assert.AreEqual(products.Count(), results.Count());

        }

        [Test]
        public void Can_Add_A_New_Product_To_The_Index()
        {
            //// Arrange            
            BaseIndexer.DisableInitializationCheck = true;
            var provider = (ProductIndexer) ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];

            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];

            //// Act
            var product = MockProductDataMaker.MockProductCollectionForInserting(1).First();
            _productService.Save(product);
            provider.AddProductToIndex(product);
            


            //// Assert
            var criteria = searcher.CreateSearchCriteria("productvariant", BooleanOperation.And);
            criteria.Field("productKey", product.Key.ToString()).And().Field("master", "true");

            ISearchResults results = searcher.Search(criteria);

            Assert.IsTrue(results.Count() == 1);
        }
    }
}