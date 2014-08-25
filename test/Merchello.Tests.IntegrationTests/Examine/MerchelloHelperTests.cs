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
    [TestFixture]
    public class MerchelloHelperTests : DatabaseIntegrationTestBase
    {
        private ProductIndexer _provider;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

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
            
            var merchello = new MerchelloHelper(MerchelloContext.Services);

            var productVariantService = PreTestDataWorker.ProductVariantService;
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


            var attributes = new ProductAttributeCollection()
            {
                product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blue" ),
                product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "XL")
            };

            productVariantService.CreateProductVariantWithKey(product, attributes);

            _provider.AddProductToIndex(product);

            //// Act
            var productDisplay = merchello.Query.Product.GetByKey(product.Key);

            //// Assert
            Assert.NotNull(productDisplay);
            Assert.IsTrue(productDisplay.ProductOptions.Any());

        }

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