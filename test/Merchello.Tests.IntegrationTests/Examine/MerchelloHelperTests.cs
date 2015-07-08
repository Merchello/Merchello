using System;
using System.Linq;
using Examine;
using Merchello.Core.Models;
using Merchello.Examine.Providers;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Examine
{
    using Merchello.Core;

    [TestFixture]
    public class MerchelloHelperTests : DatabaseIntegrationTestBase
    {
        private ProductIndexer _provider;
        private IWarehouse _warehouse;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();
            var warehouseService = PreTestDataWorker.WarehouseService;
            _warehouse = warehouseService.GetDefaultWarehouse();

            _provider = (ProductIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];
            _provider.RebuildIndex();
        }

        
        
        //[Test]
        //public void Can_GetAllProducts_From_Index()
        //{

        //    //// Arrange
        //    var merchello = new MerchelloHelper();

        //    //// Act
        //    var products = merchello.AllProducts();

        //    //// Assert
        //    Assert.IsTrue(products.Any());
        //}

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Can_RetrieveProductOptions_From_ProductInIndex()
        {            
            //// Arrange

            var merchello = new MerchelloHelper(MerchelloContext.Current.Services, false);

            var productService = PreTestDataWorker.ProductService;

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
            product.TrackInventory = true;
            product.AddToCatalogInventory(_warehouse.WarehouseCatalogs.First());
            productService.Save(product);
           
            foreach (var variant in product.ProductVariants)
            {
                variant.CatalogInventories.First().Count = 1;
            }
            productService.Save(product);

            foreach (var p in product.ProductVariants)
            {
                Assert.AreEqual(1, p.TotalInventoryCount, "Preindexed product variant count");
            }

            _provider.AddProductToIndex(product);

            //// Act
            var productDisplay = merchello.Query.Product.GetByKey(product.Key);

            //// Assert
            Assert.NotNull(productDisplay);
            Assert.IsTrue(productDisplay.ProductOptions.Any());

            //http://issues.merchello.com/youtrack/issue/M-604
            foreach (var variant in productDisplay.ProductVariants)
            {
                Assert.AreEqual(1, variant.TotalInventoryCount);
            }
            Assert.AreEqual(12, productDisplay.TotalInventoryCount, "Total inventory count failed");

        }

        [Test]
        public void Can_Retrieve_A_Product_From_The_Index_By_Sku()
        {
            //// Arrange

            var merchello = new MerchelloHelper(MerchelloContext.Current.Services, false);

            var productService = PreTestDataWorker.ProductService;

            var product = MockProductDataMaker.MockProductCollectionForInserting(1).First();
            product.Height = 11M;
            product.Width = 11M;
            product.Length = 11M;
            product.CostOfGoods = 15M;
            product.OnSale = true;
            product.SalePrice = 18M;
            productService.Save(product);


            _provider.AddProductToIndex(product);

            //// Act
            var productDisplay = merchello.Query.Product.GetBySku(product.Sku);

            //// Assert
            Assert.NotNull(productDisplay);
            Assert.AreEqual(product.Key, productDisplay.Key);
        }

        [Test]
        public void Can_Retrieve_A_ProductVariant_From_The_Index()
        {
            //// Arrange

            var merchello = new MerchelloHelper(MerchelloContext.Current.Services, false);

            var productService = PreTestDataWorker.ProductService;

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
            productService.Save(product);
            _provider.AddProductToIndex(product);

            Assert.IsTrue(product.ProductVariants.Any());
            var variant = product.ProductVariants.First();

            //// Act
            var productVariantDisplay = merchello.Query.Product.GetProductVariantBySku(variant.Sku);

            //// Assert
            Assert.NotNull(productVariantDisplay);
            Assert.AreEqual(variant.Key, productVariantDisplay.Key);
        }

        //[Test]
        //public void Can_Get_A_ProductFrom_The_IndexByKey()
        //{
        //    var key = new Guid("2f6d404b-fc0d-4305-a97c-9a5e1efe13a8");
        //    var merchello = new MerchelloHelper(MerchelloContext.Services);

        //    var product = merchello.Query.Product.GetByKey(key);

        //    Assert.NotNull(product);
        //}

        //[Test]
        //public void Can_GetGetIguanas_From_Index()
        //{
        //    //// Arrange
        //    var merchello = new MerchelloHelper();

        //    //// Act
        //    var searched = merchello.SearchProducts("princess");
        //    var result = searched.FirstOrDefault();

        //    //// Assert
        //    Assert.IsTrue(searched.Any());
        //    Console.WriteLine(searched.Count());


        //}
    }
}