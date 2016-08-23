namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoTwoZero
{
    using System;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Responsible for adding the ProductOption Entity type field key.
    /// </summary>
    [Migration("2.2.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddProductOptionTypeField : MerchelloMigrationBase, IMerchelloMigration
    {

        /// <summary>
        /// Updates the merchTypeField table adding the ProductOption type field value.
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;

            var entity = new EntityTypeField();
            var dtos = database.Fetch<TypeFieldDto>("SELECT * FROM merchTypeField WHERE merchTypeField.pk = @key", new { @key = entity.ProductOption.TypeKey });

            if (!dtos.Any())
            database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.ProductOption.TypeKey, Alias = entity.ProductOption.Alias, Name = entity.ProductOption.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.2.0 database to a prior version, the database schema has already been modified");
        }
    }
}