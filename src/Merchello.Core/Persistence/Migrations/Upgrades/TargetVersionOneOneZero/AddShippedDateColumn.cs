using System;
using Merchello.Core.Configuration;
using Umbraco.Core.Persistence.Migrations;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneZero
{
    /// <summary>
    /// Adds the shippedDate field to the shipment table
    /// </summary>
    [Migration("1.1.0", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddShippedDateColumn : MigrationBase 
    {
        public override void Up()
        {
            Alter.Table("merchShipment").AddColumn("shippedDate").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now);
        }

        public override void Down()
        {
            Delete.Column("shippedDate").FromTable("merchShipment");
        }
    }
}