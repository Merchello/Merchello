using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.Shipping
{
    public class ShippingProviderTestBase : DatabaseIntegrationTestBase
    {
        protected IGatewayProviderService GatewayProviderService;
        protected IWarehouseCatalog Catalog;
        protected IStoreSettingService StoreSettingService;
        internal IShipCountryService ShipCountryService;
        
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            // assert we have our defaults setup
            var dtos = PreTestDataWorker.Database.Query<WarehouseDto>("SELECT * FROM merchWarehouse");
            var catalogs = PreTestDataWorker.Database.Query<WarehouseCatalogDto>("SELECT * FROM merchWarehouseCatalog");

            if (!dtos.Any() || !catalogs.Any())
            {
                Assert.Ignore("Warehouse defaults are not installed.");
            }

            // TODO : This is only going to be the case for the initial Merchello release
            Catalog = PreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();

            if (Catalog == null)
            {
                Assert.Ignore("Warehouse Catalog is null");
            }

            GatewayProviderService = PreTestDataWorker.GatewayProviderService;
            StoreSettingService = PreTestDataWorker.StoreSettingService;
            ShipCountryService = PreTestDataWorker.ShipCountryService;


            PreTestDataWorker.DeleteAllShipCountries();
            const string countryCode = "US";

            var us = StoreSettingService.GetCountryByCode(countryCode);
            var shipCountry = new ShipCountry(Catalog.Key, us);
            ShipCountryService.Save(shipCountry);

            var dk = StoreSettingService.GetCountryByCode("DK");
            ShipCountryService.Save(new ShipCountry(Catalog.Key, dk));
            
        }

    }
}