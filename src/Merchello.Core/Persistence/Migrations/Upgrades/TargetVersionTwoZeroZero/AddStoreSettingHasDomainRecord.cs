namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoZeroZero
{
    using System;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Adds a new store setting that is toggled during migrations to ping with the current domain name.
    /// Used to assist Merchello project team (internally) to better understand Merchello usage and implementations anonymously.
    /// </summary>
    [Migration("1.14.0", "2.0.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    internal class AddStoreSettingHasDomainRecord : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The Umbraco database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddStoreSettingHasDomainRecord"/> class.
        /// </summary>
        public AddStoreSettingHasDomainRecord()
            : base(ApplicationContext.Current.DatabaseContext.SqlSyntax, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
        }

        /// <summary>
        /// Upgrades the database.
        /// </summary>
        public override void Up()
        {
            var storeSettingService = MerchelloContext.Current.Services.StoreSettingService;
            var setting = storeSettingService.GetByKey(Core.Constants.StoreSettingKeys.HasDomainRecordKey);
            if (setting == null)
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Core.Constants.StoreSettingKeys.HasDomainRecordKey, Name = "hasDomainRecord", Value = false.ToString(), TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

        /// <summary>
        /// Downgrades the database
        /// </summary>
        public override void Down()
        {
            var storeSettingService = MerchelloContext.Current.Services.StoreSettingService;
            var setting = storeSettingService.GetByKey(Core.Constants.StoreSettingKeys.HasDomainRecordKey);
            if (setting != null) storeSettingService.Delete(setting);
        }
    }
}