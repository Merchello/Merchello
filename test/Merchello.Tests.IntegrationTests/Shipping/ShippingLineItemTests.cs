namespace Merchello.Tests.IntegrationTests.Shipping
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Gateways.Shipping.FixedRate;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Workflow;

    using NUnit.Framework;

    public class ShippingLineItemTests: ShippingProviderTestBase
    {
        private IShipCountry _shipCountry;

        private IShippingGatewayProvider _shippingProvider;
        private ICustomerBase _customer;
        private IBasket _basket;
        private const int ProductCount = 3;
        private IAddress _destination;
        private const decimal WeightPerProduct = 2M;
        private const decimal PricePerProduct = 10M;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            #region BackOffice
            

            //// Arrange
            var usCountry = ShipCountryService.GetShipCountryByCountryCode(Catalog.Key, "US");
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;

            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Gateways.Shipping.CreateInstance(key);

            rateTableProvider.DeleteAllActiveShipMethods(usCountry);
            var gwshipMethod1 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, usCountry, "Ground (Vary By Pricc) 1");
            gwshipMethod1.RateTable.AddRow(0, 10, 5);
            gwshipMethod1.RateTable.AddRow(10, 15, 10);
            gwshipMethod1.RateTable.AddRow(15, 25, 25);
            gwshipMethod1.RateTable.AddRow(25, 60, 30); // total price should be 50M so we should hit this tier
            gwshipMethod1.RateTable.AddRow(25, 10000, 50);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod1);    
            
            var gwshipMethod2 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, usCountry, "Ground (VBW)");
            gwshipMethod2.RateTable.AddRow(0, 10, 5);
            gwshipMethod2.RateTable.AddRow(10, 15, 10); // total weight should be 10M so we should hit this tier
            gwshipMethod2.RateTable.AddRow(15, 25, 25);
            gwshipMethod2.RateTable.AddRow(25, 10000, 100);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod2);

            var gwshipMethod3 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, usCountry, "Ground (Vary By Price) 3");
            gwshipMethod3.RateTable.AddRow(0, 10, 5);
            gwshipMethod3.RateTable.AddRow(10, 15, 10);
            gwshipMethod3.RateTable.AddRow(15, 25, 25);
            gwshipMethod3.RateTable.AddRow(25, 60, 30); // total price should be 50M so we should hit this tier
            gwshipMethod3.RateTable.AddRow(25, 10000, 50);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod3);

            #endregion

            _shippingProvider = rateTableProvider;
        }

        [SetUp]
        public void Init()
        {
            _destination = new Address()
            {
                Name = "Mindfly Web Design Studio",
                Address1 = "114 W. Magnolia St.  Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US"
            };

            PreTestDataWorker.DeleteAllItemCaches();   
            PreTestDataWorker.DeleteAllInvoices();
            _customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            _basket = Basket.GetBasket(MerchelloContext, _customer);

            for (var i = 0; i < ProductCount; i++) _basket.AddItem(PreTestDataWorker.MakeExistingProduct(true, WeightPerProduct, PricePerProduct));

            Basket.Save(MerchelloContext, _basket);

            _shipCountry = ShipCountryService.GetShipCountryByCountryCode(Catalog.Key, "US");

        }

        [Test]
        public void Can_Quote_A_Basket_Shipment_And_Assert_That_ShipMethods_Are_Set()
        {
            //// Arrange
            var shipment = _basket.PackageBasket(MerchelloContext, _destination).FirstOrDefault();
            Assert.NotNull(shipment);

            //// Act
            var quotes = _shippingProvider.QuoteShippingGatewayMethodsForShipment(shipment).ToArray();
            Assert.NotNull(quotes);
            Assert.IsTrue(quotes.Any());

            //// Assert
            foreach (var quote in quotes)
            {
                Assert.AreEqual(quote.ShipMethod.Key, quote.Shipment.ShipMethodKey);
            }

        }
    }
}