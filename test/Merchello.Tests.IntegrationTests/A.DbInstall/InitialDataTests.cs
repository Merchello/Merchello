﻿using System.Linq;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Tests.Base.TestHelpers;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.IntegrationTests.A.DbInstall
{
    using Merchello.Core.Persistence.Migrations;

    [TestFixture]
    public class InitialDataTests
    {
        private UmbracoDatabase _database;

        private BaseDataCreation _creation;
        private MerchelloDatabaseSchemaHelper _schemaHelper;

        [TestFixtureSetUp]
        public void Init()
        {
            var worker = new DbPreTestDataWorker();
            _database = worker.Database;
            _creation = new BaseDataCreation(worker.Database, worker.TestLogger);
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            _database.Dispose();
        }

                /// <summary>
        /// Test to verify Merchello 
        /// </summary>
        [Test]
        public void Can_Populate_typeFieldData_Into_merchTypeField()
        {
            //// Arrange
            const int expected = 34;

            //// Act
            _creation.InitializeBaseData("merchTypeField");

            //// Assert
            var dtos = _database.Query<TypeFieldDto>("SELECT * FROM merchTypeField");
            var count = dtos.Count();
            Assert.AreEqual(expected, count);
        }

        [Test]
        public void Can_Populate_InvoiceStatusData_Into_merchInvoiceStatus()
        {
            //// Arrange


            //// Act
            _creation.InitializeBaseData("merchInvoiceStatus");
            var dtos = _database.Query<InvoiceStatusDto>("SELECT * FROM merchInvoiceStatus").OrderBy(x => x.SortOrder);

            //// Assert
            Assert.IsTrue(dtos.Any());
            Assert.IsTrue(dtos.First().Name == "Unpaid");
            Assert.IsTrue(dtos.First().SortOrder == 1);
            Assert.IsTrue(dtos.Last().Name == "Fraud");
            Assert.IsTrue(dtos.Last().SortOrder == 5);
        }

        [Test]
        public void Can_Populate_OrderStatusData_Into_merchOrderStatus()
        {
            //// Act
            _creation.InitializeBaseData("merchOrderStatus");
            var dtos = _database.Query<OrderStatusDto>("SELECT * FROM merchOrderStatus").OrderBy(x => x.SortOrder);

            //// Assert
            Assert.IsTrue(dtos.Any());
            Assert.IsTrue(dtos.First().Name == "Not Fulfilled");
            Assert.IsTrue(dtos.First().SortOrder == 1);
            Assert.IsTrue(dtos.Last().Name == "Cancelled");
            Assert.IsTrue(dtos.Last().SortOrder == 5);
        }

        [Test]
        public void Can_Populate_DefaultWarehouse_Into_merchWarehouse()
        {
            //// Arrange
            var expected = 1;

            //// Act
            _creation.InitializeBaseData("merchWarehouse");
            var dtos = _database.Query<WarehouseDto>("SELECT * FROM merchWarehouse");
            var catalogs = _database.Query<WarehouseCatalogDto>("SELECT * FROM merchWarehouseCatalog");

            //// Assert
            Assert.IsTrue(dtos.Any());
            Assert.IsTrue(catalogs.Any());
        }


        [Test]
        public void Can_Populate_GatewayProviders()
        {
            //// Arrange
            var expected = 3;

            //// Act
            _creation.InitializeBaseData("merchGatewayProviderSettings");
            var dtos = _database.Query<WarehouseDto>("SELECT * FROM merchGatewayProviderSettings");

            //// Assert
            Assert.IsTrue(dtos.Any());
            Assert.AreEqual(expected, dtos.Count());
        }

        [Test]
        public void Can_Populate_StoreSettings()
        {
            //// Arrange
            const int expected = 15;

            //// Act
            _creation.InitializeBaseData("merchStoreSetting");
            var dtos = _database.Query<StoreSettingDto>("SELECT * FROM merchStoreSetting");

            //// Assert
            Assert.IsTrue(dtos.Any());
            Assert.AreEqual(expected, dtos.Count());

        }

        [Test]
        public void Can_Populate_ShipmentStatuses()
        {
            //// Arrange
            var expected = 5;

            //// Act
            _creation.InitializeBaseData("merchShipmentStatus");
            var dtos = _database.Query<ShipmentStatusDto>("SELECT * FROM merchShipmentStatus");

            //// Assert
            Assert.IsTrue(dtos.Any());
            Assert.AreEqual(expected, dtos.Count());
        }

    }
}
