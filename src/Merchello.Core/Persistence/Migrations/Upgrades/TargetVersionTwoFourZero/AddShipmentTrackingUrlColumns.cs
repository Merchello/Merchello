namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoFourZero
{
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoZeroZero;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Adds the shipment tracking Url field.
    /// </summary>
    [Migration("2.4.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddShipmentTrackingUrlColumns : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Updates the merchShipment table adding the trackingUrl field.
        /// </summary>
        public override void Up()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var database = dbContext.Database;

            var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

            if (
                columns.Any(
                    x => x.TableName.InvariantEquals("merchShipment") && x.ColumnName.InvariantEquals("trackingUrl"))
                == false)
            {
                Logger.Info(typeof(AddShipmentTrackingUrlColumns), "Adding trackingUrl colument to merchShipment table.");

                //// Add the new currency code column
                Create.Column("trackingUrl").OnTable("merchShipment").AsString(1000).Nullable();
            }
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.4.0 database to a prior version, the database schema has already been modified");
        }
    }
}