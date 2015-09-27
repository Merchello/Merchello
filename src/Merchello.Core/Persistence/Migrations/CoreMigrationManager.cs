namespace Merchello.Core.Persistence.Migrations
{
    using System;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Events;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Migrations.Analytics;
    using Merchello.Core.Persistence.Migrations.Initial;

    using Semver;

    using umbraco.BusinessLogic;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// The merchello upgrade helper.
    /// </summary>
    internal class CoreMigrationManager
    {
        /// <summary>
        /// The <see cref="Database"/>.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// The _sql syntax provider.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreMigrationManager"/> class.
        /// </summary>
        public CoreMigrationManager()
            : this(ApplicationContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreMigrationManager"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        public CoreMigrationManager(ApplicationContext applicationContext)
            : this(applicationContext.DatabaseContext.Database, applicationContext.DatabaseContext.SqlSyntax, LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreMigrationManager"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="sqlSyntaxProvider">
        /// The SQL Syntax Provider.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CoreMigrationManager(Database database, ISqlSyntaxProvider sqlSyntaxProvider, ILogger logger)
        {
            Mandate.ParameterNotNull(database, "database");
            Mandate.ParameterNotNull(sqlSyntaxProvider, "sqlSyntaxProvider");
            Mandate.ParameterNotNull(logger, "logger");
            
            _database = database;
            _sqlSyntaxProvider = sqlSyntaxProvider;
            _logger = logger;
        }

        /// <summary>
        /// The delegate for the upgraded event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="MerchelloMigrationEventArgs"/>.
        /// </param>
        public delegate void UpgradedEventHandler(object sender, MerchelloMigrationEventArgs e);

        /// <summary>
        /// The upgraded event.
        /// </summary>
        public event UpgradedEventHandler Upgraded;

        /// <summary>
        /// Checks the binary version against the web.config configuration status version.
        /// </summary>
        public void EnsureMerchelloVersion()
        {
            if (MerchelloConfiguration.ConfigurationStatusVersion != MerchelloVersion.Current)
            {
                LogHelper.Info<CoreMigrationManager>(
                    "Merchello Versions did not match - initializing upgrade.");

                if (UpgradeMerchello(_database))
                {
                    LogHelper.Info<CoreMigrationManager>("Upgrade completed successfully.");
                }
            }
            else
            {
                LogHelper.Debug<CoreMigrationManager>("Merchello Version Verified - no upgrade required.");
            }
        }

        /// <summary>
        /// Executes the Migration runner.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the migration was successful.
        /// </returns>
        private bool UpgradeMerchello(Database database)
        {
            var databaseSchemaCreation = new DatabaseSchemaCreation(_database, _logger, new DatabaseSchemaHelper(_database, _logger, _sqlSyntaxProvider), _sqlSyntaxProvider);
            var schemaResult = databaseSchemaCreation.ValidateSchema();
            var dbVersion = schemaResult.DetermineInstalledVersion();

            if (dbVersion != MerchelloVersion.Current)
            {
                try
                {
                    _logger.Info<CoreMigrationManager>("Merchello database upgraded required.  Initializing Upgrade.");
                    var entryService = ApplicationContext.Current.Services.MigrationEntryService;
                    var runner = new MigrationRunner(
                        ApplicationContext.Current.Services.MigrationEntryService,
                        _logger,
                        new SemVersion(MerchelloConfiguration.ConfigurationStatusVersion),
                        new SemVersion(MerchelloVersion.Current),
                        MerchelloConfiguration.MerchelloMigrationName,
                        null);
                    var upgraded = runner.Execute(database);
                    if (upgraded)
                    {
                        var migrationKey = this.EnsureMigrationKey(schemaResult);

                        var record = new MigrationRecord()
                                         {
                                             MigrationKey = migrationKey,
                                             CurrentVersion = dbVersion.ToString(),
                                             TargetVersion = MerchelloVersion.Current.ToString(),
                                             DbProvider = database.GetDatabaseProvider().ToString(),
                                             InstallDate = DateTime.Now,
                                             IsUpgrade = true
                                         };

                        this.OnUpgraded(record);

                        _logger.Info<CoreMigrationManager>("Merchello Schema Migration completed successfully");
                    }

                    _logger.Debug<CoreMigrationManager>("Merchello migration runner returned false.");
                }
                catch (Exception ex)
                {
                    _logger.Error<CoreMigrationManager>("Merchello Database Schema Upgrade Failed", ex);
                    throw;
                }
            }
            else
            {
                    // this is a new install                  
                    var migrationKey = this.EnsureMigrationKey(schemaResult);

                    var record = new MigrationRecord()
                                     {
                                         MigrationKey = migrationKey,
                                         CurrentVersion = MerchelloConfiguration.ConfigurationStatus,
                                         TargetVersion = MerchelloVersion.Current.ToString(),
                                         DbProvider = database.GetDatabaseProvider().ToString(),
                                         InstallDate = DateTime.Now,
                                         IsUpgrade = !MerchelloConfiguration.ConfigurationStatus.Equals("0.0.0")
                                     };
                    this.OnUpgraded(record);
            }
            
            MerchelloConfiguration.ConfigurationStatus = MerchelloVersion.Current.ToString();

            return true;
        }

        /// <summary>
        /// The ensure migration key.
        /// </summary>
        /// <param name="schemaResult">
        /// The schema result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string EnsureMigrationKey(MerchelloDatabaseSchemaResult schemaResult)
        {
            var migrationSetting =
                            schemaResult.StoreSettings.FirstOrDefault(
                                x => x.Key == Constants.StoreSettingKeys.MigrationKey);

            var nullSettingKey = Guid.NewGuid().ToString();
            if (migrationSetting == null)
            {
                this.InsertMigrationKey(nullSettingKey);
            }

            var migrationKey = migrationSetting != null ? 
                string.IsNullOrEmpty(migrationSetting.Value) ? nullSettingKey : migrationSetting.Value : 
                nullSettingKey;

            Guid validGuid;
            if (Guid.TryParse(migrationKey, out validGuid))            
            if (validGuid.Equals(Guid.Empty))
            {
                // reset the key
                nullSettingKey = Guid.NewGuid().ToString();
                var dto = migrationSetting;
                if (dto != null)
                {
                    dto.Value = nullSettingKey;
                    _database.Update(dto);
                    migrationKey = nullSettingKey;
                }
            }
                         
            return migrationKey;
        }

        /// <summary>
        /// The insert migration key.
        /// </summary>
        /// <param name="migrationKey">
        /// The migration key.
        /// </param>
        private void InsertMigrationKey(string migrationKey)
        {
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Core.Constants.StoreSettingKeys.MigrationKey, Name = "migration", Value = migrationKey, TypeName = "System.Guid", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

        /// <summary>
        /// The on upgraded.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        private void OnUpgraded(MigrationRecord record)
        {
            if (Upgraded != null)
            {
                Upgraded(this, new MerchelloMigrationEventArgs(record));
            }
        }
    }
}