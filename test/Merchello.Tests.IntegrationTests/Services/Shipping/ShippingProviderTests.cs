using System;
using System.Data;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Cache;

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
            var ctorArgs = new[] { typeof(IGatewayProviderService), typeof(IGatewayProvider), typeof(IRuntimeCacheProvider) };
            var ctoArgValues = new object[] { _gatewayProviderService, provider, _merchelloContext.Cache.RuntimeCache };
            var gateway = ActivatorHelper.CreateInstance<GatewayProviderBase>(Type.GetType(provider.TypeFullName), ctorArgs, ctoArgValues);

            //// Assert
            Assert.NotNull(gateway);
            Assert.IsTrue(GatewayProviderType.Shipping == gateway.GatewayProvider.GatewayProviderType);
        }

        /// <summary>
        /// Test verifies that a list of all shipping providers can be retrieved from the GatewayContext
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_All_ShippingProviders_From_The_GatewayContext()
        {
            //// Arrange
            const GatewayProviderType gatewayProviderType = GatewayProviderType.Shipping;

            //// Act
            var providers = _merchelloContext.Gateways.GetGatewayProviders(gatewayProviderType);

            //// Assert
            Assert.NotNull(providers);
            Assert.IsTrue(providers.Any());
        }


        /// <summary>
        /// Test verifies that a ShippingGateway class can be instantiated from a IGatewayProvider reference from the GatewayContext
        /// </summary>
        [Test]
        public void Can_Instantiate_A_ShippingProvider_From_The_GatewayContext()
        {
            //// Arrange
            const GatewayProviderType gatewayProviderType = GatewayProviderType.Shipping;
            var provider = _merchelloContext.Gateways.GetGatewayProviders(gatewayProviderType).FirstOrDefault();
            Assert.NotNull(provider);

            //// Act
            var shippingProvider = _merchelloContext.Gateways.GetShippingGatewayProvider(provider);

            //// Assert
            Assert.NotNull(shippingProvider);
        }

        /// <summary>
        /// Test verifies that a provider can be associated with a ShipCountry
        /// </summary>
        [Test]
        public void Can_Add_A_Shipmethod_To_A_Provider_With_A_ShipCountry()
        {
            //// Arrange
            var country = _shippingService.GetShipCountryByCountryCode(_catalog.Key, "US");
            var provider = _merchelloContext.Gateways.GetGatewayProviders(GatewayProviderType.Shipping).FirstOrDefault();
            var shippingProvider = _merchelloContext.Gateways.GetShippingGatewayProvider(provider);
            Assert.NotNull(shippingProvider);
            
            //// Act
            var resource = shippingProvider.ListAvailableMethods().FirstOrDefault();
            var gatewayShipMethod = shippingProvider.CreateShipMethod(resource, country, "Ground");
            shippingProvider.SaveShipMethod(gatewayShipMethod);

            //// Assert
            Assert.IsTrue(gatewayShipMethod.ShipMethod.HasIdentity);

        }

        /// <summary>
        /// Test verifies that a constraint violation is thrown if a duplicate shipmethod is added per for a country
        /// </summary>
        [Test]
        public void Attempting_To_Add_A_Duplicate_Provider_ShipMethod_For_ShipCountry_Results_In_A_Error()
        {
            var country = _shippingService.GetShipCountryByCountryCode(_catalog.Key, "US");
            var provider = _merchelloContext.Gateways.GetGatewayProviders(GatewayProviderType.Shipping).FirstOrDefault();
            var shippingProvider = _merchelloContext.Gateways.GetShippingGatewayProvider(provider);
            Assert.NotNull(shippingProvider);
            var resource = shippingProvider.ListAvailableMethods().FirstOrDefault();
            var gatewayShipMethod = shippingProvider.CreateShipMethod(resource, country, "Ground");
            shippingProvider.SaveShipMethod(gatewayShipMethod);

            //// Act

            //// Assert
            Assert.Throws<ConstraintException>(() => shippingProvider.CreateShipMethod(resource, country, "Ground"));

        }
    }
}