namespace Merchello.Tests.IntegrationTests.Services.Warehouse
{
    using System.Linq;

    using ClientDependency.Core;

    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.IntegrationTests.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class WarehhouseServiceTests : DatabaseIntegrationTestBase
    {
        private IWarehouseService _warehouseService;

        [TestFixtureSetUp]
        public void Init()
        {
            
            _warehouseService = PreTestDataWorker.WarehouseService;
        }

        [SetUp]
        public void Setup()
        {
            PreTestDataWorker.DeleteWarehouseCatalogs();
        }

        /// <summary>
        /// Test confirms that the default warehouse can be retrieved
        /// </summary>
        [Test]
        public void Can_Get_The_Default_Warehouse()
        {
            //// Arrange
            var defaultWarehouseKey = Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseKey;

            //// Act
            var warehouse = _warehouseService.GetDefaultWarehouse();

            //// Assert
            Assert.NotNull(warehouse, "Warehouse was null");
            Assert.AreEqual(defaultWarehouseKey, warehouse.Key);
        }

        /// <summary>
        /// Test confirms that the default warehouse contains the default catalog
        /// </summary>
        [Test]
        public void Default_Warehouse_Contains_The_Default_Catalog()
        {
            //// Arrange
            var defaultCatalogKey = Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey;
            
            //// Act
            var warehouse = _warehouseService.GetDefaultWarehouse();
            
            //// Assert
            Assert.NotNull(warehouse);
            Assert.IsTrue(warehouse.WarehouseCatalogs.Any(), "The warehouse's catalog collection was empty");
            Assert.NotNull(warehouse.WarehouseCatalogs.FirstOrDefault(x => x.Key == defaultCatalogKey));
        }

        /// <summary>
        /// Test confirms that a second catalog can be added to a warehouse
        /// </summary>
        [Test]
        public void Can_Add_A_Catalog_To_The_Default_Warehouse()
        {
            //// Arrange
            var warehouse = _warehouseService.GetDefaultWarehouse();

            //// Act
            var catalog = new WarehouseCatalog(warehouse.Key) { Name = "2nd Catalog" };
            _warehouseService.Save(catalog);
            var catalogs = _warehouseService.GetAllWarehouseCatalogs();
            warehouse = _warehouseService.GetDefaultWarehouse();

            //// Assert
            Assert.IsTrue(catalog.HasIdentity, "catalog does not have an identity");
            Assert.NotNull(catalogs, "Catalogs was null");
            Assert.IsTrue(catalogs.Any(), "Catalogs collection was empty");
            Assert.IsTrue(warehouse.WarehouseCatalogs.Count() > 1);
        }
    }
}