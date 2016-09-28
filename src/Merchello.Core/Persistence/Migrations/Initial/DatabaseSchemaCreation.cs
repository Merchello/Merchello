namespace Merchello.Core.Persistence.Migrations.Initial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.Persistence.DatabaseModelDefinitions;
    using Merchello.Core.Events;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    /// <inheritdoc/>
    internal class DatabaseSchemaCreation : IDatabaseSchemaCreation
    {
        #region All Ordered Tables

        /// <summary>
        /// The ordered tables.
        /// </summary>
        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            { 1, typeof(LockDto) },
            { 2, typeof(MigrationStatusDto) },
            { 10, typeof(TypeFieldDto) },
            { 20, typeof(DetachedContentTypeDto) },
            { 30, typeof(AnonymousCustomerDto) },
            { 40, typeof(CustomerDto) },
            { 50, typeof(CustomerAddressDto) },
            { 60, typeof(ItemCacheDto) },
            { 70, typeof(ItemCacheItemDto) },
            { 80, typeof(GatewayProviderSettingsDto) },
            { 90, typeof(WarehouseDto) },
            { 100, typeof(WarehouseCatalogDto) },
            { 110, typeof(ShipCountryDto) },
            { 120, typeof(ShipMethodDto) },
            { 130, typeof(ShipRateTierDto) },
            { 140, typeof(InvoiceStatusDto) },
            { 150, typeof(InvoiceDto) },
            { 160, typeof(InvoiceItemDto) },
            { 170, typeof(OrderStatusDto) },
            { 180, typeof(OrderDto) },
            { 190, typeof(ShipmentStatusDto) },
            { 200, typeof(ShipmentDto) },
            { 210, typeof(OrderItemDto) },
            { 220, typeof(PaymentMethodDto) },
            { 230, typeof(PaymentDto) },
            { 240, typeof(ProductDto) },
            { 250, typeof(ProductVariantDto) },
            { 260, typeof(ProductOptionDto) },
            { 270, typeof(ProductAttributeDto) },
            { 280, typeof(Product2ProductOptionDto) },
            { 290, typeof(CatalogInventoryDto) },
            { 300, typeof(TaxMethodDto) },
            { 310, typeof(ProductVariant2ProductAttributeDto) },
            { 320, typeof(AppliedPaymentDto) },
            { 330, typeof(ProductVariantIndexDto) },
            { 340, typeof(StoreSettingDto) },
            { 350, typeof(NotificationMethodDto) },
            { 360, typeof(NotificationMessageDto) },
            { 370, typeof(AuditLogDto) },
            { 380, typeof(OfferSettingsDto) },
            { 390, typeof(OfferRedeemedDto) },
            { 400, typeof(EntityCollectionDto) },
            { 410, typeof(Product2EntityCollectionDto) },
            { 420, typeof(Invoice2EntityCollectionDto) },
            { 430, typeof(Customer2EntityCollectionDto) },
            { 440, typeof(ProductVariantDetachedContentDto) },
            { 450, typeof(NoteDto) },
            { 460, typeof(ProductOptionAttributeShareDto) }
        };

        #endregion

        /// <summary>
        /// The <see cref="IDatabaseSchemaManager"/>.
        /// </summary>
        private readonly IDatabaseSchemaManager _schemaManager;

        /// <summary>
        /// The <see cref="Database"/>.
        /// </summary>
        private readonly IMerchelloDatabase _db;

        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchemaCreation"/> class.
        /// </summary>
        /// <param name="database">
        /// The <see cref="IMerchelloDatabase"/>.
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        /// <param name="schemaManager">
        /// The <see cref="IDatabaseSchemaManager"/>.
        /// </param>
        public DatabaseSchemaCreation(IMerchelloDatabase database, ILogger logger, IDatabaseSchemaManager schemaManager)
        {
            Ensure.ParameterNotNull(database, nameof(database));
            this._db = database;
            this._logger = logger;
            this._schemaManager = schemaManager;
        }


        #region Events

        /// <summary>
        /// The save event handler
        /// </summary>
        /// <param name="e">
        /// The database creation event arguments.
        /// </param>
        internal delegate void DatabaseEventHandler(DatabaseCreationEventArgs e);

        /// <summary>
        /// Occurs when [before save].
        /// </summary>
        internal static event DatabaseEventHandler BeforeCreation;

        /// <summary>
        /// Occurs when [after save].
        /// </summary>
        internal static event DatabaseEventHandler AfterCreation;



        #endregion

        /// <summary>
        /// Initializes the database by creating the umbraco db schema.
        /// </summary>
        public void InitializeDatabaseSchema()
        {
            var e = new DatabaseCreationEventArgs();
            this.FireBeforeCreation(e);

            if (e.Cancel == false)
            {
                foreach (var item in OrderedTables.OrderBy(x => x.Key))
                {
                    this._schemaManager.CreateTable(false, item.Value);
                }
            }

            this.FireAfterCreation(e);
        }

        /// <summary>
        /// Validates the schema of the current database.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseSchemaResult"/>.
        /// </returns>
        public DatabaseSchemaResult ValidateSchema()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var result = new DatabaseSchemaResult(this._db.SqlSyntax);

            // get the db index defs
            result.DbIndexDefinitions = this._db.SqlSyntax.GetDefinedIndexes(this._db.Database)
                .Select(x => new DbIndexDefinition
                {
                    TableName = x.Item1,
                    IndexName = x.Item2,
                    ColumnName = x.Item3,
                    IsUnique = x.Item4
                }).ToArray();

            result.TableDefinitions.AddRange(OrderedTables
                .OrderBy(x => x.Key)
                .Select(x => DefinitionFactory.GetTableDefinition(x.Value, this._db.SqlSyntax)));

            this.ValidateDbTables(result);
            this.ValidateDbColumns(result);
            this.ValidateDbIndexes(result);
            this.ValidateDbConstraints(result);

            return result;
        }

        /// <summary>
        /// Drops all Merchello tables in the database.
        /// </summary>
        public void UninstallDatabaseSchema()
        {
            this._logger.Info<DatabaseSchemaCreation>("Start Merchello UninstallDatabaseSchema");

            foreach (var item in OrderedTables.OrderByDescending(x => x.Key))
            {
                var tableNameAttribute = item.Value.FirstAttribute<TableNameAttribute>();

                var tableName = tableNameAttribute == null ? item.Value.Name : tableNameAttribute.Value;

                this._logger.Info<DatabaseSchemaCreation>("Uninstall" + tableName);

                try
                {
                    if (this._schemaManager.TableExist(tableName))
                    {
                        this._schemaManager.DropTable(tableName);
                    }
                }
                catch (Exception ex)
                {
                    // swallow this for now, not sure how best to handle this with diff databases... though this is internal
                    // and only used for unit tests. If this fails its because the table doesn't exist... generally!
                    this._logger.Error<DatabaseSchemaCreation>("Could not drop table " + tableName, ex);
                }
            }
        }


        /// <summary>
        /// Raises the <see cref="BeforeCreation"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected internal virtual void FireBeforeCreation(DatabaseCreationEventArgs e)
        {
            if (BeforeCreation != null)
            {
                BeforeCreation(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="AfterCreation"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void FireAfterCreation(DatabaseCreationEventArgs e)
        {
            if (AfterCreation != null)
            {
                AfterCreation(e);
            }
        }

        /// <summary>
        /// Validates database constraints.
        /// </summary>
        /// <param name="result">
        /// The <see cref="DatabaseSchemaResult"/>.
        /// </param>
        private void ValidateDbConstraints(DatabaseSchemaResult result)
        {
            // Check constraints in configured database against constraints in schema
            var constraintsInDatabase = this._db.SqlSyntax.GetConstraintsPerColumn(this._db.Database).DistinctBy(x => x.Item3).ToList();
            var foreignKeysInDatabase = constraintsInDatabase.Where(x => x.Item3.InvariantStartsWith("FK_")).Select(x => x.Item3).ToList();
            var primaryKeysInDatabase = constraintsInDatabase.Where(x => x.Item3.InvariantStartsWith("PK_")).Select(x => x.Item3).ToList();
            var indexesInDatabase = constraintsInDatabase.Where(x => x.Item3.InvariantStartsWith("IX_")).Select(x => x.Item3).ToList();
            var indexesInSchema = result.TableDefinitions.SelectMany(x => x.Indexes.Select(y => y.Name)).ToList();
            var unknownConstraintsInDatabase =
                constraintsInDatabase.Where(
                    x =>
                    x.Item3.InvariantStartsWith("FK_") == false && x.Item3.InvariantStartsWith("PK_") == false &&
                    x.Item3.InvariantStartsWith("IX_") == false).Select(x => x.Item3).ToList();
            var foreignKeysInSchema = result.TableDefinitions.SelectMany(x => x.ForeignKeys.Select(y => y.Name)).ToList();
            var primaryKeysInSchema = result.TableDefinitions.SelectMany(x => x.Columns.Select(y => y.PrimaryKeyName))
                .Where(x => x.IsNullOrWhiteSpace() == false).ToList();

            // Add valid and invalid foreign key differences to the result object
            // We'll need to do invariant contains with case insensitivity because foreign key, primary key, and even index naming w/ MySQL is not standardized
            // In theory you could have: FK_ or fk_ ...or really any standard that your development department (or developer) chooses to use.
            foreach (var unknown in unknownConstraintsInDatabase)
            {
                if (foreignKeysInSchema.InvariantContains(unknown) || primaryKeysInSchema.InvariantContains(unknown) || indexesInSchema.InvariantContains(unknown))
                {
                    result.ValidConstraints.Add(unknown);
                }
                else
                {
                    result.Errors.Add(new Tuple<string, string>("Unknown", unknown));
                }
            }

            // Foreign keys:
            var validForeignKeyDifferences = foreignKeysInDatabase.Intersect(foreignKeysInSchema, StringComparer.InvariantCultureIgnoreCase);
            foreach (var foreignKey in validForeignKeyDifferences)
            {
                result.ValidConstraints.Add(foreignKey);
            }

            var invalidForeignKeyDifferences =
                foreignKeysInDatabase.Except(foreignKeysInSchema, StringComparer.InvariantCultureIgnoreCase)
                                .Union(foreignKeysInSchema.Except(foreignKeysInDatabase, StringComparer.InvariantCultureIgnoreCase));
            foreach (var foreignKey in invalidForeignKeyDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Constraint", foreignKey));
            }

            // Primary keys:
            // Add valid and invalid primary key differences to the result object
            var validPrimaryKeyDifferences = primaryKeysInDatabase.Intersect(primaryKeysInSchema, StringComparer.InvariantCultureIgnoreCase);
            foreach (var primaryKey in validPrimaryKeyDifferences)
            {
                result.ValidConstraints.Add(primaryKey);
            }

            var invalidPrimaryKeyDifferences =
                primaryKeysInDatabase.Except(primaryKeysInSchema, StringComparer.InvariantCultureIgnoreCase)
                                .Union(primaryKeysInSchema.Except(primaryKeysInDatabase, StringComparer.InvariantCultureIgnoreCase));
            foreach (var primaryKey in invalidPrimaryKeyDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Constraint", primaryKey));
            }

            // Constaints:
            // REFACTOR - should be able to remove these since we should have not legacy
            // NOTE: SD: The colIndex checks above should really take care of this but I need to keep this here because it was here before
            // and some schema validation checks might rely on this data remaining here!
            // Add valid and invalid index differences to the result object
            var validIndexDifferences = indexesInDatabase.Intersect(indexesInSchema, StringComparer.InvariantCultureIgnoreCase);
            foreach (var index in validIndexDifferences)
            {
                result.ValidConstraints.Add(index);
            }

            var invalidIndexDifferences =
                indexesInDatabase.Except(indexesInSchema, StringComparer.InvariantCultureIgnoreCase)
                                .Union(indexesInSchema.Except(indexesInDatabase, StringComparer.InvariantCultureIgnoreCase));
            foreach (var index in invalidIndexDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Constraint", index));
            }
        }

        /// <summary>
        /// Validates database columns.
        /// </summary>
        /// <param name="result">
        /// The <see cref="DatabaseSchemaResult"/>.
        /// </param>
        private void ValidateDbColumns(DatabaseSchemaResult result)
        {
            // Check columns in configured database against columns in schema
            var columnsInDatabase = _db.SqlSyntax.GetColumnsInSchema(this._db.Database);
            var columnsPerTableInDatabase = columnsInDatabase.Select(x => string.Concat(x.TableName, ",", x.ColumnName)).ToList();
            var columnsPerTableInSchema = result.TableDefinitions.SelectMany(x => x.Columns.Select(y => string.Concat(y.TableName, ",", y.Name))).ToList();
            
            // Add valid and invalid column differences to the result object
            var validColumnDifferences = columnsPerTableInDatabase.Intersect(columnsPerTableInSchema, StringComparer.InvariantCultureIgnoreCase);
            foreach (var column in validColumnDifferences)
            {
                result.ValidColumns.Add(column);
            }

            var invalidColumnDifferences =
                columnsPerTableInDatabase.Except(columnsPerTableInSchema, StringComparer.InvariantCultureIgnoreCase)
                                .Union(columnsPerTableInSchema.Except(columnsPerTableInDatabase, StringComparer.InvariantCultureIgnoreCase));
            foreach (var column in invalidColumnDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Column", column));
            }
        }

        /// <summary>
        /// Validates the database tables.
        /// </summary>
        /// <param name="result">
        /// The <see cref="DatabaseSchemaResult"/>.
        /// </param>
        private void ValidateDbTables(DatabaseSchemaResult result)
        {
            // Check tables in configured database against tables in schema
            var tablesInDatabase = _db.SqlSyntax.GetTablesInSchema(this._db.Database).ToList();
            var tablesInSchema = result.TableDefinitions.Select(x => x.Name).ToList();

            // Add valid and invalid table differences to the result object
            var validTableDifferences = tablesInDatabase.Intersect(tablesInSchema, StringComparer.InvariantCultureIgnoreCase);
            foreach (var tableName in validTableDifferences)
            {
                result.ValidTables.Add(tableName);
            }

            var invalidTableDifferences =
                tablesInDatabase.Except(tablesInSchema, StringComparer.InvariantCultureIgnoreCase)
                                .Union(tablesInSchema.Except(tablesInDatabase, StringComparer.InvariantCultureIgnoreCase));
            foreach (var tableName in invalidTableDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Table", tableName));
            }
        }

        /// <summary>
        /// Validates the database indexes
        /// </summary>
        /// <param name="result">
        /// The <see cref="DatabaseSchemaResult"/>.
        /// </param>
        private void ValidateDbIndexes(DatabaseSchemaResult result)
        {
            // These are just column indexes NOT constraints or Keys
            var colIndexesInDatabase = result.DbIndexDefinitions.Select(x => x.IndexName).ToList();
            var indexesInSchema = result.TableDefinitions.SelectMany(x => x.Indexes.Select(y => y.Name)).ToList();

            // Add valid and invalid index differences to the result object
            var validColIndexDifferences = colIndexesInDatabase.Intersect(indexesInSchema, StringComparer.InvariantCultureIgnoreCase);
            foreach (var index in validColIndexDifferences)
            {
                result.ValidIndexes.Add(index);
            }

            var invalidColIndexDifferences =
                colIndexesInDatabase.Except(indexesInSchema, StringComparer.InvariantCultureIgnoreCase)
                                .Union(indexesInSchema.Except(colIndexesInDatabase, StringComparer.InvariantCultureIgnoreCase));
            foreach (var index in invalidColIndexDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Index", index));
            }
        }
    }
}