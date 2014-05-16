using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.Services.GatewayProvider
{
    [TestFixture]
    [Category("Service Integration")]
    public class GatewayProviderServiceTests : DatabaseIntegrationTestBase
    {
        private IGatewayProviderService _gatewayProviderService;
        private IWarehouseCatalog _catalog;
        private IStoreSettingService _storeSettingService;
        private IShipCountryService _shipCountryService;
        
        [SetUp]
        public void Init()
        {
            _gatewayProviderService = PreTestDataWorker.GatewayProviderService;
            _storeSettingService = PreTestDataWorker.StoreSettingService;
            _shipCountryService = PreTestDataWorker.ShipCountryService;

            
            _catalog = PreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();

            PreTestDataWorker.DeleteAllShipCountries();
            var country = _storeSettingService.GetCountryByCode("US");
            var shipCountry = new ShipCountry(_catalog.Key, country);
            _shipCountryService.Save(shipCountry);
           
       
            var shippingProvider =
               (FixedRateShippingGatewayProvider) MerchelloContext.Gateways.Shipping.CreateInstance(Core.Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey);
            Assert.NotNull(shippingProvider);

            var resource = shippingProvider.ListResourcesOffered().FirstOrDefault();
            var gatewayShipMethod = shippingProvider.CreateShippingGatewayMethod(resource, shipCountry, "Ground");
            shippingProvider.SaveShippingGatewayMethod(gatewayShipMethod);
        }

        /// <summary>
        /// Test confirms that a collection of GatewayProviders can be returned by ShipCountry
        /// </summary>
        [Test]
        public void Can_Query_A_List_Of_GatewayProviders_By_Country()
        {
            //// Arrange
            var shipCountry = _shipCountryService.GetShipCountriesByCatalogKey(_catalog.Key).First(x => x.CountryCode == "US");

            
            //// Act
            var gateways = _gatewayProviderService.GetGatewayProvidersByShipCountry(shipCountry);

            //// Assert
            Assert.NotNull(gateways);
            Assert.IsTrue(gateways.Any());
        }
    }
}