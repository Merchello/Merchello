namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneTwelveZero
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    using DatabaseSchemaHelper = Merchello.Core.Persistence.Migrations.DatabaseSchemaHelper;

    /// <summary>
    /// The create detached published content type table.
    /// </summary>
    [Migration("1.11.0", "1.11.0.1", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateDetachedPublishedContentTypeTable : MigrationBase 
    {
        /// <summary>
        /// Tables in the order of creation or reverse deletion.
        /// </summary>
        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            { 0, typeof(DetachedContentTypeDto) }
        };

        /// <summary>
        /// Adds the merchDetatchedPublishedContentType table to the database
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            if (!database.TableExist("merchDetachedContentType"))
            {
                DatabaseSchemaHelper.InitializeDatabaseSchema(database, OrderedTables, "Merchello 1.12.0 upgrade");

                ////var entity = new EntityTypeField();
                ////database.Insert(
                ////    "merchDetachedContentType", 
                ////    "Key", 
                ////    new DetachedContentTypeDto()
                ////    {
                ////        Key = Constants.DefaultKeys.DetachedPublishedContentType.DefaultProductVariantDetachedPublishedContentTypeKey, 
                ////        Name = "No Extended Content",
                ////        EntityTfKey = entity.Product.TypeKey,
                ////        ContentTypeKey = null,
                ////        UpdateDate = DateTime.Now, 
                ////        CreateDate = DateTime.Now
                ////    });
            }
        }

        /// <summary>
        /// The down.
        /// </summary>
        /// <exception cref="DataLossException">'
        /// Throws a data loss exception on a downgrade attempt
        /// </exception>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.12.0 database to a prior version, the database schema has already been modified");
        }
    }
}