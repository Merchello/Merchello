using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.Services;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;
using System.Collections.Generic;

namespace Merchello.Tests.IntegrationTests.DisplayClasses
{
    [TestFixture]
    public class WarehouseDisplayTests : ServiceIntegrationTestBase
    {
        private IWarehouse _warehouse;
        private IWarehouseCatalog _warehouseCatalog;

        [SetUp]
        public void Init()
        {

            var warehouseService = PreTestDataWorker.WarehouseService;
            _warehouse = warehouseService.GetDefaultWarehouse();

            _warehouseCatalog = _warehouse.DefaultCatalog();
        }

        [Test]
        public void Can_Build_WarehouseDisplay_From_Warehouse()
        {
            //// Arrange
            var warehouseService = PreTestDataWorker.WarehouseService;
            var warehouse = warehouseService.GetByKey(_warehouse.Key);

            //// Act
            var warehouseDisplay = warehouse.ToWarehouseDisplay();

            //// Assert
            Assert.NotNull(warehouseDisplay);
            Assert.AreEqual(warehouse.Key, warehouseDisplay.Key);
            Assert.AreEqual(warehouse.Name, warehouseDisplay.Name);
            Assert.AreEqual(warehouse.Address1, warehouseDisplay.Address1);
            Assert.AreEqual(warehouse.Address2, warehouseDisplay.Address2);
            Assert.AreEqual(warehouse.Locality, warehouseDisplay.Locality);
            Assert.AreEqual(warehouse.Region, warehouseDisplay.Region);
            Assert.AreEqual(warehouse.PostalCode, warehouseDisplay.PostalCode);
            Assert.AreEqual(warehouse.CountryCode, warehouseDisplay.CountryCode);
            Assert.AreEqual(warehouse.Phone, warehouseDisplay.Phone);
            Assert.AreEqual(warehouse.Email, warehouseDisplay.Email);
            Assert.AreEqual(warehouse.IsDefault, warehouseDisplay.IsDefault);
            Assert.AreEqual(warehouse.WarehouseCatalogs.Count(), warehouseDisplay.WarehouseCatalogs.Count());

        }

        [Test]
        public void Can_Build_Warehouse_From_WarehouseDisplay()
        {
            //// Arrange
            var warehouseService = PreTestDataWorker.WarehouseService;
            var warehouse = warehouseService.GetByKey(_warehouse.Key);
            var warehouseDisplay = warehouse.ToWarehouseDisplay();

            warehouseDisplay.PostalCode = "97333";
            warehouseDisplay.Name = "New Warehouse Name";
            WarehouseCatalogDisplay warehouseCatalogDisplay = warehouseDisplay.WarehouseCatalogs.First();
            
            // Test adding a catalog from the back office.  Not supported in v1

            //List<WarehouseCatalogDisplay> warehouseCatalogs = warehouseDisplay.WarehouseCatalogs as List<WarehouseCatalogDisplay>;

            //WarehouseCatalogDisplay warehouseCatalogDisplay = new WarehouseCatalogDisplay
            //{
            //    Key = Guid.NewGuid(),
            //    WarehouseKey = warehouse.Key,
            //    Name = "New Test Catalog",
            //    Description = "Test Description"
            //};

            //warehouseCatalogs.Add(warehouseCatalogDisplay);

            //// Act
            var mappedWarehouse = warehouseDisplay.ToWarehouse(warehouse);
            var matchingWarehouseCatalogs = mappedWarehouse.WarehouseCatalogs;
            //var matchingWarehouseCatalogs = mappedWarehouse.WarehouseCatalogs.Where(x => x.Key == warehouseCatalogDisplay.Key);
            IWarehouseCatalog mappedWarehouseCatalog = null;
            if (matchingWarehouseCatalogs.Count() > 0)
            {
                mappedWarehouseCatalog = matchingWarehouseCatalogs.First();
            }

            //// Assert
            Assert.NotNull(mappedWarehouse);
            Assert.AreEqual(mappedWarehouse.Key, warehouseDisplay.Key);
            Assert.AreEqual(mappedWarehouse.Name, warehouseDisplay.Name);
            Assert.AreEqual(mappedWarehouse.Address1, warehouseDisplay.Address1);
            Assert.AreEqual(mappedWarehouse.Address2, warehouseDisplay.Address2);
            Assert.AreEqual(mappedWarehouse.Locality, warehouseDisplay.Locality);
            Assert.AreEqual(mappedWarehouse.Region, warehouseDisplay.Region);
            Assert.AreEqual(mappedWarehouse.PostalCode, warehouseDisplay.PostalCode);
            Assert.AreEqual(mappedWarehouse.CountryCode, warehouseDisplay.CountryCode);
            Assert.AreEqual(mappedWarehouse.Phone, warehouseDisplay.Phone);
            Assert.AreEqual(mappedWarehouse.Email, warehouseDisplay.Email);
            Assert.AreEqual(mappedWarehouse.IsDefault, warehouseDisplay.IsDefault);
            Assert.AreEqual(mappedWarehouse.WarehouseCatalogs.Count(), warehouseDisplay.WarehouseCatalogs.Count());

            Assert.NotNull(mappedWarehouseCatalog);
            Assert.AreEqual(mappedWarehouseCatalog.Key, warehouseCatalogDisplay.Key);
            Assert.AreEqual(mappedWarehouseCatalog.WarehouseKey, warehouseCatalogDisplay.WarehouseKey);
            Assert.AreEqual(mappedWarehouseCatalog.Name, warehouseCatalogDisplay.Name);
            Assert.AreEqual(mappedWarehouseCatalog.Description, warehouseCatalogDisplay.Description);
        }

    }
}