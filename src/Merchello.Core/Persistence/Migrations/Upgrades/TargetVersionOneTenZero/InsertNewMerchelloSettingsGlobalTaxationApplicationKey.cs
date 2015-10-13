namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneTenZero
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
    [Migration("1.7.0", "1.10.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class InsertNewMerchelloSettingsGlobalTaxationApplicationKey : IMerchelloMigration
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
            this._database = database;
        }

        /// <summary>
        /// Adds the settings key
        /// </summary>
        public void Up()
        {
            this._database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Core.Constants.StoreSettingKeys.GlobalTaxationApplicationKey, Name = "globalTaxationApplication", Value = TaxationApplication.Invoice.ToString(), TypeName = "System.Guid", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

        /// <summary>
        /// Removes the key
        /// </summary>
        public void Down()
        {
            this._database.Delete("merchStoreSetting", "pk", null, Core.Constants.StoreSettingKeys.GlobalTaxationApplicationKey);
        }
    }
}