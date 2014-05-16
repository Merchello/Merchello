using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using Merchello.Core.Configuration;
using Merchello.Core.Events;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;

namespace Merchello.Core.Persistence.Migrations
{
    internal sealed class DatabaseSchemaHelper
    {
        internal static void UninstallDatabaseSchema(Database database, Dictionary<int, Type> orderedTables, string version)
        {
            LogHelper.Info<DatabaseSchemaHelper>(String.Format("Start UninstallDataSchema {0}", version));

            foreach (var item in orderedTables.OrderByDescending(x => x.Key))
            {
                var tableNameAttribute = item.Value.FirstAttribute<TableNameAttribute>();

                string tableName = tableNameAttribute == null ? item.Value.Name : tableNameAttribute.Value;

                LogHelper.Info<DatabaseSchemaHelper>("Uninstall" + tableName);

                try
                {
                    if (database.TableExist(tableName))
                    {
                        database.DropTable(tableName);
                    }
                }
                catch (Exception ex)
                {
                    //swallow this for now, not sure how best to handle this with diff databases... though this is internal
                    // and only used for unit tests. If this fails its because the table doesn't exist... generally!
                    LogHelper.Error<DatabaseSchemaHelper>("Could not drop table " + tableName, ex);
                }
            }
        }

        internal static void InitializeDatabaseSchema(Database database, Dictionary<int, Type> orderedTables, string migrationName)
        {
            LogHelper.Info<DatabaseSchemaHelper>(String.Format("Start InstallDataSchema {0}", migrationName));

            foreach (var item in orderedTables.OrderBy(x => x.Key))
            {
                database.CreateTable(false, item.Value);
            }
         
        }

        /// <summary>
        /// Determines if the database schema needs to be upgraded with a version update
        /// </summary>
        internal static bool VerifyDatabaseSchema(Database database)
        {
            if (!MerchelloConfiguration.Current.AutoUpdateDbSchema) return true;

            // Check if merchello has been upgraded
            if (MerchelloConfiguration.ConfigurationStatus == MerchelloVersion.Current.ToString()) return true;

            LogHelper.Info<CoreBootManager>("Beginning Merchello DB Schema Upgrade");

            // 1.0.1 was Merchello's first public release
            var configVersion = String.IsNullOrEmpty(MerchelloConfiguration.ConfigurationStatus)
                ? "1.0.1"
                : MerchelloConfiguration.ConfigurationStatus;

            var currentVersion = new Version(configVersion);
            var targetVersion = MerchelloVersion.Current;

            var isUpgrage = IsUpgrade(currentVersion, targetVersion);

            var migration = new MigrationRunner(currentVersion, MerchelloVersion.Current, MerchelloConfiguration.MerchelloMigrationName);

            if (migration.Execute(database, isUpgrage))
            {
                try
                {
                    // update the web.config merchelloConfigurationVersion
                    var config = WebConfigurationManager.OpenWebConfiguration("~");
                    config.AppSettings.Settings["merchelloConfigurationStatus"].Value = MerchelloVersion.Current.ToString();

                    config.Save(ConfigurationSaveMode.Modified);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<CoreBootManager>("Failed to update 'merchelloConfigurationStatus' AppSetting", ex);
                    return false;
                }
            }

            LogHelper.Info<CoreBootManager>("Finished Merchello DB Schema Upgrade");

            return true;
        }

        private static bool IsUpgrade(Version config, Version target)
        {
            if (target.Major > config.Major) return true;
            if (target.Minor > config.Minor) return true;
            return target.MinorRevision > config.MinorRevision; // we should not be doing database updates in minor version
        }
    }
}