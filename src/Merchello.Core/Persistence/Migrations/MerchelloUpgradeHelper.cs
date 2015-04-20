namespace Merchello.Core.Persistence.Migrations
{
    using System;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.Migrations.Analytics;
    using Merchello.Core.Persistence.Migrations.Initial;

    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// The merchello upgrade helper.
    /// </summary>
    internal sealed class MerchelloUpgradeHelper
    {
        /// <summary>
        /// Checks the binary version against the web.config configuration status version.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool CheckConfigurationStatusVersion()
        {
            return MerchelloConfiguration.ConfigurationStatusVersion == MerchelloVersion.Current;
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
        public static bool UpgradeMerchello(Database database)
        {
            var databaseSchemaCreation = new DatabaseSchemaCreation(database);
            var schemaResult = databaseSchemaCreation.ValidateSchema();
            var dbVersion = schemaResult.DetermineInstalledVersion();

            if (dbVersion != MerchelloVersion.Current)
            {
                try
                {
                    LogHelper.Info<MerchelloUpgradeHelper>("Merchello database upgraded required.  Initializing Upgrade.");
                    var runner = new MigrationRunner(MerchelloConfiguration.ConfigurationStatusVersion, MerchelloVersion.Current, MerchelloConfiguration.MerchelloMigrationName);
                    var upgraded = runner.Execute(database);
                    if (upgraded)
                    {

                        var record = new MigrationRecord()
                                         {

                                             TargetVersion = MerchelloVersion.Current.ToString(),
                                             DbProvider = database.GetDatabaseProvider().ToString(),
                                             InstallDate = DateTime.Now,
                                             IsUpgrade = true
                                         };

                        LogHelper.Info<MerchelloUpgradeHelper>("Merchello Schema Migration completed successfully");
                    }

                    LogHelper.Debug<MerchelloUpgradeHelper>("Merchello migration runner returned false.");
                }
                catch (Exception ex)
                {
                    LogHelper.Error<MerchelloUpgradeHelper>("Merchello Database Schema Upgrade Failed", ex);
                    throw;
                }
            }
            
            MerchelloConfiguration.ConfigurationStatus = MerchelloVersion.Current.ToString();

            return true;
        }

    }
}