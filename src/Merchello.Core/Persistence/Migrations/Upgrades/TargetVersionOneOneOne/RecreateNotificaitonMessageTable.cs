using Merchello.Core.Configuration;
using Umbraco.Core.Persistence.Migrations;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneOne
{
    //[Migration("1.1.0", "1.1.1", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class RecreateNotificaitonMessageTable : MigrationBase
    {
        public override void Up()
        {
            throw new System.NotImplementedException();
        }

        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.1.0 database to a prior version, the database schema has already been modified");
        }
    }
}