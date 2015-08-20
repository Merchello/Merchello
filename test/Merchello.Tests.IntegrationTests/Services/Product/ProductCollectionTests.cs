namespace Merchello.Tests.IntegrationTests.Services.Product
{
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class ProductCollectionTests : MerchelloAllInTestBase
    {
        private IProductService _productService;

        private IEntityCollectionService _entityCollectionService;

        private IProduct _product;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            DbPreTestDataWorker.DeleteAllProducts();
            DbPreTestDataWorker.DeleteAllEntityCollections();

            _productService = DbPreTestDataWorker.ProductService;
            _entityCollectionService = DbPreTestDataWorker.EntityCollectionService;

            _product = DbPreTestDataWorker.MakeExistingProduct();
            _product.ProductOptions.Add(new ProductOption("Color"));
            _product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Blk"));
            _product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blu"));
            _product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Red", "Red"));
            _product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Green", "Gre"));
            _product.ProductOptions.Add(new ProductOption("Size"));
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Sm"));
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Medium", "M"));
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Large", "Lg"));
            _product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("X-Large", "XL"));
            _product.Height = 20;
            _product.Weight = 20;
            _product.Length = 20;
            _product.Width = 20;
            _product.Shippable = true;
            _productService.Save(_product);
        }

    }
}