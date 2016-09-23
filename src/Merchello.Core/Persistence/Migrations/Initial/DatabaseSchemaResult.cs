namespace Merchello.Core.Persistence.Migrations.Initial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Merchello.Core.Acquired.Persistence.DatabaseModelDefinitions;
    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.SqlSyntax;
    using Merchello.Core.Services;

    using Semver;

    /// <summary>
    /// Represents a database schema result for storing information about the current database schema.
    /// </summary>
    internal class DatabaseSchemaResult
    {
        /// <summary>
        /// The <see cref="ISqlSyntaxProvider"/>.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntax;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchemaResult"/> class.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The <see cref="ISqlSyntaxProvider"/>.
        /// </param>
        public DatabaseSchemaResult(ISqlSyntaxProvider sqlSyntax)
        {
            this._sqlSyntax = sqlSyntax;
            this.Errors = new List<Tuple<string, string>>();
            this.TableDefinitions = new List<TableDefinition>();
            this.ValidTables = new List<string>();
            this.ValidColumns = new List<string>();
            this.ValidConstraints = new List<string>();
            this.ValidIndexes = new List<string>();
        }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public List<Tuple<string, string>> Errors { get; set; }

        /// <summary>
        /// Gets or sets the table definitions.
        /// </summary>
        public List<TableDefinition> TableDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the valid tables.
        /// </summary>
        public List<string> ValidTables { get; set; }

        /// <summary>
        /// Gets or sets the valid columns.
        /// </summary>
        public List<string> ValidColumns { get; set; }

        /// <summary>
        /// Gets or sets the valid constraints.
        /// </summary>
        public List<string> ValidConstraints { get; set; }

        /// <summary>
        /// Gets or sets the valid indexes.
        /// </summary>
        public List<string> ValidIndexes { get; set; }

        /// <summary>
        /// Gets or sets the database index definitions.
        /// </summary>
        internal IEnumerable<DbIndexDefinition> DbIndexDefinitions { get; set; }

        /// <summary>
        /// Checks in the db which version is installed based on the migrations that have been run
        /// </summary>
        /// <param name="migrationStatusService">The <see cref="IMigrationStatusService"/></param>
        /// <returns>The <see cref="SemVersion"/></returns>
        public SemVersion DetermineInstalledVersionByMigrations(IMigrationStatusService migrationStatusService)
        {
            var allMigrations = migrationStatusService.GetAll(Constants.MerchelloMigrationName);
            var mostrecent = allMigrations.OrderByDescending(x => x.Version).Select(x => x.Version).FirstOrDefault();

            return mostrecent ?? new SemVersion(new Version(0, 0, 0));
        }

        /// <summary>
        /// Determines the version of the currently installed database by detecting the current database structure
        /// </summary>
        /// <returns>
        /// A <see cref="Version"/> with Major and Minor values for 
        /// non-empty database, otherwise "0.0.0" for empty databases.
        /// </returns>
        public Version DetermineInstalledVersion()
        {
            // If (ValidTables.Count == 0) database is empty and we return -> new Version(0, 0, 0);
            if (this.ValidTables.Count == 0)
                return new Version(0, 0, 0);

            // If Errors is empty or if TableDefinitions tables + columns correspond to valid tables + columns then we're at current version
            if (this.Errors.Any() == false ||
                (this.TableDefinitions.All(x => this.ValidTables.Contains(x.Name))
                 && this.TableDefinitions.SelectMany(definition => definition.Columns).All(x => this.ValidColumns.Contains(x.Name))))
                return MerchelloVersion.Current;

            // TODO- starting at version 2.x
           
            return MerchelloVersion.Current;
        }

        /// <summary>
        /// Gets a summary of the schema validation result
        /// </summary>
        /// <returns>A string containing a human readable string with a summary message</returns>
        public string GetSummary()
        {
            var sb = new StringBuilder();
            if (this.Errors.Any() == false)
            {
                sb.AppendLine("The database schema validation didn't find any errors.");
                return sb.ToString();
            }

            // Table error summary
            if (this.Errors.Any(x => x.Item1.Equals("Table")))
            {
                sb.AppendLine("The following tables were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", this.Errors.Where(x => x.Item1.Equals("Table")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }
            
            // Column error summary
            if (this.Errors.Any(x => x.Item1.Equals("Column")))
            {
                sb.AppendLine("The following columns were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", this.Errors.Where(x => x.Item1.Equals("Column")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }

            // Constraint error summary
            if (this.Errors.Any(x => x.Item1.Equals("Constraint")))
            {
                sb.AppendLine("The following constraints (Primary Keys, Foreign Keys and Indexes) were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", this.Errors.Where(x => x.Item1.Equals("Constraint")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }
            
            // Index error summary
            if (this.Errors.Any(x => x.Item1.Equals("Index")))
            {
                sb.AppendLine("The following indexes were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", this.Errors.Where(x => x.Item1.Equals("Index")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }
            
            // Unknown constraint error summary
            if (this.Errors.Any(x => x.Item1.Equals("Unknown")))
            {
                sb.AppendLine("The following unknown constraints (Primary Keys, Foreign Keys and Indexes) were found in the database, but are not in the current schema:");
                sb.AppendLine(string.Join(",", this.Errors.Where(x => x.Item1.Equals("Unknown")).Select(x => x.Item2)));
                sb.AppendLine(" ");
            }

            return sb.ToString();
        }
    }
}