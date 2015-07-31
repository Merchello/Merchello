namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneElevenZero
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// The create detached published content type table.
    /// </summary>
    [Migration("1.10.0", "1.10.1.1", 3, MerchelloConfiguration.MerchelloMigrationName)]
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
                DatabaseSchemaHelper.InitializeDatabaseSchema(database, OrderedTables, "Merchello 1.11.0 upgrade");
                var entity = new EntityTypeField();
                database.Insert(
                    "merchDetachedContentType", 
                    "Key", 
                    new DetachedContentTypeDto()
                    {
                        Key = Constants.DefaultKeys.DetachedPublishedContentType.DefaultProductVariantDetachedPublishedContentTypeKey, 
                        Name = "No Extended Content",
                        EntityTfKey = entity.Product.TypeKey,
                        ContentTypeId = null,
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
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.11.0 database to a prior version, the database schema has already been modified");
        }
    }
}