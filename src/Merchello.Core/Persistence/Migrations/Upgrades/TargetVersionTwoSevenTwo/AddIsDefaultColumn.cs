using System;
using System.Linq;
using Merchello.Core.Configuration;
using Umbraco.Core;
using Umbraco.Core.Persistence.Migrations;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoSevenTwo
{
    /// <summary>
    ///     Adds new IsDefault column to product variants which makes it easier to display the default selected variant
    /// </summary>
    [Migration("2.7.2", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddIsDefaultColumn : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

            // 'isDefault' column
            if (columns.Any(x => x.TableName.InvariantEquals("merchProductVariant") && x.ColumnName.InvariantEquals("isDefault")) == false)
            {
                Logger.Info(typeof(AddIsDefaultColumn), "Adding IsDefault column to merchProductVariant table.");

                //// Add the new 'isDefaultChoice' column
                Create.Column("isDefault").OnTable("merchProductVariant").AsBoolean().WithDefaultValue(false);
            }
        }

        /// <summary>
        ///     Downgrades the database.
        /// </summary>
        /// <exception cref="DataLossException"></exception>
        public override void Down()
        {
            throw new DataLossException(
                "Cannot downgrade from version 2.7.2 database to a prior version, the database schema has already been modified");
        }
    }
}