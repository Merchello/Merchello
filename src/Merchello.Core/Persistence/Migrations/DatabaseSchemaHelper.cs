using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Events;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence.Migrations
{
    internal sealed class DatabaseSchemaHelper
    {
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

        internal static void InitializeDatabaseSchema(Database database, Dictionary<int, Type> orderedTables, string migrationName)
        {
            LogHelper.Info<DatabaseSchemaHelper>(string.Format("Start InstallDataSchema {0}", migrationName));

            foreach (var item in orderedTables.OrderBy(x => x.Key))
            {
                database.CreateTable(false, item.Value);
            }
         
        }
    }
}