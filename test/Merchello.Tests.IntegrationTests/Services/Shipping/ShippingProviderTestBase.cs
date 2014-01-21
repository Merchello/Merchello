using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.Services.Shipping
{
    public class ShippingProviderTestBase : ServiceIntegrationTestBase
    {
        protected IGatewayProviderService GatewayProviderService;
        protected IWarehouseCatalog Catalog;
        protected ISettingsService SettingsService;
        protected IShippingService ShippingService;
        protected IMerchelloContext MerchelloContext;

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
            Catalog = PreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();

            if (Catalog == null)
            {
                Assert.Ignore("Warehouse Catalog is null");
            }

            GatewayProviderService = PreTestDataWorker.GatewayProviderService;
            SettingsService = PreTestDataWorker.SettingsService;
            ShippingService = PreTestDataWorker.ShippingService;

            MerchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));
        }

    }
}