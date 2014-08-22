using System;
using System.Linq;
using Examine;
using Examine.SearchCriteria;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Examine.Providers;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.DisplayClasses
{
    using Merchello.Core.Services;
    using Merchello.Web;

    using Moq;

    [TestFixture]
    public class ProductDisplayTests : DatabaseIntegrationTestBase
    {
        private Guid _productKey;
        private Guid _productVariantKey;
        private int _examineId;
        private IWarehouse _warehouse;

        [SetUp]
        public void Init()
        {

            var warehouseService = PreTestDataWorker.WarehouseService;
            _warehouse = warehouseService.GetDefaultWarehouse();

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
            product.Manufacturer = "Nike";
            product.ManufacturerModelNumber = "N01-012021-A";
            product.TrackInventory = true;
            productService.Save(product);

            _productKey = product.Key;

            var attributes = new ProductAttributeCollection()
            {
                product.ProductOptions.First(x => x.Name == "Color").Choices.First(x => x.Sku == "Blue"),
                product.ProductOptions.First(x => x.Name == "Size").Choices.First(x => x.Sku == "XL" )
            };

            var variant = productVariantService.CreateProductVariantWithKey(product, attributes);
            variant.AddToCatalogInventory(_warehouse.DefaultCatalog());
            productVariantService.Save(variant);
            _productVariantKey = variant.Key;
            _examineId = ((ProductVariant) variant).ExamineId;

            var provider = (ProductIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"];
            provider.AddProductToIndex(product);
        }

        [Test]
        public void Can_Build_ProductVariantDisplay_From_Indexed_ProductVariant()
        {
            //// Arrange
            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];
            var criteria = searcher.CreateSearchCriteria("productvariant", BooleanOperation.And);
            criteria.Id(_examineId);
            
            var result = searcher.Search(criteria).FirstOrDefault();

            //// Act
            var productVariantDisplay = result.ToProductVariantDisplay();

            //// Assert
            Assert.NotNull(productVariantDisplay);
            Assert.IsTrue(2 == productVariantDisplay.Attributes.Count());
            Assert.IsTrue(productVariantDisplay.CatalogInventories.Any());
        }

        //[Test]
        //public void Can_Build_ProductDisplay_From_Indexed_Product()
        //{
        //    //// Arrange
        //    var merchello = new MerchelloHelper(new Mock<ServiceContext>().Object);
        //    var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];
        //    var criteria = searcher.CreateSearchCriteria("productvariant", BooleanOperation.And);
        //    criteria.Field("productKey", _productKey.ToString()).And().Field("master", "True");

        //    var result = searcher.Search(criteria).FirstOrDefault();

        //    //// Act
        //    var product = result.ToProductDisplay(merchello.Query.Product.);

        //    //// Assert
        //    Assert.NotNull(product);
        //}

        [Test]
        public void Can_Build_ProductDisplay_From_Product()
        {
            //// Arrange
            var productService = PreTestDataWorker.ProductService;
            var product = productService.GetByKey(_productKey);
            var productVariant = product.ProductVariants.First();
            var productOption = product.ProductOptions.First();
            var productChoice = productOption.Choices.First();
            var catalogInventory = productVariant.CatalogInventories.First();

            //// Act
            var productDisplay = product.ToProductDisplay();
            var productVariantDisplay = productDisplay.ProductVariants.First();
            var productOptionDisplay = productDisplay.ProductOptions.First();
            var productChoiceDisplay = productOptionDisplay.Choices.First();
            var catalogInventoryDisplay = productVariantDisplay.CatalogInventories.First();

            //// Assert
            Assert.NotNull(productDisplay);
            Assert.AreEqual(product.Price, productDisplay.Price);
            Assert.AreEqual(product.SalePrice, productDisplay.SalePrice);
            Assert.AreEqual(product.Height, productDisplay.Height);
            Assert.AreEqual(product.OnSale, productDisplay.OnSale);
            Assert.AreEqual(product.Sku, productDisplay.Sku);
            Assert.AreEqual(product.Manufacturer, productDisplay.Manufacturer);
            Assert.AreEqual(product.ManufacturerModelNumber, productDisplay.ManufacturerModelNumber);
            Assert.AreEqual(product.TrackInventory, productDisplay.TrackInventory);
            Assert.AreNotEqual(productDisplay.ManufacturerModelNumber, productDisplay.Manufacturer);
            Assert.AreEqual(product.ProductOptions.Count, productDisplay.ProductOptions.Count());
            Assert.AreEqual(product.ProductVariants.Count, productDisplay.ProductVariants.Count());

            Assert.NotNull(productVariantDisplay);
            Assert.AreEqual(productVariant.Sku, productVariantDisplay.Sku);
            Assert.AreEqual(productVariant.Price, productVariantDisplay.Price);
            Assert.AreEqual(productVariant.ProductKey, productVariantDisplay.ProductKey);
            Assert.AreEqual(productVariant.TrackInventory, productVariantDisplay.TrackInventory);
            Assert.AreEqual(productVariant.Attributes.Count(), productVariantDisplay.Attributes.Count());
            Assert.IsTrue(productVariantDisplay.CatalogInventories.Any());

            Assert.NotNull(catalogInventoryDisplay);
            Assert.AreEqual(catalogInventory.CatalogKey, catalogInventoryDisplay.CatalogKey);
            Assert.AreEqual(catalogInventory.Count, catalogInventoryDisplay.Count);
            Assert.AreEqual(catalogInventory.ProductVariantKey, catalogInventoryDisplay.ProductVariantKey);
            Assert.AreEqual(catalogInventory.LowCount, catalogInventoryDisplay.LowCount);

            Assert.NotNull(productOptionDisplay);
            Assert.AreEqual(productOption.Name, productOptionDisplay.Name);
            Assert.AreEqual(productOption.SortOrder, productOptionDisplay.SortOrder);
            Assert.AreEqual(productOption.Choices.Count(), productOptionDisplay.Choices.Count());

            Assert.NotNull(productChoiceDisplay);
            Assert.AreEqual(productChoice.Name, productChoiceDisplay.Name);
            Assert.AreEqual(productChoice.SortOrder, productChoiceDisplay.SortOrder);
            Assert.AreEqual(productChoice.Sku, productChoiceDisplay.Sku);
        }

        [Test]
        public void Can_Build_Product_From_ProductDisplay()
        {
            //// Arrange
            var productService = PreTestDataWorker.ProductService;
            var product = productService.GetByKey(_productKey);
            var productDisplay = product.ToProductDisplay();

            productDisplay.Barcode = "test-barcode";
            productDisplay.SalePrice = 17M;

            //// Act
            var mappedProduct = productDisplay.ToProduct(product);

            //// Assert
            Assert.NotNull(mappedProduct);
            Assert.IsTrue(mappedProduct.HasIdentity);
            Assert.AreEqual(mappedProduct.Price, productDisplay.Price);
            Assert.AreEqual(mappedProduct.SalePrice, productDisplay.SalePrice);
            Assert.AreEqual(mappedProduct.Height, productDisplay.Height);
            Assert.AreEqual(mappedProduct.OnSale, productDisplay.OnSale);
            Assert.AreEqual(mappedProduct.Sku, productDisplay.Sku);
            Assert.AreEqual(mappedProduct.Manufacturer, productDisplay.Manufacturer);
            Assert.AreEqual(mappedProduct.ManufacturerModelNumber, productDisplay.ManufacturerModelNumber);
            //Assert.AreNotEqual(mappedProduct.ManufacturerModelNumber, mappedProduct.Manufacturer);
            Assert.AreEqual(mappedProduct.ProductOptions.Count, mappedProduct.ProductOptions.Count());
            Assert.AreEqual(mappedProduct.ProductVariants.Count, mappedProduct.ProductVariants.Count());
        }

        //[Test]
        //public void Can_Find_All_Products_From_Index()
        //{
        //    //// Arrange
        //    var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];
        //    var criteria = searcher.CreateSearchCriteria();
        //    criteria.Field("master", "True");

        //    //// Act
        //    var allProducts = searcher.Search(criteria).OrderByDescending(x => x.Score)
        //                              .Select(result => result.ToProductDisplay());

        //    //// Assert
        //    Assert.NotNull(allProducts);
        //    Assert.IsTrue(allProducts.Any());
        //}

    }
}