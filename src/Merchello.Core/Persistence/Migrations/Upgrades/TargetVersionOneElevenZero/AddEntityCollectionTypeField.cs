namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneElevenZero
{
    using System;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// The add entity collection type field.
    /// </summary>
    [Migration("1.10.0", "1.11.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddEntityCollectionTypeField : IMerchelloMigration
    {
        /// <summary>
        /// The _database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEntityCollectionTypeField"/> class.
        /// </summary>
        public AddEntityCollectionTypeField()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        /// <summary>
        /// Inserts the entity collection type field.
        /// </summary>
        public void Up()
        {
            var entity = new EntityTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.EntityCollection.TypeKey, Alias = entity.EntityCollection.Alias, Name = entity.EntityCollection.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
        }

        /// <summary>
        /// Deletes the entity collection type field.
        /// </summary>
        public void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.9.0 database to a prior version, the database schema has already been modified");
        }
    }
}