using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Sales;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web.Workflow;
using NUnit.Framework;
using Umbraco.Core;
using System.Linq;
using Merchello.Web;

namespace Merchello.Tests.IntegrationTests.Builders
{

    public class BuilderTestBase : DatabaseIntegrationTestBase
    {
        protected IItemCache ItemCache;
        protected ICustomerBase Customer;
        protected SalePreparationBase SalePreparationMock;
        protected IAddress BillingAddress;
        protected IBasket Basket;
        protected const int ProductCount = 5;
        protected const decimal WeightPerProduct = 3;
        protected const decimal PricePerProduct = 5;
        
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

       
            PreTestDataWorker.DeleteAllShipCountries();

            var defaultCatalog = PreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();
            if (defaultCatalog == null) Assert.Ignore("Default WarehouseCatalog is null");

            var us = MerchelloContext.Services.StoreSettingService.GetCountryByCode("US");
            var usCountry = new ShipCountry(defaultCatalog.Key, us);
            ((ServiceContext)MerchelloContext.Services).ShipCountryService.Save(usCountry);

            var key = Core.Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Gateways.Shipping.GetProviderByKey(key);
            rateTableProvider.DeleteAllActiveShipMethods(usCountry);

            #region Add and configure 3 rate table shipmethods

            var gwshipMethod1 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, usCountry, "Ground (Vary by Price)");
            gwshipMethod1.RateTable.AddRow(0, 10, 25);
            gwshipMethod1.RateTable.AddRow(10, 15, 30);
            gwshipMethod1.RateTable.AddRow(15, 25, 35);
            gwshipMethod1.RateTable.AddRow(25, 60, 40); // total price should be 50M so we should hit this tier
            gwshipMethod1.RateTable.AddRow(25, 10000, 50);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod1);

            #endregion
        }

        [SetUp]
        public virtual void Init()
        {
            Customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            Basket = Web.Workflow.Basket.GetBasket(MerchelloContext, Customer);

            var odd = true;
            for (var i = 0; i < ProductCount; i++)
            {
                
                var product = PreTestDataWorker.MakeExistingProduct(true, WeightPerProduct, PricePerProduct);
                product.AddToCatalogInventory(PreTestDataWorker.WarehouseCatalog);
                product.CatalogInventories.First().Count = 10;
                product.TrackInventory = true;
                PreTestDataWorker.ProductService.Save(product);
                Basket.AddItem(product, 2);

                odd = !odd;
            }

            BillingAddress = new Address()
            {
                Name = "Out there",
                Address1 = "some street",
                Locality = "some city",
                Region = "ST",
                PostalCode = "98225",
                CountryCode = "US"
            };

            var origin = new Address()
            {
                Name = "Somewhere",
                Address1 = "origin street",
                Locality = "origin city",
                Region = "ST",
                PostalCode = "98225",
                CountryCode = "US"
            };



            PreTestDataWorker.DeleteAllItemCaches();
            PreTestDataWorker.DeleteAllInvoices();


            Customer.ExtendedData.AddAddress(BillingAddress, AddressType.Billing);
            ItemCache = new Core.Models.ItemCache(Customer.Key, ItemCacheType.Checkout);

            PreTestDataWorker.ItemCacheService.Save(ItemCache);

            foreach (var item in Basket.Items)
            {
                ItemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
            }


            // setup the checkout
            SalePreparationMock = new SalePreparationMock(MerchelloContext, ItemCache, Customer);

            // add the shipment rate quote
            var shipment = Basket.PackageBasket(MerchelloContext, BillingAddress).First();
            var shipRateQuote = shipment.ShipmentRateQuotes(MerchelloContext).FirstOrDefault();
            
            //_checkoutMock.ItemCache.Items.Add(shipRateQuote.AsLineItemOf<InvoiceLineItem>());
            SalePreparationMock.SaveShipmentRateQuote(shipRateQuote);
        }

    }
}