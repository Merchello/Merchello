namespace Merchello.Core.Persistence.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Merchello.Core.Configuration;
    using Merchello.Core.Events;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Migrations.Analytics;
    using Merchello.Core.Persistence.Migrations.Initial;

    using Semver;

    using umbraco.BusinessLogic;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;
    using Umbraco.Web;

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
        /// The SQL syntax provider.
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
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        /// <summary>
        /// Checks the binary version against the web.config configuration status version.
        /// </summary>
        public void EnsureMerchelloVersion()
        {
            if (MerchelloConfiguration.ConfigurationStatusVersion != MerchelloVersion.Current)
            {
                _logger.Info<CoreMigrationManager>(
                    "Merchello Versions did not match - initializing upgrade.");

                if (UpgradeMerchello(_database))
                {
                    _logger.Info<CoreMigrationManager>("Upgrade completed successfully.");
                }
            }
            else
            {
                _logger.Debug<CoreMigrationManager>("Merchello Version Verified - no upgrade required.");
            }
        }


        /// <summary>
        /// Ensures the Merchello database has been installed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool EnsureDatabase()
        {
            var databaseSchemaCreation = new DatabaseSchemaCreation(_database, _logger, new DatabaseSchemaHelper(_database, _logger, _sqlSyntaxProvider), _sqlSyntaxProvider);
            var schemaResult = databaseSchemaCreation.ValidateSchema();
            var databaseVersion = schemaResult.DetermineInstalledVersion();

            if (databaseVersion != new Version("0.0.0")) return true;
            
            // install the database
            var schemaHelper = new MerchelloDatabaseSchemaHelper(this._database, this._logger, this._sqlSyntaxProvider);
            schemaHelper.CreateDatabaseSchema();

            var baseDataCreation = new BaseDataCreation(this._database, this._logger);
            baseDataCreation.InitializeBaseData("merchTypeField");
            baseDataCreation.InitializeBaseData("merchInvoiceStatus");
            baseDataCreation.InitializeBaseData("merchOrderStatus");
            baseDataCreation.InitializeBaseData("merchWarehouse");
            baseDataCreation.InitializeBaseData("merchGatewayProviderSettings");
            baseDataCreation.InitializeBaseData("merchStoreSetting");
            baseDataCreation.InitializeBaseData("merchShipmentStatus");

            return false;
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

            var upgraded = false;

            if (dbVersion != MerchelloVersion.Current)
            {
                try
                {
                    _logger.Info<CoreMigrationManager>("Merchello database upgraded required.  Initializing Upgrade.");

                    var resolver = new MigrationResolver(_logger, PluginManager.Current.ResolveMerchelloMigrations());

                    var migrations = resolver.OrderedUpgradeMigrations(
                        MerchelloConfiguration.ConfigurationStatusVersion,
                        MerchelloVersion.Current).ToList();

                    var context = InitializeMigrations(migrations, _database, _database.GetDatabaseProvider());

                    try
                    {
                        ExecuteMigrations(context, _database);

                        upgraded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error<CoreMigrationManager>("Merchello migration failed", ex);
                        upgraded = false;
                    }
                    

                    _logger.Debug<CoreMigrationManager>("Merchello migration runner returned false.");
                }
                catch (Exception ex)
                {
                    _logger.Error<CoreMigrationManager>("Merchello Database Schema Upgrade Failed", ex);
                    throw;
                }
            }

            var currentVersion = dbVersion.ToString();

            if (!upgraded)
            {
                currentVersion = MerchelloConfiguration.ConfigurationStatusVersion.ToString();
            }

            var migrationKey = this.EnsureMigrationKey(schemaResult);

            var record = new MigrationRecord()
            {
                MigrationKey = migrationKey,
                CurrentVersion = currentVersion,
                TargetVersion = MerchelloVersion.Current.ToString(),
                DbProvider = database.GetDatabaseProvider().ToString(),
                InstallDate = DateTime.Now,
                IsUpgrade = currentVersion != "0.0.0"
            };

            this.OnUpgraded(record);

            _logger.Info<CoreMigrationManager>("Merchello Schema Migration completed successfully");
  
            MerchelloConfiguration.ConfigurationStatus = MerchelloVersion.Current.ToString();

            return true;
        }

        /// <summary>
        /// Initializes the Merchell Migrations.
        /// </summary>
        /// <param name="migrations">
        /// The migrations.
        /// </param>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="databaseProvider">
        /// The database provider.
        /// </param>
        /// <param name="isUpgrade">
        /// The is upgrade.
        /// </param>
        /// <returns>
        /// The <see cref="MerchelloMigrationContext"/>.
        /// </returns>
        internal MerchelloMigrationContext InitializeMigrations(List<IMigration> migrations, Database database, DatabaseProviders databaseProvider, bool isUpgrade = true)
        {
            //Loop through migrations to generate sql
            var context = new MerchelloMigrationContext(databaseProvider, database, _logger);

            foreach (var migration in migrations)
            {
                var baseMigration = migration as MerchelloMigrationBase;
                if (baseMigration != null)
                {
                    if (isUpgrade)
                    {
                        baseMigration.GetUpExpressions(context);
                        _logger.Info<CoreMigrationManager>(string.Format("Added UPGRADE migration '{0}' to context", baseMigration.GetType().Name));
                    }
                    else
                    {
                        baseMigration.GetDownExpressions(context);
                        _logger.Info<CoreMigrationManager>(string.Format("Added DOWNGRADE migration '{0}' to context", baseMigration.GetType().Name));
                    }
                }
                else
                {
                    //this is just a normal migration so we can only call Up/Down
                    if (isUpgrade)
                    {
                        migration.Up();
                        _logger.Info<MigrationRunner>(string.Format("Added UPGRADE migration '{0}' to context", migration.GetType().Name));
                    }
                    else
                    {
                        migration.Down();
                        _logger.Info<MigrationRunner>(string.Format("Added DOWNGRADE migration '{0}' to context", migration.GetType().Name));
                    }
                }
            }

            return context;
        }


        private void ExecuteMigrations(IMigrationContext context, Database database)
        {
            //Transactional execution of the sql that was generated from the found migrations
            using (var transaction = database.GetTransaction())
            {
                int i = 1;
                foreach (var expression in context.Expressions)
                {
                    var sql = expression.Process(database);
                    if (string.IsNullOrEmpty(sql))
                    {
                        i++;
                        continue;
                    }

                    //TODO: We should output all of these SQL calls to files in a migration folder in App_Data/TEMP
                    // so if people want to executed them manually on another environment, they can.

                    //The following ensures the multiple statement sare executed one at a time, this is a requirement
                    // of SQLCE, it's unfortunate but necessary.
                    // http://stackoverflow.com/questions/13665491/sql-ce-inconsistent-with-multiple-statements
                    var sb = new StringBuilder();
                    using (var reader = new StringReader(sql))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (line.Equals("GO", StringComparison.OrdinalIgnoreCase))
                            {
                                //Execute the SQL up to the point of a GO statement
                                var exeSql = sb.ToString();
                                _logger.Info<MigrationRunner>("Executing sql statement " + i + ": " + exeSql);
                                database.Execute(exeSql);

                                //restart the string builder
                                sb.Remove(0, sb.Length);
                            }
                            else
                            {
                                sb.AppendLine(line);
                            }
                        }
                        //execute anything remaining
                        if (sb.Length > 0)
                        {
                            var exeSql = sb.ToString();
                            _logger.Info<MigrationRunner>("Executing sql statement " + i + ": " + exeSql);
                            database.Execute(exeSql);
                        }
                    }

                    i++;
                }

                transaction.Complete();

            }
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
                                x => x.Key == Constants.StoreSetting.MigrationKey);

            var nullSettingKey = Guid.NewGuid().ToString();
            if (migrationSetting == null)
            {
                this.InsertMigrationKey(nullSettingKey);
            }

            var migrationKey = migrationSetting != null ? 
                string.IsNullOrEmpty(migrationSetting.Value) ? nullSettingKey : migrationSetting.Value : 
                nullSettingKey;

            // Saves a previously saved migration key without a value
            if (migrationKey.Equals(nullSettingKey))
            {
                var setting =
                    MerchelloContext.Current.Services.StoreSettingService.GetByKey(
                        Constants.StoreSetting.MigrationKey);
                if (setting != null) setting.Value = migrationKey;
                MerchelloContext.Current.Services.StoreSettingService.Save(setting);
            }

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
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Core.Constants.StoreSetting.MigrationKey, Name = "migration", Value = migrationKey, TypeName = "System.Guid", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
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