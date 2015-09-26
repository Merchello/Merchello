namespace Merchello.Core.Persistence.Migrations
{
    using System;
    using System.Collections.Generic;

    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;

    internal static class DatabaseSchemaHelperExtensions
    {
        internal static void UninstallMerchelloDatabaseSchema(
            this DatabaseSchemaHelper dbSchemaHelper,
            Dictionary<int, Type> orderedTables,
            string version)
        {
            LogHelper.Info<DatabaseSchemaHelper>(string.Format("Start UninstallDataSchema {0}", version));
        }
    }
}