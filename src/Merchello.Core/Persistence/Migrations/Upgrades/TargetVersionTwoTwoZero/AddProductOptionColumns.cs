namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoTwoZero
{
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoZeroZero;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Adds the "shared" field to the merchProductOption table for shared option refactoring.
    /// </summary>
    [Migration("2.2.0", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddProductOptionColumns : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Updates the merchProductOption table adding the shared and detached content type key fields.
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

            // 'shared' column
            if (columns.Any(
                  x => x.TableName.InvariantEquals("merchProductOption") && x.ColumnName.InvariantEquals("shared"))
              == false)
            {
                Logger.Info(typeof(AddProductOptionColumns), "Adding shared column to merchProductOption table.");

                //// Add the new 'shared' column
                Create.Column("shared").OnTable("merchProductOption").AsBoolean().WithDefaultValue(false);
            }

            // 'detachedContentType' column
            if (columns.Any(x => x.TableName.InvariantEquals("merchProductOption") && x.ColumnName.InvariantEquals("detachedContentTypeKey"))
              == false)
            {
                Logger.Info(typeof(AddProductOptionColumns), "Adding detachedContentType column to merchProductOption table.");

                //// Add the new 'shared' column
                Create.Column("detachedContentTypeKey").OnTable("merchProductOption").AsGuid().Nullable()
                    .ForeignKey("FK_merchProductOptionDetachedContent_merchProductOption", "merchDetachedContentType", "pk");
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