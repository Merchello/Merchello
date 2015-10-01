namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneElevenZero
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

   // using DatabaseSchemaHelper = Merchello.Core.Persistence.Migrations.DatabaseSchemaHelper;

    /// <summary>
    /// The create product 2 product collection table.
    /// </summary>
    [Migration("1.10.0", "1.11.0", 5, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateProduct2EntityCollectionTable : IMerchelloMigration
    {
        /// <summary>
        /// The schema helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _schemaHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProduct2EntityCollectionTable"/> class.
        /// </summary>
        public CreateProduct2EntityCollectionTable()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _schemaHelper = new DatabaseSchemaHelper(dbContext.Database, LoggerResolver.Current.Logger, dbContext.SqlSyntax);
        }

        /// <summary>
        /// Creates the merchProduct2ProductCollection table in the database
        /// </summary>
        public void Up()
        {
            if (!_schemaHelper.TableExist("merchProduct2EntityCollection"))
            {
                _schemaHelper.CreateTable(false, typeof(Product2EntityCollectionDto));
            }
        }

        /// <summary>
        /// The down.
        /// </summary>
        /// <exception cref="DataLossException">
        /// Throws a data loss exception on a downgrade attempt
        /// </exception>
        public void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.11.0 database to a prior version, the database schema has already been modified");
        }
    }
}