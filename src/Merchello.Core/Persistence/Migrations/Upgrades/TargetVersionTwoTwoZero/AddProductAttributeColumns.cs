namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoTwoZero
{
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Adds columns to the merchProductAttribute table.
    /// </summary>
    [Migration("2.2.0", 2, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddProductAttributeColumns : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Updates the merchProductAttribute table adding the columns.
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

            // 'shared' column
            if (columns.Any(
                  x => x.TableName.InvariantEquals("merchProductAttribute") && x.ColumnName.InvariantEquals("detachedContentValues"))
              == false)
            {
                Logger.Info(typeof(AddProductAttributeColumns), "Adding detachedContentValues column to merchProductAttribute table.");

                //// Add the new 'shared' column
                var textType = SqlSyntax.GetSpecialDbType(SpecialDbTypes.NTEXT);
                Create.Column("detachedContentValues").OnTable("merchProductAttribute").AsCustom(textType).Nullable();
            }

            // the 'sharedCount' column
            if (columns.Any(
                    x =>
                    x.TableName.InvariantEquals("merchProductAttribute") && x.ColumnName.InvariantEquals("useCount"))
                == false)
            {
                Logger.Info(typeof(AddProductAttributeColumns), "Adding useCount column to merchProductAttribute table.");

                //// Add the new 'shared' column
                Create.Column("useCount").OnTable("merchProductAttribute").AsInt16().Nullable();

                SetUseCountValue(database);

                Alter.Table("merchProductAttribute").AlterColumn("useCount").AsInt16().NotNullable();
            }

        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.2.0 database to a prior version, the database schema has already been modified");
        }

        /// <summary>
        /// Updates the UseCount field.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        private void SetUseCountValue(Database database)
        {
            var dtos = database.Fetch<KeyDto>("SELECT pk FROM merchProductAttribute");
            foreach (var dto in dtos)
            {
                // we are setting the useCount here to 1 because all choices for previous versions had to be used
                // and are already be associated with a product variant
                Update.Table("merchProductAttribute")
                        .Set(new { useCount = 1 })
                        .Where(new { pk = dto.Key });
            }
        }
    }
}