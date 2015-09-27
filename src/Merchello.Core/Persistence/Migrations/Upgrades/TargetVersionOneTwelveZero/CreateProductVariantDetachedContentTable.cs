namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneTwelveZero
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;


    /// <summary>
    /// The create product variant 2 detached content type table.
    /// </summary>
    [Migration("1.11.0", "1.11.0.1", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateProductVariantDetachedContentTable : MigrationBase
    {
        /// <summary>
        /// The schema helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _schemaHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProductVariantDetachedContentTable"/> class.
        /// </summary>
        public CreateProductVariantDetachedContentTable()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _schemaHelper = new DatabaseSchemaHelper(dbContext.Database, LoggerResolver.Current.Logger, dbContext.SqlSyntax);
        }

        /// <summary>
        /// Creates the ProductVariant2DetachedContentType table
        /// </summary>
        public override void Up()
        {
            if (!_schemaHelper.TableExist("merchProductVariantDetachedContent"))
            {
                _schemaHelper.CreateTable(false, typeof(ProductVariantDetachedContentDto));
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