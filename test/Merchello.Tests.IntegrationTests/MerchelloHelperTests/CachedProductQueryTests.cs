namespace Merchello.Tests.IntegrationTests.MerchelloHelperTests
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;

    using NUnit.Framework;

    [TestFixture]
    public class CachedProductQueryTests : MerchelloAllInTestBase
    {
        private MerchelloHelper _merchello;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();
            DbPreTestDataWorker.DeleteAllProducts();
            var defaultWarehouse = DbPreTestDataWorker.WarehouseService.GetDefaultWarehouse();

            _merchello = new MerchelloHelper(MerchelloContext.Current.Services, false);

            var productService = MerchelloContext.Current.Services.ProductService;

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
            product.Price = 30M;
            product.Width = 11M;
            product.Length = 11M;
            product.Manufacturer = "Manufacturer1";
            product.CostOfGoods = 15M;
            product.OnSale = true;
            product.SalePrice = 25M;
            productService.Save(product);

            var product2 = MockProductDataMaker.MockProductCollectionForInserting(1).First();
            product2.ProductOptions.Add(new ProductOption("Color"));
            product2.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Purple", "purple"));
            product2.ProductOptions.Add(new ProductOption("Size"));
            product2.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Sm"));
            product2.Price = 40M;
            product2.Height = 11M;
            product2.Width = 11M;
            product2.Length = 11M;
            product2.CostOfGoods = 15M;
            product2.Manufacturer = "Manufacturer2";
            product2.OnSale = false;
            product2.SalePrice = 35M;
            product2.AddToCatalogInventory(defaultWarehouse.DefaultCatalog());           
            productService.Save(product2);
            product2.CatalogInventories.First().Count = 10;
            productService.Save(product2);

            var product3 = MockProductDataMaker.MockProductCollectionForInserting(1).First();
            product3.ProductOptions.Add(new ProductOption("Color"));
            product3.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blue"));
            product3.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Red", "Red"));
            product3.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Green", "Green"));
            product3.Price = 20M;
            product3.Height = 11M;
            product3.Width = 11M;
            product3.Length = 11M;
            product3.CostOfGoods = 15M;
            product3.Manufacturer = "Manufacturer2";
            product3.OnSale = false;
            product3.SalePrice = 20M;
            product3.AddToCatalogInventory(defaultWarehouse.DefaultCatalog());
            productService.Save(product3);
            product3.CatalogInventories.First().Count = 10;
            productService.Save(product3);


            var product4 = MockProductDataMaker.MockProductCollectionForInserting(1).First();
            product4.ProductOptions.Add(new ProductOption("Size"));
            product4.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Sm"));
            product4.ProductOptions.Add(new ProductOption("Material"));
            product4.ProductOptions.First(x => x.Name == "Material").Choices.Add(new ProductAttribute("Wood", "wood"));
            product4.ProductOptions.First(x => x.Name == "Material").Choices.Add(new ProductAttribute("Plastic", "plastic"));
            product4.Price = 21M;
            product4.Height = 11M;
            product4.Width = 11M;
            product4.Length = 11M;
            product4.CostOfGoods = 15M;
            product4.Manufacturer = "Manufacturer3";
            product4.OnSale = true;
            product4.SalePrice = 18M;
            product4.AddToCatalogInventory(defaultWarehouse.DefaultCatalog());
            productService.Save(product4);
            product4.CatalogInventories.First().Count = 10;
            productService.Save(product4);
        }

        [Test]
        public void Query_Returns_An_Empty_Result_For_A_Product_Without_Matching_Option_And_Value()
        {
            //// Arrange
            // handled in setup

            //// Act
            var products = _merchello.Query.Product.GetProductsWithOption("Color", "Black", 1, 10);

            //// Assert
            Assert.IsFalse(products.Items.Any(), "Search returned items");

        }

        [Test]
        public void Query_Returns_A_Product_With_Matching_Option()
        {
            //// Arrange
            // handled in setup

            //// Act
            var products = _merchello.Query.Product.GetProductsWithOption("Color", 1, 10);

            //// Assert
            Assert.IsTrue(products.Items.Any(), "Search did not return any items");
            Assert.AreEqual(3, products.Items.Count(), "Count did not return as expected");
        }

        [Test]
        public void Query_Returns_A_Product_With_AnyOptions_Matching_Options()
        {
            //// Arrange
            // handled in setup

            //// Act
            var products = _merchello.Query.Product.GetProductsWithOption(new[] {"Material", "Size" }, 1, 10);

            //// Assert
            Assert.IsTrue(products.Items.Any(), "Search did not return any items");
            Assert.AreEqual(3, products.Items.Count(), "Count did not return as expected");
        }

        [Test]
        public void Query_Returns_Product_With_Color_Purple()
        {
            //// Arrange
            // handled in setup

            //// Act
            var products = _merchello.Query.Product.GetProductsWithOption("Color", new[] { "Purple" }, 1, 10);

            //// Assert
            Assert.IsTrue(products.Items.Any(), "Search did not return any items");
            Assert.AreEqual(1, products.Items.Count(), "Count did not return as expected");
        }

        [Test]
        public void Can_Return_A_List_Of_Products_By_Price_Range()
        {
            //// Arrange
            var min = 15M;
            var max = 20M;

            //// Act
            var results1 = _merchello.Query.Product.GetProductsInPriceRange(min, max, 1, 10);
            var results2 = _merchello.Query.Product.GetProductsInPriceRange(38M, 41M, 1, 10);

            //// Assert
            Assert.IsTrue(results1.Items.Any(), "Search 1 did not return any items");
            Assert.AreEqual(2, results1.Items.Count(), "Count 1 did not return as expected");

            Assert.IsTrue(results2.Items.Any(), "Search 2 did not return any items");
            Assert.AreEqual(1, results2.Items.Count(), "Count 2 did not return as expected");
        }

    }
}