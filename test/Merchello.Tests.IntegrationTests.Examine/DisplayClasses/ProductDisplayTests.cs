using System;
using System.Linq;
using Examine;
using Examine.SearchCriteria;
using Merchello.Core.Models;
using Merchello.Examine.Providers;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.Services;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;
using umbraco.presentation.dialogs;

namespace Merchello.Tests.IntegrationTests.Examine.DisplayClasses
{
    [TestFixture]
    public class ProductDisplayTests : ServiceIntegrationTestBase
    {
        private Guid _productKey;

        [SetUp]
        public void Init()
        {
            //// Arrange            
            var provider = (ProductIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];

            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];

            var productVariantService = PreTestDataWorker.ProductVariantService;
            var productService = PreTestDataWorker.ProductService;

            //// Act
            var product = MockProductDataMaker.MockProductCollectionForInserting(1).First();
            product.ProductOptions.Add(new ProductOption("Color"));
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Blue", "Blue"));
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Red", "Red"));
            product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Green", "Green"));
            product.ProductOptions.Add(new ProductOption("Size"));
            product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Small", "Sm"));
            product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Medium", "Med"));
            product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Large", "Lg"));
            product.ProductOptions["Size"].Choices.Add(new ProductAttribute("X-Large", "XL"));
            product.Height = 11M;
            product.Width = 11M;
            product.Length = 11M;
            product.CostOfGoods = 15M;
            product.OnSale = true;
            product.SalePrice = 18M;
            productService.Save(product);

            _productKey = product.Key;

            var attributes = new ProductAttributeCollection()
            {
                product.ProductOptions["Color"].Choices["Blue"],
                product.ProductOptions["Size"].Choices["XL"]
            };

            productVariantService.CreateProductVariantWithId(product, attributes);

            provider.AddProductToIndex(product);
        }

        [Test]
        public void Can_Build_ProductVariantDisplay_From_Indexed_Product()
        {
            //// Arrange
            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];
            var criteria = searcher.CreateSearchCriteria("productvariant", BooleanOperation.And);
            criteria.Field("productKey", _productKey.ToString()).And().Field("master", "true");

            var result = searcher.Search(criteria).FirstOrDefault();

            //// Act
            var productVariantDisplay = result.ToProductVariantDisplay();

            //// Assert
            Assert.NotNull(productVariantDisplay);
        }
    }
}