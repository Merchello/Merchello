namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneTwelveZero
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    //using DatabaseSchemaHelper = Merchello.Core.Persistence.Migrations.DatabaseSchemaHelper;

    /// <summary>
    /// The create detached published content type table.
    /// </summary>
    [Migration("1.11.0", "1.12.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateDetachedPublishedContentTypeTable : IMerchelloMigration 
    {    
        /// <summary>
        /// The _database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// The schema helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _schemaHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDetachedPublishedContentTypeTable"/> class.
        /// </summary>
        public CreateDetachedPublishedContentTypeTable()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
            _schemaHelper = new DatabaseSchemaHelper(dbContext.Database, LoggerResolver.Current.Logger, dbContext.SqlSyntax);
        }

        /// <summary>
        /// Adds the merchDetachedPublishedContentType table to the database
        /// </summary>
        public void Up()
        {
            if (!_schemaHelper.TableExist("merchDetachedContentType"))
            {
                _schemaHelper.CreateTable(false, typeof(DetachedContentTypeDto));

                var entity = new EntityTypeField();
                _database.Insert(
                    "merchDetachedContentType",
                    "Key",
                    new DetachedContentTypeDto()
                    {
                        Key = Core.Constants.DetachedPublishedContentType.DefaultProductVariantDetachedPublishedContentTypeKey,
                        Name = "No Extended Content",
                        EntityTfKey = entity.Product.TypeKey,
                        ContentTypeKey = null,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now
                    });
            }
        }

        /// <summary>
        /// The down.
        /// </summary>
        /// <exception cref="DataLossException">'
        /// Throws a data loss exception on a downgrade attempt
        /// </exception>
        public void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.12.0 database to a prior version, the database schema has already been modified");
        }
    }
}