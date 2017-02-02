namespace Merchello.Tests.IntegrationTests.MerchelloHelperTests
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;
    using Merchello.Web.DataModifiers;
    using Merchello.Web.DataModifiers.Product;

    using NUnit.Framework;

    using Umbraco.Web;

    [TestFixture]
    public class ProductDataModiferTests : MerchelloAllInTestBase
    {
        private IStoreSettingService _settingService;

        private Guid _productKey;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();


            var defaultWarehouse = DbPreTestDataWorker.WarehouseService.GetDefaultWarehouse();
            var defaultCatalog = defaultWarehouse.WarehouseCatalogs.FirstOrDefault();
            if (defaultCatalog == null) Assert.Ignore("Default WarehouseCatalog is null");

            DbPreTestDataWorker.DeleteAllShipCountries();
            var uk = MerchelloContext.Current.Services.StoreSettingService.GetCountryByCode("GB");
            var ukCountry = new ShipCountry(defaultCatalog.Key, uk);
            ((ServiceContext)MerchelloContext.Current.Services).ShipCountryService.Save(ukCountry);

            this._settingService = MerchelloContext.Current.Services.StoreSettingService;
            var setting = this._settingService.GetByKey(Core.Constants.StoreSetting.GlobalTaxationApplicationKey);
            setting.Value = "Product";
            this._settingService.Save(setting);

            var taxProvider = MerchelloContext.Current.Gateways.Taxation.GetProviderByKey(Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);
            taxProvider.DeleteAllTaxMethods();
            var gwTaxMethod = taxProvider.CreateTaxMethod("GB", 25);
            gwTaxMethod.TaxMethod.ProductTaxMethod = true;
            taxProvider.SaveTaxMethod(gwTaxMethod);
            ((TaxationContext)MerchelloContext.Current.Gateways.Taxation).ClearProductPricingMethod();

            DbPreTestDataWorker.DeleteAllProducts();
            var product = MockProductDataMaker.MockProductForInserting("Product", "PR", 16M);
            product.SalePrice = 12M;
            MerchelloContext.Current.Services.ProductService.Save(product);
            _productKey = product.Key;
        }

        [TestFixtureTearDown]
        public override void FixtureTearDown()
        {
            base.FixtureTearDown();
            var setting = this._settingService.GetByKey(Core.Constants.StoreSetting.GlobalTaxationApplicationKey);
            setting.Value = "Invoice";
            this._settingService.Save(setting);
            ((TaxationContext)MerchelloContext.Current.Gateways.Taxation).ClearProductPricingMethod();
        }

        [Test]
        public void Can_Instantiate_The_ModifiableProductVaraintDataModfierChain()
        {
            const int TaskCount = 1;
            var chain = new ProductVariantDataModifierChain(MerchelloContext.Current);

            Assert.NotNull(chain);
            Assert.AreEqual(TaskCount, chain.TaskCount);

        }

        [Test]
        public void Can_Get_Product_From_Helper_With_Tax_Included()
        {
            //// Arrange
            var merchello = new MerchelloHelper();
            
            //// Act
            var product = merchello.Query.Product.GetByKey(_productKey);

            //// Assert
            Assert.NotNull(product);
            Assert.AreEqual(20, product.Price);
            Assert.AreEqual(15, product.SalePrice);
           
        }
    }
}