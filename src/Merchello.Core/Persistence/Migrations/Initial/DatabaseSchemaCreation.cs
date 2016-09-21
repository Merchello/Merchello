namespace Merchello.Core.Persistence.Migrations.Initial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DatabaseModelDefinitions;
    using Events;
    using Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;



    /// <summary>
    /// Represents the initial database schema creation by running CreateTable for all DTOs against the database.
    /// </summary>
    internal class DatabaseSchemaCreation
    {
        #region Private Members

        /// <summary>
        /// The ordered tables.
        /// </summary>
        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            { 0, typeof(TypeFieldDto) },
            { 1, typeof(DetachedContentTypeDto) },
            { 2, typeof(AnonymousCustomerDto) },
            { 3, typeof(CustomerDto) },
            { 4, typeof(CustomerIndexDto) },             
            { 5, typeof(CustomerAddressDto) },
            { 6, typeof(ItemCacheDto) },
            { 7, typeof(ItemCacheItemDto) },
            { 8, typeof(GatewayProviderSettingsDto) },
            { 9, typeof(WarehouseDto) },
            { 10, typeof(WarehouseCatalogDto) },                
            { 11, typeof(ShipCountryDto) },
            { 12, typeof(ShipMethodDto) },
            { 13, typeof(ShipRateTierDto) },                
            { 14, typeof(InvoiceStatusDto) },  
            { 15, typeof(InvoiceDto) },                   
            { 16, typeof(InvoiceItemDto) },
            { 17, typeof(InvoiceIndexDto) },
            { 18, typeof(OrderStatusDto) },
            { 19, typeof(OrderDto) },    
            { 20, typeof(ShipmentStatusDto) },                              
            { 21, typeof(ShipmentDto) },                 
            { 22, typeof(OrderItemDto) },
            { 23, typeof(PaymentMethodDto) }, 
            { 24, typeof(PaymentDto) },                
            { 25, typeof(ProductDto) },
            { 26, typeof(ProductVariantDto) },
            { 27, typeof(ProductOptionDto) },
            { 28, typeof(ProductAttributeDto) },
            { 29, typeof(Product2ProductOptionDto) },
            { 30, typeof(CatalogInventoryDto) },
            { 31, typeof(TaxMethodDto) },
            { 32, typeof(ProductVariant2ProductAttributeDto) },           
            { 33, typeof(AppliedPaymentDto) },
            { 34, typeof(ProductVariantIndexDto) },
            { 35, typeof(StoreSettingDto) },
            { 36, typeof(OrderIndexDto) },
            { 37, typeof(NotificationMethodDto) },
            { 38, typeof(NotificationMessageDto) },
            { 39, typeof(AuditLogDto) },
            { 40, typeof(OfferSettingsDto) },
            { 41, typeof(OfferRedeemedDto) },
            { 42, typeof(EntityCollectionDto) },
            { 43, typeof(Product2EntityCollectionDto) },
            { 44, typeof(Invoice2EntityCollectionDto) },
            { 45, typeof(Customer2EntityCollectionDto) },
            { 46, typeof(ProductVariantDetachedContentDto) },
            { 47, typeof(NoteDto) },
            { 48, typeof(ProductOptionAttributeShareDto) }
        };

        /// <summary>
        /// The database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// The Umbraco's <see cref="DatabaseSchemaHelper"/>.
        /// </summary>
        private readonly DatabaseSchemaHelper _umbSchemaHelper;

        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="ISqlSyntaxProvider"/>.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntax;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchemaCreation"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="databaseSchemaHelper">
        /// The database Schema Helper.
        /// </param>
        public DatabaseSchemaCreation(Database database, ILogger logger, DatabaseSchemaHelper databaseSchemaHelper, ISqlSyntaxProvider sqlSyntax)
        {
            _database = database;
            _logger = logger;
            _umbSchemaHelper = databaseSchemaHelper;
            _sqlSyntax = sqlSyntax;
        }


        /// <summary>
        /// Drops all Merchello tables in the database
        /// </summary>
        internal void UninstallDatabaseSchema()
        {
            _logger.Info<DatabaseSchemaCreation>("Start UninstallDatabaseSchema");

            foreach (var item in OrderedTables.OrderByDescending(x => x.Key))
            {
                var tableNameAttribute = item.Value.FirstAttribute<TableNameAttribute>();

                string tableName = tableNameAttribute == null ? item.Value.Name : tableNameAttribute.Value;

                _logger.Info<DatabaseSchemaCreation>("Uninstall" + tableName);

                try
                {
                    if (_umbSchemaHelper.TableExist(tableName))
                    {
                        _umbSchemaHelper.DropTable(tableName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error<DatabaseSchemaCreation>("Could not drop table " + tableName, ex);
                }
            }
        }


        /// <summary>
        /// Initialize the database by creating the umbraco database schema
        /// </summary>
        public void InitializeDatabaseSchema()
        {
            var e = new DatabaseCreationEventArgs();
            FireBeforeCreation(e);

            if (!e.Cancel)
            {
                foreach (var item in OrderedTables.OrderBy(x => x.Key))
                {
                    _umbSchemaHelper.CreateTable(false, item.Value);   
                }
            }

            FireAfterCreation(e);
        }

        /// <summary>
        /// Validates the schema of the current database
        /// </summary>
        /// <returns>
        /// The <see cref="MerchelloDatabaseSchemaResult"/>.
        /// </returns>
        public MerchelloDatabaseSchemaResult ValidateSchema()
        {
            var result = new MerchelloDatabaseSchemaResult(_database)
            {
                DbIndexDefinitions = SqlSyntaxContext.SqlSyntaxProvider.GetDefinedIndexes(_database)
                    .Select(x => new DbIndexDefinition()
                    {
                        TableName = x.Item1,
                        IndexName = x.Item2,
                        ColumnName = x.Item3,
                        IsUnique = x.Item4
                    }).ToArray()
            };

            //get the db index defs

            foreach (var item in OrderedTables.OrderBy(x => x.Key))
            {
                var tableDefinition = DefinitionFactory.GetTableDefinition(item.Value);
                result.TableDefinitions.Add(tableDefinition);
            }

            ValidateDbTables(result);

            ValidateDbColumns(result);

            ValidateDbIndexes(result);

            ValidateDbConstraints(result);

            if (
                !result.MerchelloErrors.Any(
                    x => x.Item1.Equals("Table") && x.Item2.InvariantEquals("merchStoreSetting")))
            {
                // catch this so it doesn't kick off on an install
                LoadMerchelloData(result);
            }
            else
            {
                result.StoreSettings = Enumerable.Empty<StoreSettingDto>();
                result.TypeFields = Enumerable.Empty<TypeFieldDto>();
            }


            return result;
        }

        /// <summary>
        /// The load merchello data.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private void LoadMerchelloData(MerchelloDatabaseSchemaResult result)
        {
            var sqlSettings = new Sql();
            sqlSettings.Select("*")
                .From<StoreSettingDto>(_sqlSyntax);

            result.StoreSettings = _database.Fetch<StoreSettingDto>(sqlSettings);

            var sqlTypeFields = new Sql();
            sqlSettings.Select("*")
                .From<TypeFieldDto>(_sqlSyntax);

            result.TypeFields = _database.Fetch<TypeFieldDto>(sqlTypeFields);
        }

        private void ValidateDbConstraints(MerchelloDatabaseSchemaResult result)
        {
            //MySql doesn't conform to the "normal" naming of constraints, so there is currently no point in doing these checks.
            //TODO: At a later point we do other checks for MySql, but ideally it should be necessary to do special checks for different providers.
            // ALso note that to get the constraints for MySql we have to open a connection which we currently have not.
            if (_sqlSyntax is MySqlSyntaxProvider)
                return;

            //Check constraints in configured database against constraints in schema
            var constraintsInDatabase = _sqlSyntax.GetConstraintsPerColumn(_database).DistinctBy(x => x.Item3).ToList();
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

            //Add valid and invalid foreign key differences to the result object
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

            //Foreign keys:

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


            //Primary keys:

            //Add valid and invalid primary key differences to the result object
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

            //Constaints:

            //NOTE: SD: The colIndex checks above should really take care of this but I need to keep this here because it was here before
            // and some schema validation checks might rely on this data remaining here!
            //Add valid and invalid index differences to the result object
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

        private void ValidateDbColumns(MerchelloDatabaseSchemaResult result)
        {
            //Check columns in configured database against columns in schema
            var columnsInDatabase = SqlSyntaxContext.SqlSyntaxProvider.GetColumnsInSchema(_database);
            var columnsPerTableInDatabase = columnsInDatabase.Select(x => string.Concat(x.TableName, ",", x.ColumnName)).ToList();
            var columnsPerTableInSchema = result.TableDefinitions.SelectMany(x => x.Columns.Select(y => string.Concat(y.TableName, ",", y.Name))).ToList();
            //Add valid and invalid column differences to the result object
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

        private void ValidateDbTables(MerchelloDatabaseSchemaResult result)
        {
            //Check tables in configured database against tables in schema
            var tablesInDatabase = SqlSyntaxContext.SqlSyntaxProvider.GetTablesInSchema(_database).ToList();
            var tablesInSchema = result.TableDefinitions.Select(x => x.Name).ToList();
            //Add valid and invalid table differences to the result object
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

        private void ValidateDbIndexes(MerchelloDatabaseSchemaResult result)
        {
            //These are just column indexes NOT constraints or Keys
            //var colIndexesInDatabase = result.DbIndexDefinitions.Where(x => x.IndexName.InvariantStartsWith("IX_")).Select(x => x.IndexName).ToList();
            var colIndexesInDatabase = result.DbIndexDefinitions.Select(x => x.IndexName).ToList();
            var indexesInSchema = result.TableDefinitions.SelectMany(x => x.Indexes.Select(y => y.Name)).ToList();

            //Add valid and invalid index differences to the result object
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

        #region Events

        /// <summary>
        /// The save event handler
        /// </summary>
        internal delegate void DatabaseEventHandler(DatabaseCreationEventArgs e);

        /// <summary>
        /// Occurs when [before save].
        /// </summary>
        internal static event DatabaseEventHandler BeforeCreation;
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
        /// Occurs when [after save].
        /// </summary>
        internal static event DatabaseEventHandler AfterCreation;
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

        #endregion
    }

}
