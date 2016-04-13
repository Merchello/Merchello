namespace Merchello.Core.Persistence.Migrations.Initial
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

            if (StoreSettings.All(x => x.Key != Constants.StoreSettingKeys.GlobalTaxationApplicationKey))
            {
                return new Version(1, 9, 0);
            }

            if (!this.ValidTables.Contains("merchEntityCollection")
                || !this.ValidTables.Contains("merchProduct2EntityCollection"))
            {
                return new Version(1, 10, 0);
            }

            if (!this.ValidTables.Contains("merchDetachedContentType")
                || !this.ValidTables.Contains("merchProductVariantDetachedContent"))
            {
                return new Version(1, 11, 0);
            }

            if (!this.ValidColumns.Contains("merchInvoice,currencyCode"))
            {
                return new Version(1, 13, 0);
            }

            if (!this.ValidColumns.Contains("merchNote,internalOnly") ||
                StoreSettings.All(x => x.Key != Constants.StoreSettingKeys.HasDomainRecordKey) ||
                !this.ValidColumns.Contains("merchNote,author") ||
                this.ValidColumns.Contains("merchCustomer,notes"))
            {
                return new Version(1, 14, 1);
            }

            //// If Errors is empty or if TableDefinitions tables + columns correspond to valid tables + columns then we're at current version
            if (this.MerchelloErrors.Any() == false ||
                (this.TableDefinitions.All(x => this.ValidTables.Contains(x.Name))
                 && this.TableDefinitions.SelectMany(definition => definition.Columns).All(x => this.ValidColumns.Contains(x.Name))))
                return MerchelloVersion.Current;

            return MerchelloVersion.Current;
        }

    }
}
