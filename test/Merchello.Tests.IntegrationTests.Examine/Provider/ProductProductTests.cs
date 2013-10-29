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
    public class ProductProductTests : ServiceIntegrationTestBase
    {
        private const int ProductCount = 100;
        private IProductService _productService;
        private IProduct _randomProduct;

        [SetUp]
        public void Init()
        {
            _productService = PreTestDataWorker.ProductService;
           // PreTestDataWorker.DeleteAllProducts();            

           // var products = MockProductDataMaker.MockProductCollectionForInserting(ProductCount);
           //_productService.Save(products);

           //_randomProduct = products.OrderBy(x => Guid.NewGuid()).First();
        }

        [Test]
        public void Can_Index_Products()
        {
            //// Arrange
            BaseMerchelloIndexer.DisableInitializationCheck = true;
            var timer = new Stopwatch();
            timer.Start();
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();            
            timer.Stop();

           //// Act
            Console.Write("Time to index: " + timer.Elapsed.ToString());

        }

        [Test]
        public void Can_Add_A_New_Product_To_The_Index()
        {
            //// Arrange            
            BaseMerchelloIndexer.DisableInitializationCheck = true;
            var provider = (MerchelloProductIndexer) ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];

            MerchelloSearcher.DisableInitializationCheck = true;
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