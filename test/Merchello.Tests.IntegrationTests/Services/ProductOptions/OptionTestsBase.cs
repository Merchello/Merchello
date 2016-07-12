namespace Merchello.Tests.IntegrationTests.Services.ProductOptions
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    public abstract class OptionTestsBase : MerchelloAllInTestBase
    {
        protected IProductOptionService _productOptionService;

        protected IProductService _productService;

        protected Guid _optionKey;

        protected Guid _productKey;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            this._productOptionService = MerchelloContext.Current.Services.ProductOptionService;

            this._productService = MerchelloContext.Current.Services.ProductService;

            this.DbPreTestDataWorker.DeleteAllProducts();
            this.DbPreTestDataWorker.DeleteAllSharedOptions();

            var option = CreateSavedOption();

            _optionKey = option.Key;


            var product = DbPreTestDataWorker.MakeExistingProduct();
            product.ProductOptions.Add(new ProductOption("Color"));

            //// Act
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Black"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Blue"));

            DbPreTestDataWorker.ProductService.Save(product);

            _productKey = product.Key;

            MerchelloContext.Current.Cache.RuntimeCache.ClearAllCache();

        }

        protected IProductOption CreateSavedOption()
        {
            var option = this._productOptionService.CreateProductOption("Retrieve", true);
            option.AddChoice("Choice 1", "choice1");
            option.AddChoice("Choice 2", "choice2");
            option.AddChoice("Choice 3", "choice3");
            option.AddChoice("Choice 4", "choice4");
            this._productOptionService.Save(option);

            return option;
        }

    }
}