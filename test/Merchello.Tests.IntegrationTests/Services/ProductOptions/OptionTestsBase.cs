namespace Merchello.Tests.IntegrationTests.Services.ProductOptions
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;

    using NUnit.Framework;

    public abstract class OptionTestsBase : MerchelloAllInTestBase
    {
        protected IProductOptionService _productOptionService;

        protected IProductService _productService;

        protected Guid _optionKey;

        protected Guid _product1Key;

        protected Guid _product2Key;

        private int _sharedCount = 1;

        protected MerchelloHelper _merchello;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            this._merchello = new MerchelloHelper();

            this._productOptionService = MerchelloContext.Current.Services.ProductOptionService;

            this._productService = MerchelloContext.Current.Services.ProductService;

            this.DbPreTestDataWorker.DeleteAllProducts();
            this.DbPreTestDataWorker.DeleteAllSharedOptions();

            var option = CreateSavedOption();

            _optionKey = option.Key;


            var product1 = CreateSavedProduct();

            this._product1Key = product1.Key;


            var product2 = DbPreTestDataWorker.MakeExistingProduct();
            product2.ProductOptions.Add(new ProductOption("Size"));
            product2.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "sm"));
            product2.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Medium", "md"));
            product2.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Large", "lg"));

            DbPreTestDataWorker.ProductService.Save(product2);

            _product2Key = product2.Key;
            
            MerchelloContext.Current.Cache.RuntimeCache.ClearAllCache();

        }

        protected IProduct CreateSavedProduct()
        {
            var p = DbPreTestDataWorker.MakeExistingProduct();
            p.ProductOptions.Add(new ProductOption("Color"));
            p.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Black", "Black"));
            p.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blue"));
            DbPreTestDataWorker.ProductService.Save(p);

            return p;
        }

        protected IProductOption CreateSavedOption()
        {
            var option = this._productOptionService.CreateProductOption("Shared Option " + _sharedCount, true);
            option.AddChoice("Choice 1", "choice1");
            option.AddChoice("Choice 2", "choice2");
            option.AddChoice("Choice 3", "choice3");
            option.AddChoice("Choice 4", "choice4");
            this._productOptionService.Save(option);

            _sharedCount++;

            return option;
        }

    }
}