namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoTwoZero
{
    using System.Linq;

    using Merchello.Core.Configuration;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.DatabaseAnnotations;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Adds columns to the merchProduct2ProductOption table.
    /// </summary>
    [Migration("2.2.0", 4, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddProduct2ProductOptionColumns : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Updates the merchProduct2ProductOption table adding the columns.
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

            // 'shared' column
            if (columns.Any(
                  x => x.TableName.InvariantEquals("merchProduct2ProductOption") && x.ColumnName.InvariantEquals("useName"))
              == false)
            {
                Logger.Info(typeof(AddProduct2ProductOptionColumns), "Adding useName column to merchProduct2ProductOption table.");

                //// Add the new 'useName' column
                Create.Column("useName").OnTable("merchProduct2ProductOption").AsString().Nullable();
            }
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