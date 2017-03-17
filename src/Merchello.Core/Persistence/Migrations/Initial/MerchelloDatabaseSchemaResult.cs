﻿namespace Merchello.Core.Persistence.Migrations.Initial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.DatabaseModelDefinitions;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations.Initial;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Class to override Umbraco DatabaseSchemaResult with Merchello specifics
    /// </summary>
    public class MerchelloDatabaseSchemaResult : DatabaseSchemaResult
    {
		/// <summary>
		/// The <see cref="Database"/>.
		/// </summary>
		private readonly Database _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloDatabaseSchemaResult"/> class.
        /// </summary>
        public MerchelloDatabaseSchemaResult()
            : this(ApplicationContext.Current.DatabaseContext.Database)
        {
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="MerchelloDatabaseSchemaResult"/> class.
		/// </summary>
		/// <param name="database">
		/// The database.
		/// </param>
		public MerchelloDatabaseSchemaResult(Database database)
		{
		    Ensure.ParameterNotNull(database, "database");
		    this._database = database;
		}

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
        /// <remarks>
        /// TODO these checks could be moved into a task chain for easier maintenance
        /// </remarks>
        public new Version DetermineInstalledVersion()
        {
            //// If (ValidTables.Count == 0) database is empty and we return -> new Version(0, 0, 0);
            if (this.ValidTables.Count == 0)
                return new Version(0, 0, 0);

            if (this.StoreSettings.All(x => x.Key != Constants.StoreSetting.MigrationKey))
            {
                return new Version(1, 7, 0);
            }

            if (StoreSettings.All(x => x.Key != Constants.StoreSetting.GlobalTaxationApplicationKey))
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
                StoreSettings.All(x => x.Key != Constants.StoreSetting.HasDomainRecordKey) ||
                !this.ValidColumns.Contains("merchNote,author") ||
                !this.ValidColumns.Contains("merchCustomer,notes") ||
                this.TypeFields.All(x => x.Key != Constants.TypeFieldKeys.PaymentMethod.RedirectKey))
            {
                return new Version(1, 14, 1);
            }

            if (!this.ValidColumns.Contains("merchProductOption,shared") ||
                !this.ValidColumns.Contains("merchProductOption,detachedContentTypeKey") ||
                !this.ValidColumns.Contains("merchProductOption,uiOption") ||
                !this.ValidColumns.Contains("merchProductAttribute,detachedContentValues") ||
                !this.ValidColumns.Contains("merchProductAttribute,isDefaultChoice") ||
                !this.ValidColumns.Contains("merchProduct2ProductOption,useName") ||
                !this.ValidTables.Contains("merchProductOptionAttributeShare") ||
                this.TypeFields.All(x => x.Key != Constants.TypeFieldKeys.Entity.ProductOptionKey))
            {
                return new Version(2, 1, 0);
            }

			// SD: Not a very elegant solution to the problem of discovering the size of an existing column
			// Should perhaps look to refactor this into something reusable
            // RSS: Added a DatabaseExtensions class and moved SD code into extension method GetDbTableColumnSize using 
            // parameratized SQL query  
            var merchAppliedPaymentDescriptionSize = _database.GetDbTableColumnSize("merchAppliedPayment", "description");
					
            if (!this.ValidColumns.Contains("merchEntityCollection,isFilter")
                || !this.ValidColumns.Contains("merchEntityCollection,extendedData") 
				|| merchAppliedPaymentDescriptionSize != 500)
			{
				return new Version(2, 2, 0);
	        }

            if (this.ValidColumns.Contains("merchProductAttribute,detachedContentValues"))
            {
                var merchProductAttributeDetachedContentValuesSize = _database.GetDbTableColumnSize("merchProductAttribute", "detachedContentValues");
                if (merchProductAttributeDetachedContentValuesSize <= 500)
                {
                    return new Version(2, 3, 0);
                }
            }

            if (!this.ValidColumns.Contains("merchShipment,trackingUrl"))
            {
                return new Version(2, 3, 1);
            }

            if (!this.ValidIndexes.Contains("IX_merchProductVariantName") ||
                !this.ValidIndexes.Contains("IX_merchProductVariantPrice") ||
                !this.ValidIndexes.Contains("IX_merchProductVariantSalePrice") ||
                !this.ValidIndexes.Contains("IX_merchProductVariantBarcode") ||
                !this.ValidIndexes.Contains("IX_merchProductVariantManufacturer") ||
                !this.ValidIndexes.Contains("IX_merchInvoiceBillToPostalCode") ||
                !this.ValidTables.Contains("merchDigitalMedia"))
            {
                return new Version(2, 4, 0);
            }

	        // If Errors is empty or if TableDefinitions tables + columns correspond to valid tables + columns then we're at current version
            if (this.MerchelloErrors.Any() == false ||
                (this.TableDefinitions.All(x => this.ValidTables.Contains(x.Name))
                 && this.TableDefinitions.SelectMany(definition => definition.Columns).All(x => this.ValidColumns.Contains(x.Name))))
                return MerchelloVersion.Current;

            return MerchelloVersion.Current;
        }

    }
}
