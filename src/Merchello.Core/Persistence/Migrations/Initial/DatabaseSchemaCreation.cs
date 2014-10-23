namespace Merchello.Core.Persistence.Migrations.Initial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using DatabaseModelDefinitions;
    using Events;
    using Models.Rdbms;
    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;

    // TODO Generate SQL SCRIPT and follow the order of table creation

    /// <summary>
    /// Represents the initial database schema creation by running CreateTable for all DTOs against the db.
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
            { 1, typeof(AnonymousCustomerDto) },
            { 2, typeof(CustomerDto) },
            { 3, typeof(CustomerIndexDto) },             
            { 4, typeof(CustomerAddressDto) },
            { 5, typeof(ItemCacheDto) },
            { 6, typeof(ItemCacheItemDto) },
            { 7, typeof(GatewayProviderSettingsDto) },
            { 8, typeof(WarehouseDto) },
            { 9, typeof(WarehouseCatalogDto) },                
            { 10, typeof(ShipCountryDto) },
            { 11, typeof(ShipMethodDto) },
            { 12, typeof(ShipRateTierDto) },                
            { 13, typeof(InvoiceStatusDto) },  
            { 14, typeof(InvoiceDto) },                   
            { 15, typeof(InvoiceItemDto) },
            { 16, typeof(InvoiceIndexDto) },
            { 17, typeof(OrderStatusDto) },
            { 18, typeof(OrderDto) },    
            { 19, typeof(ShipmentStatusDto) },                              
            { 20, typeof(ShipmentDto) },                 
            { 21, typeof(OrderItemDto) },
            { 22, typeof(PaymentMethodDto) }, 
            { 23, typeof(PaymentDto) },                
            { 24, typeof(ProductDto) },
            { 25, typeof(ProductVariantDto) },
            { 26, typeof(ProductOptionDto) },
            { 27, typeof(ProductAttributeDto) },
            { 28, typeof(Product2ProductOptionDto) },
            { 29, typeof(CatalogInventoryDto) },
            { 30, typeof(TaxMethodDto) },
            { 31, typeof(ProductVariant2ProductAttributeDto) },           
            { 32, typeof(AppliedPaymentDto) },
            { 33, typeof(ProductVariantIndexDto) },
            { 34, typeof(StoreSettingDto) },
            { 35, typeof(OrderIndexDto) },
            { 36, typeof(NotificationMethodDto) },
            { 37, typeof(NotificationMessageDto) },
            { 38, typeof(AuditLogDto) }
        };

        /// <summary>
        /// The database.
        /// </summary>
        private readonly Database _database;        

        #endregion

        /// <summary>
        /// Drops all Merchello tables in the db
        /// </summary>
        internal void UninstallDatabaseSchema()
        {
            DatabaseSchemaHelper.UninstallDatabaseSchema(_database, OrderedTables, MerchelloVersion.Current.ToString());
        }

        public DatabaseSchemaCreation(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// Initialize the database by creating the umbraco db schema
        /// </summary>
        public void InitializeDatabaseSchema()
        {
            var e = new DatabaseCreationEventArgs();
            FireBeforeCreation(e);

            if (!e.Cancel)
            {
                DatabaseSchemaHelper.InitializeDatabaseSchema(_database, OrderedTables, MerchelloVersion.Current.ToString());
            }

            FireAfterCreation(e);
        }

        /// <summary>
        /// Validates the schema of the current database
        /// </summary>
        public PluginDatabaseSchemaResult ValidateSchema()
        {
            var result = new PluginDatabaseSchemaResult
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

            return result;
        }

        private void ValidateDbConstraints(PluginDatabaseSchemaResult result)
        {
            //MySql doesn't conform to the "normal" naming of constraints, so there is currently no point in doing these checks.
            //TODO: At a later point we do other checks for MySql, but ideally it should be necessary to do special checks for different providers.
            // ALso note that to get the constraints for MySql we have to open a connection which we currently have not.
            if (SqlSyntaxContext.SqlSyntaxProvider is MySqlSyntaxProvider)
                return;

            //Check constraints in configured database against constraints in schema
            var constraintsInDatabase = SqlSyntaxContext.SqlSyntaxProvider.GetConstraintsPerColumn(_database).DistinctBy(x => x.Item3).ToList();
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

        private void ValidateDbColumns(PluginDatabaseSchemaResult result)
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

        private void ValidateDbTables(PluginDatabaseSchemaResult result)
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

        private void ValidateDbIndexes(PluginDatabaseSchemaResult result)
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
