﻿namespace Merchello.Core.Persistence.Migrations.Initial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.DatabaseModelDefinitions;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations.Initial;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Class to override Umbraco DatabaseSchemaResult with Merchello specifics
    /// </summary>
    public class MerchelloDatabaseSchemaResult : DatabaseSchemaResult
    {

        /// <summary>
        /// Gets or sets the merchello errors.
        /// </summary>
        public IEnumerable<Tuple<string, string>> MerchelloErrors 
        {
            get
            {
                return this.Errors.Where(x => x.Item2.Contains("merch"));
            }
        }

        /// <summary>
        /// Gets or sets the database index definitions.
        /// </summary>
        internal IEnumerable<DbIndexDefinition> DbIndexDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the type fields.
        /// </summary>
        /// <remarks>
        /// These can be helpful when determining the Merchello Version
        /// </remarks>
        internal IEnumerable<TypeFieldDto> TypeFields { get; set; }

        /// <summary>
        /// Gets or sets the store settings.
        /// </summary>
        /// <remarks>
        /// These can be helpful when determining the Merchello Version
        /// </remarks>
        internal IEnumerable<StoreSettingDto> StoreSettings { get; set; } 

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
            if (this.ValidTables.Count == 0)
                return new Version(0, 0, 0);

            if (this.StoreSettings.All(x => x.Key != Constants.StoreSettingKeys.MigrationKey))
            {
                return new Version(1, 7, 0);
            }


            //// If Errors is empty or if TableDefinitions tables + columns correspond to valid tables + columns then we're at current version
            if (this.MerchelloErrors.Any() == false ||
                (this.TableDefinitions.All(x => this.ValidTables.Contains(x.Name))
                 && this.TableDefinitions.SelectMany(definition => definition.Columns).All(x => this.ValidColumns.Contains(x.Name))))
                return MerchelloVersion.Current;


            //// if the error is for umbracoServer
            if (this.MerchelloErrors.Any(x => x.Item1.Equals("Table") && x.Item2.InvariantEquals("merchOfferSettings")))
            {
                return new Version(1, 8, 3);
            }


            return MerchelloVersion.Current;
        }

    }
}
