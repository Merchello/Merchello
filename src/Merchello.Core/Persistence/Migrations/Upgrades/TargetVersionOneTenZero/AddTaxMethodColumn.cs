namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneTenZero
{
    using Merchello.Core.Configuration;

    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Adds the productTaxMethod field to the merchelloTaxMethod table.
    /// </summary>
    [Migration("1.7.0", "1.10.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddTaxMethodColumn : MigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Adds the productTaxation field on an upgrade
        /// </summary>
        public override void Up()
        {
            this.Alter.Table("merchTaxMethod").AddColumn("productTaxMethod").AsBoolean().NotNullable().WithDefaultValue('0');
        }

        /// <summary>
        /// Removes the productTaxMethod field on a downgrade
        /// </summary>        
        public override void Down()
        {
            this.Delete.Column("productTaxMethod").FromTable("merchTaxMethod");
        }
    }
}