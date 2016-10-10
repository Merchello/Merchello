using System;
using System.Diagnostics;
using System.Linq;
using Examine;
using Examine.SearchCriteria;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine.Providers;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Examine
{
    using System.Threading;

    [TestFixture]
    public class ProductProviderTests : DatabaseIntegrationTestBase
    {
        private const int ProductCount = 2;
        private IProductService _productService;
        
        [SetUp]
        public void Init()
        {
            _productService = PreTestDataWorker.ProductService;
        }

        /// <summary>
        /// Test to verify that the product index can be rebuilt
        /// </summary>
        [Test]
        public void Can_Rebuild_Product_Index()
        {
            // Arrange
            PreTestDataWorker.DeleteAllProducts();
            var products = MockProductDataMaker.MockProductCollectionForInserting(ProductCount);
            _productService.Save(products);

            //// Act
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

        // This is troubleshooting a community member issue - and data is specific to their database
        // DO NOT USE
        //public void Can_Add_Reindex_An_Existing_Product()
        //{
        //    var product = PreTestDataWorker.ProductService.GetByKey(new Guid("2f6d404b-fc0d-4305-a97c-9a5e1efe13a8"));
        //    Assert.NotNull(product);

        //    var provider = (ProductIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];

        //    provider.AddProductToIndex(product);

        //    var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];

        //    //// Assert
        //    var criteria = searcher.CreateSearchCriteria("productvariant", BooleanOperation.And);
        //    criteria.Field("productKey", product.Key.ToString()).And().Field("master", "true");

        //    ISearchResults results = searcher.Search(criteria);
        //    Assert.IsTrue(results.Count() == 1);
        //    var result = results.First();

        //}

        /// <summary>
        /// Test verifies that a new product can be added to an existing index
        /// </summary>
        [Test, Category("LongRunning")]
        public void Can_Add_A_New_Product_To_The_Index()
        {
            PreTestDataWorker.DeleteAllProducts();
            
            //// Arrange            
            var provider = (ProductIndexer) ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];
            provider.RebuildIndex();

            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];

            var productVariantService = PreTestDataWorker.ProductVariantService;

            //// Act
            var product = MockProductDataMaker.MockProductCollectionForInserting(1).First();
            product.ProductOptions.Add(new ProductOption("Color"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blue"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Red", "Red"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Green", "Green"));
            product.ProductOptions.Add(new ProductOption("Size"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Sm"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Medium", "Med"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Large", "Lg"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("X-Large", "XL"));
            product.Height = 11M;
            product.Width = 11M;
            product.Length = 11M;
            product.CostOfGoods = 15M;
            product.OnSale = true;
            product.SalePrice = 18M;
            product.Manufacturer = "Merchello";
            _productService.Save(product);
            
            provider.AddProductToIndex(product);

            //// Assert
            var criteria = searcher.CreateSearchCriteria("productvariant", BooleanOperation.And);
            criteria.Field("productKey", product.Key.ToString()).And().Field("master", "true");

            ISearchResults results = searcher.Search(criteria);

            Assert.IsTrue(results.Count() == 1);
            var result = results.First();
            Assert.NotNull(result.Fields["productOptions"]);
            Assert.AreEqual("Merchello", result.Fields["manufacturer"]);
            provider.RebuildIndex();
            
        }

        /// <summary>
        /// Test verifies that a product can be deleted from the index
        /// </summary>
        [Test]
        public void Can_Remove_A_Product_From_The_Index()
        {
            //// Arrange            
            var provider = (ProductIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];

            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];
            var criteria = searcher.CreateSearchCriteria(Merchello.Examine.IndexTypes.ProductVariant);
            criteria.Field("allDocs", "1");
            var results = searcher.Search(criteria);

            var providerKey = results.Select(x => x.Fields["productKey"]).FirstOrDefault();
            Assert.IsNotNullOrEmpty(providerKey);

            var key = new Guid(providerKey);

            var product = _productService.GetByKey(key);

            //// Act
            provider.DeleteProductFromIndex(product);

            Thread.Sleep(200);

            //// Assert
            criteria = searcher.CreateSearchCriteria("productvariant", BooleanOperation.And);
            criteria.Field("productKey", product.Key.ToString()).And().Field("master", "true");

            results = searcher.Search(criteria);

            Assert.IsFalse(results.Any());

        }
    }
}