using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Configuration;
using Umbraco.Core.Persistence.Migrations.Initial;

namespace Merchello.Core.Persistence.Migrations.Initial
{
    public class DatabaseShemaResult : DatabaseSchemaResult
    {
        /// <summary>
        /// Determines the version of the currently installed database.
        /// </summary>
        /// <returns>
        /// A <see cref="Version"/> with Major and Minor values for 
        /// non-empty database, otherwise "0.0.0" for empty databases.
        /// </returns>
        new public Version DetermineInstalledVersion()
        {
            if (ValidTables.Count == 0)
                return new Version(0, 0, 0);

            //If Errors is empty or if TableDefinitions tables + columns correspond to valid tables + columns then we're at current version
            if (!Errors.Any() ||
                (TableDefinitions.All(x => ValidTables.Contains(x.Name))
                 && TableDefinitions.SelectMany(definition => definition.Columns).All(x => ValidColumns.Contains(x.Name))))
                return MerchelloVersion.Current;
           

            return MerchelloVersion.Current;
        }

    }
}
