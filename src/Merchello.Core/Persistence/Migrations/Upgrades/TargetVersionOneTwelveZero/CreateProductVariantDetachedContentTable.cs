namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneTwelveZero
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// The create product variant 2 detached content type table.
    /// </summary>
    [Migration("1.11.0", "1.11.0.1", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateProductVariantDetachedContentTable : MigrationBase
    {
        /// <summary>
        /// Tables in the order of creation or reverse deletion.
        /// </summary>
        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            { 0, typeof(ProductVariantDetachedContentDto) }
        };

        /// <summary>
        /// Creates the ProductVariant2DetachedContentType table
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            if (!database.TableExist("merchProductVariantDetachedContent"))
            {
                DatabaseSchemaHelper.InitializeDatabaseSchema(database, OrderedTables, "Merchello 1.12.0 upgrade");
            }
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.12.0 database to a prior version, the database schema has already been modified");
        }
    }
}