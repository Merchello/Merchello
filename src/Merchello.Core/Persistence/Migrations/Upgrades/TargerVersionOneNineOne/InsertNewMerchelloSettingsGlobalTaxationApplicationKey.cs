namespace Merchello.Core.Persistence.Migrations.Upgrades.TargerVersionOneNineOne
{
    using System;

    using Merchello.Core.Configuration;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Insert the new merchello settings GlobalTaxationApplication setting.
    /// </summary>
    [Migration("1.7.0", "1.9.0.1", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class InsertNewMerchelloSettingsGlobalTaxationApplicationKey : MigrationBase
    {
        /// <summary>
        /// The <see cref="Database"/>.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertNewMerchelloSettingsGlobalTaxationApplicationKey"/> class.
        /// </summary>
        public InsertNewMerchelloSettingsGlobalTaxationApplicationKey()
            : this(ApplicationContext.Current.DatabaseContext.Database)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertNewMerchelloSettingsGlobalTaxationApplicationKey"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        public InsertNewMerchelloSettingsGlobalTaxationApplicationKey(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// Adds the settings key
        /// </summary>
        public override void Up()
        {
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Core.Constants.StoreSettingKeys.GlobalTaxationApplicationKey, Name = "globalTaxationApplication", Value = TaxationApplication.Invoice.ToString(), TypeName = "System.Guid", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

        /// <summary>
        /// The down.
        /// </summary>
        /// <exception cref="DataLossException">
        /// Throws an exception if a downgrade is attempted
        /// </exception>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.9.1 database to a prior version, the database schema has already been modified");
        }
    }
}