using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence.Migrations;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneZero;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    /// <summary>
    /// Exposes access to internal Migration (Database installation and Upgrade) classes
    /// 
    /// This is an iterum approach to Upgrading your Merchello Database to the current version
    /// </summary>
    public class MigrationHelper
    {
        private readonly Database _database;

        public MigrationHelper(Database database)
        {
            Mandate.ParameterNotNull(database, "database is null");

            _database = database;
        }

        /// <summary>
        /// Creates a new version of Merchello's Database Schema
        /// </summary>
        public void InitializeDatabaseSchema()
        {
            var creation = new DatabaseSchemaCreation(_database);
            creation.InitializeDatabaseSchema();
        }

        /// <summary>
        /// Completely uninstalls Merchello's database schema
        /// </summary>
        public void UninstallDatabaseSchema()
        {
            var uninstaller = new DatabaseSchemaCreation(_database);
            uninstaller.UninstallDatabaseSchema();
        }

        /// <summary>
        /// Upgrades a database schema from Merchello version 1.0.1.4 (second relase) to Merchello version 1.1.0
        /// </summary>
        public void UpgradeTargetVersionOneOneZero()
        {
            // add the notification tables
            DatabaseSchemaHelper.InitializeDatabaseSchema(_database, CreateOneOneZeroTables.OrderedTables, "1.1.0 upgrade");


            // Add the ShipDateColumn to the merchShipment table
            _database.Execute("ALTER TABLE merchShipment ADD shippedDate datetime NOT NULL DEFAULT GETDATE()");

            // Insert new TypeField Data
            var gwp = new GatewayProviderTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = gwp.Notification.TypeKey, Alias = gwp.Notification.Alias, Name = gwp.Notification.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            var entity = new EntityTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Customer.TypeKey, Alias = entity.Customer.Alias, Name = entity.Customer.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.GatewayProvider.TypeKey, Alias = entity.GatewayProvider.Alias, Name = entity.GatewayProvider.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Invoice.TypeKey, Alias = entity.Invoice.Alias, Name = entity.Invoice.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.ItemCache.TypeKey, Alias = entity.ItemCache.Alias, Name = entity.ItemCache.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Order.TypeKey, Alias = entity.Order.Alias, Name = entity.Order.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Payment.TypeKey, Alias = entity.Payment.Alias, Name = entity.Payment.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Product.TypeKey, Alias = entity.Product.Alias, Name = entity.Product.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Shipment.TypeKey, Alias = entity.Shipment.Alias, Name = entity.Shipment.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Warehouse.TypeKey, Alias = entity.Warehouse.Alias, Name = entity.Warehouse.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.WarehouseCatalog.TypeKey, Alias = entity.WarehouseCatalog.Alias, Name = entity.WarehouseCatalog.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

        }
    }
}
