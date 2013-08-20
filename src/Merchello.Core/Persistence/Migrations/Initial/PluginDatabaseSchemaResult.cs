using System;
using System.Linq;
using System.Text;
using Merchello.Core.Configuration;
using Merchello.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.Migrations.Initial;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Core.Persistence.Migrations.Initial
{
    /// <summary>
    /// Class to override Umbraco's DatabaseSchemaResult with Merchello specifics
    /// </summary>
    public class PluginDatabaseSchemaResult : DatabaseSchemaResult
    {

        /// <summary>
        /// Determines the version of the currently installed database.
        /// </summary>
        /// <returns>
        /// A <see cref="Version"/> with Major and Minor values for 
        /// non-empty database, otherwise "0.0.0" for empty databases.
        /// </returns>
        public new Version DetermineInstalledVersion()
        {
            //If (ValidTables.Count == 0) database is empty and we return -> new Version(0, 0, 0);
            if (ValidTables.Count == 0)
                return new Version(0, 0, 0);

            //If Errors is empty or if TableDefinitions tables + columns correspond to valid tables + columns then we're at current version
            if (!Errors.Any() ||
                (TableDefinitions.All(x => ValidTables.Contains(x.Name))
                 && TableDefinitions.SelectMany(definition => definition.Columns).All(x => ValidColumns.Contains(x.Name))))
                return MerchelloVersion.Current;


            return MerchelloVersion.Current;
        }

        /// <summary>
        /// Gets a summary of the schema validation result
        /// </summary>
        /// <returns>A string containing a human readable string with a summary message</returns>
        /// <remarks>
        /// This needed to be overriden because of the SqlSyntax singleton
        /// </remarks>
        public new string GetSummary()
        {
            var sb = new StringBuilder();
            if (Errors.Any() == false)
            {
                sb.AppendLine("The database schema validation didn't find any errors.");
                return sb.ToString();
            }

            //Table error summary
            if (Errors.Any(x => x.Item1.Equals("Table")))
            {
                sb.AppendLine("The following tables were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", Errors.Where(x => x.Item1.Equals("Table")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }
            //Column error summary
            if (Errors.Any(x => x.Item1.Equals("Column")))
            {
                sb.AppendLine("The following columns were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", Errors.Where(x => x.Item1.Equals("Column")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }
            //Constraint error summary
            if (Errors.Any(x => x.Item1.Equals("Constraint")))
            {
                sb.AppendLine("The following constraints (Primary Keys, Foreign Keys and Indexes) were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", Errors.Where(x => x.Item1.Equals("Constraint")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }
            //Unknown constraint error summary
            if (Errors.Any(x => x.Item1.Equals("Unknown")))
            {
                sb.AppendLine("The following unknown constraints (Primary Keys, Foreign Keys and Indexes) were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", Errors.Where(x => x.Item1.Equals("Unknown")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }

            if (PluginSqlSyntaxContext.SqlSyntaxProvider is MySqlSyntaxProvider)
            {
                sb.AppendLine("Please note that the constraints could not be validated because the current dataprovider is MySql.");
            }

            return sb.ToString();
        }
    }
}
