namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneEightTwo
{
    using System;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// The insert new type field data for Merchello version 1.8.2.
    /// </summary>
    [Migration("1.7.0", "1.8.1.2", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class InsertNewMerchelloSettings : MigrationBase
    {
        /// <summary>
        /// Upgrade migration
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Core.Constants.StoreSettingKeys.MigrationKey, Name = "migration", Value = Guid.NewGuid().ToString(), TypeName = "System.Guid", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

        /// <summary>
        /// Downgrade migration
        /// </summary>
        /// <exception cref="DataLossException">
        /// Throws a data loss exception
        /// </exception>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.8.2 database to a prior version, the database schema has already been modified");
        }
    }
}