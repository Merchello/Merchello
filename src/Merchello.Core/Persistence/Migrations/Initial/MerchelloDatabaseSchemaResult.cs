namespace Merchello.Core.Persistence.Migrations.Initial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.DatabaseModelDefinitions;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations.Initial;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Class to override Umbraco DatabaseSchemaResult with Merchello specifics
    /// </summary>
    public class MerchelloDatabaseSchemaResult : DatabaseSchemaResult
    {
        /// <summary>
        /// Gets or sets the database index definitions.
        /// </summary>
        internal IEnumerable<DbIndexDefinition> DbIndexDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the merchello errors.
        /// </summary>
        public IEnumerable<Tuple<string, string>> MerchelloErrors 
        {
            get
            {
                return Errors.Where(x => x.Item2.Contains("merch"));
            }
        }

        /// <summary>
        /// Determines the version of the currently installed database.
        /// </summary>
        /// <returns>
        /// A <see cref="Version"/> with Major and Minor values for 
        /// non-empty database, otherwise "0.0.0" for empty databases.
        /// </returns>
        public new Version DetermineInstalledVersion()
        {
            //// If (ValidTables.Count == 0) database is empty and we return -> new Version(0, 0, 0);
            if (ValidTables.Count == 0)
                return new Version(0, 0, 0);

            //// If Errors is empty or if TableDefinitions tables + columns correspond to valid tables + columns then we're at current version
            if (MerchelloErrors.Any() == false ||
                (TableDefinitions.All(x => ValidTables.Contains(x.Name))
                 && TableDefinitions.SelectMany(definition => definition.Columns).All(x => ValidColumns.Contains(x.Name))))
                return MerchelloVersion.Current;

            //// if the error is for umbracoServer
            if (Errors.Any(x => x.Item1.Equals("Table") && x.Item2.InvariantEquals("merchCampaignSettings")))
            {
                return new Version(1, 7, 0);
            }


            return MerchelloVersion.Current;
        }

    }
}
