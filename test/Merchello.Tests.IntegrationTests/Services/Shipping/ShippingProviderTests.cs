using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using NUnit.Framework;
using Rhino.Mocks.Constraints;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.Services.Shipping
{

    [TestFixture]
    [Category("Service Integration")]
    public class ShippingProviderTests : ServiceIntegrationTestBase
    {

        private IGatewayProviderService _gatewayProviderService;
        private IWarehouseCatalog _catalog;
        private ISettingsService _settingsService;
        private IShippingService _shippingService;
        private IMerchelloContext _merchelloContext;

        [TestFixtureSetUp]
        public void FixtureInit()
        {
            // assert we have our defaults setup
            var dtos = PreTestDataWorker.Database.Query<WarehouseDto>("SELECT * FROM merchWarehouse");
            var catalogs = PreTestDataWorker.Database.Query<WarehouseCatalogDto>("SELECT * FROM merchWarehouseCatalog");

            if (!dtos.Any() || !catalogs.Any())
            {
                Assert.Ignore("Warehouse defaults are not installed.");
            }

            // TODO : This is only going to be the case for the initial Merchello release
            _catalog = PreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();

            if (_catalog == null)
            {
                Assert.Ignore("Warehouse Catalog is null");
            }

            _settingsService = PreTestDataWorker.SettingsService;
            _shippingService = PreTestDataWorker.ShippingService;

            _merchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));
        }

        [SetUp]
        public void Init()
        {
            _gatewayProviderService = PreTestDataWorker.GatewayProviderService;
            PreTestDataWorker.DeleteAllShipCountries();
            const string countryCode = "US";

            var country = _settingsService.GetCountryByCode(countryCode);
            
            var shipCountry = new ShipCountry(_catalog.Key, country);
            _shippingService.Save(shipCountry);
        }

        /// <summary>
        /// Test verifies that a list of all shipping providers can be retrieved.
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_All_Shipping_Providers()
        {
            //// Arrange
            var expected = 1;

            //// Act
            var providers = _gatewayProviderService.GetGatewayProvidersByType(GatewayProviderType.Shipping);

            //// Assert
            Assert.IsTrue(providers.Any());
            Console.WriteLine("Provider name: {0}", providers.First().Name);
        }


        /// <summary>
        /// Test verifies that a ShippingGateway class can be instantiated from a IGatewayProvider reference
        /// </summary>
        [Test]
        public void Can_Instantiate_A_ShippingProvider_From_A_GatewayProvider()
        {
            //// Arrange
            var provider = _gatewayProviderService.GetGatewayProvidersByType(GatewayProviderType.Shipping).FirstOrDefault();
            Assert.NotNull(provider);

            //// Act

            var ctorArgs = new[] { typeof(MerchelloContext), typeof(ShippingGatewayBase<>)};
            var ctoArgValues = new object[] { merchelloContext, basket, destination };
            var strategy = ActivatorHelper.CreateInstance<BasketPackagingStrategyBase>(Type.GetType(defaultStrategy), ctorArgs, ctoArgValues);
        }


        /// <summary>
        /// Test verifies that a provider can be associated with a ShipCountry
        /// </summary>
        [Test]
        public void Can_Associate_A_Provider_With_A_ShipCountry()
        {
            //// Arrange
            var country = _shippingService.GetShipCountryByCountryCode(_catalog.Key, "US");
            var provider = _gatewayProviderService.GetGatewayProvidersByType(GatewayProviderType.Shipping).FirstOrDefault();
            Assert.NotNull(provider);
            
            //// Act
            

        }
    }
}