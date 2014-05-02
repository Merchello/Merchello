using System;
using Umbraco.Core.Persistence.Migrations;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneZero
{
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