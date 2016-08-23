namespace Merchello.Core.Persistence.Migrations.Upgrades.TargerVersionTwoThreeZero
{
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoTwoZero;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Extends the merchEntityCollection table with addition columns.
    /// </summary>
    [Migration("2.3.0", 2, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddEntityCollectionColumns : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Updates the merchEntityCollection table adding the columns.
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

            // 'isFilter' column
            if (
                columns.Any(
                    x =>
                    x.TableName.InvariantEquals("merchEntityCollection") && x.ColumnName.InvariantEquals("extendedData"))
                == false)
            {
                
            }

            // 'isFilter' column
            if (columns.Any(
                  x => x.TableName.InvariantEquals("merchEntityCollection") && x.ColumnName.InvariantEquals("isFilter"))
              == false)
            {
                Logger.Info(typeof(AddEntityCollectionColumns), "Adding isFilter column to merchEntityCollection table.");

                //// Add the new 'isDefaultChoice' column
                Create.Column("isFilter").OnTable("merchEntityCollection").AsBoolean().WithDefaultValue(false);
            }
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.3.0 database to a prior version, the database schema has already been modified");
        }
    }
}