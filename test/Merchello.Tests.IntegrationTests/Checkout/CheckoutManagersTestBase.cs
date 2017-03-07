namespace Merchello.Tests.IntegrationTests.Checkout
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping.FixedRate;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;

    using NUnit.Framework;

    public abstract class CheckoutManagersTestBase : MerchelloAllInTestBase
    {

        protected IProduct _product1;
        protected IProduct _product2;

        protected IProduct _product3;

        protected IProduct _product4;

        protected IProduct _product5;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            DbPreTestDataWorker.DeleteAllInvoices();
            DbPreTestDataWorker.DeleteAllShipCountries();


            #region Back Office

            #region Product Entry

            _product1 = DbPreTestDataWorker.MakeExistingProduct(true, 1, 1);
            _product2 = DbPreTestDataWorker.MakeExistingProduct(true, 1, 2);
            _product3 = DbPreTestDataWorker.MakeExistingProduct(true, 1, 3);
            _product4 = DbPreTestDataWorker.MakeExistingProduct(true, 1, 4);
            _product5 = DbPreTestDataWorker.MakeExistingProduct(true, 1, 5);


            #endregion

            #region WarehouseCatalog

            var defaultWarehouse = DbPreTestDataWorker.WarehouseService.GetDefaultWarehouse();
            var defaultCatalog = defaultWarehouse.WarehouseCatalogs.FirstOrDefault();
            if (defaultCatalog == null) Assert.Ignore("Default WarehouseCatalog is null");

            #endregion // WarehouseCatalog

            #region Settings -> Shipping

            // Add countries (US & DK) to default catalog
            #region Add Countries

            var us = MerchelloContext.Current.Services.StoreSettingService.GetCountryByCode("US");
            var usCountry = new ShipCountry(defaultCatalog.Key, us);
            ((ServiceContext)MerchelloContext.Current.Services).ShipCountryService.Save(usCountry);

            var dk = MerchelloContext.Current.Services.StoreSettingService.GetCountryByCode("DK");
            var dkCountry = new ShipCountry(defaultCatalog.Key, dk);
            ((ServiceContext)MerchelloContext.Current.Services).ShipCountryService.Save(dkCountry);

            #endregion // ShipCountry

            #region Add a GatewayProvider (RateTableShippingGatewayProvider)

            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.GetProviderByKey(key);

            #region Add and configure 3 rate table shipmethods

            var gwshipMethod1 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, usCountry, "Ground (Vary by Price)");
            gwshipMethod1.RateTable.AddRow(0, 10, 25);
            gwshipMethod1.RateTable.AddRow(10, 15, 30);
            gwshipMethod1.RateTable.AddRow(15, 25, 35);
            gwshipMethod1.RateTable.AddRow(25, 60, 40); // total price should be 50M so we should hit this tier
            gwshipMethod1.RateTable.AddRow(25, 10000, 50);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod1);

            var gwshipMethod2 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, usCountry, "Ground (Vary by Weight)");
            gwshipMethod2.RateTable.AddRow(0, 10, 5);
            gwshipMethod2.RateTable.AddRow(10, 15, 10); // total weight should be 10M so we should hit this tier
            gwshipMethod2.RateTable.AddRow(15, 25, 25);
            gwshipMethod2.RateTable.AddRow(25, 10000, 100);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod2);

            var gwshipMethod3 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, dkCountry, "Ground (Vary by Price)");
            gwshipMethod3.RateTable.AddRow(0, 10, 5);
            gwshipMethod3.RateTable.AddRow(10, 15, 10);
            gwshipMethod3.RateTable.AddRow(15, 25, 25);
            gwshipMethod3.RateTable.AddRow(25, 60, 30); // total price should be 50M so we should hit this tier
            gwshipMethod3.RateTable.AddRow(25, 10000, 50);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod3);

            #endregion // ShipMethods

            #endregion // GatewayProvider

            #endregion  // Shipping

            #region Settings -> Taxation

            var taxProvider = MerchelloContext.Current.Gateways.Taxation.CreateInstance(Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);

            taxProvider.DeleteAllTaxMethods();

            var gwTaxMethod = taxProvider.CreateTaxMethod("US", 0);

            gwTaxMethod.TaxMethod.Provinces["WA"].PercentAdjustment = 8.7M;

            taxProvider.SaveTaxMethod(gwTaxMethod);


            #endregion


            #region Settings
            //// http://issues.merchello.com/youtrack/issue/M-608
            var storeSetting = DbPreTestDataWorker.StoreSettingService.GetByKey(Core.Constants.StoreSetting.GlobalShippingIsTaxableKey);
            storeSetting.Value = true.ToString();
            DbPreTestDataWorker.StoreSettingService.Save(storeSetting);

            #endregion

            #endregion  // Back Office

        }




        protected void WriteBasketInfoToConsole()
        {
            Console.WriteLine("----------- Basket Item Info ---------------------");
            Console.WriteLine("Total quantity count: {0}", CurrentCustomer.Basket().TotalQuantityCount);
            Console.WriteLine("Total basket price: {0}", CurrentCustomer.Basket().TotalBasketPrice);
            Console.WriteLine("Total item count: {0}", CurrentCustomer.Basket().TotalItemCount);
        }
    }
}