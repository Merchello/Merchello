using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.TestHelpers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Gateways
{
    [TestFixture]
    public class ShippingContextTests : MerchelloAllInTestBase
    {
        private IShipCountry _else;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            DbPreTestDataWorker.DeleteAllInvoices();
            DbPreTestDataWorker.DeleteAllShipCountries();

            var defaultCatalog = DbPreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();
            if (defaultCatalog == null) Assert.Ignore("Default WarehouseCatalog is null");

            _else = DbPreTestDataWorker.ShipCountryService.GetShipCountryByCountryCode(defaultCatalog.Key, "ELSE");

            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            var gwshipMethod1 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, _else, "Ground (Vary by Price)");
            gwshipMethod1.RateTable.AddRow(0, 10, 25);
            gwshipMethod1.RateTable.AddRow(10, 15, 30);
            gwshipMethod1.RateTable.AddRow(15, 25, 35);
            gwshipMethod1.RateTable.AddRow(25, 60, 40); // total price should be 50M so we should hit this tier
            gwshipMethod1.RateTable.AddRow(25, 10000, 50);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod1);
        }

        [SetUp]
        public void Init()
        {

        }


        [Test]
        public void Can_Verify_The_GetAllowableShipCountries_Returns_All_Countries_Where_ELSE_Has_ShipMethod()
        {
            //// Arrange
            
            //// Act
            var countries = MerchelloContext.Current.Gateways.Shipping.GetAllowedShipmentDestinationCountries();

            //// Assert
            Assert.Greater(countries.Count(), 1);
        }

    }
}