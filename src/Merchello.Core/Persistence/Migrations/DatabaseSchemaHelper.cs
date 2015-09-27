namespace Merchello.Core.Persistence.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// The database schema helper.
    /// </summary>
    internal sealed class DatabaseSchemaHelperOld
    {
        /// <summary>
        /// The uninstall database schema.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="orderedTables">
        /// The ordered tables.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        internal static void UninstallDatabaseSchema(Database database, Dictionary<int, Type> orderedTables, string version)
        {
            LogHelper.Info<DatabaseSchemaHelper>(string.Format("Start UninstallDataSchema {0}", version));

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

        /// <summary>
        /// The initialize database schema.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="orderedTables">
        /// The ordered tables.
        /// </param>
        /// <param name="migrationName">
        /// The migration name.
        /// </param>
        internal static void InitializeDatabaseSchema(Database database, Dictionary<int, Type> orderedTables, string migrationName)
        {
            var logger = Logger.CreateWithDefaultLog4NetConfiguration();
            logger.Info<DatabaseSchemaHelper>(string.Format("Start InstallDataSchema {0}", migrationName));

            foreach (var item in orderedTables.OrderBy(x => x.Key))
            {
                database.CreateTable(false, item.Value);
            }
        }
    }
}