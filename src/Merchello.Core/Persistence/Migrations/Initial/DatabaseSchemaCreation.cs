using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Events;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.DatabaseModelDefinitions;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;



namespace Merchello.Core.Persistence.Migrations.Initial
{
    // TODO Generate SQL SCRIPT and follow the order of table creation

    /// <summary>
    /// Represents the initial database schema creation by running CreateTable for all DTOs against the db.
    /// </summary>
    internal class DatabaseSchemaCreation
    {
        #region Private Members

        private readonly Database _database;

        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {

                {0, typeof(TypeFieldDto)},
                {1, typeof(AnonymousCustomerDto)},
                {2, typeof(CustomerDto)},
                {3, typeof(CustomerAddressDto)},             
                {4, typeof(ItemCacheDto)},
                {5, typeof(ItemCacheItemDto)},
                {6, typeof(GatewayProviderDto)},

                {7, typeof(WarehouseDto)},
                {8, typeof(WarehouseCatalogDto)},
                
                //{8, typeof(WarehouseCountryDto)},
                {9, typeof(ShipCountryDto)},
                {10, typeof(ShipMethodDto)},
               // {11, typeof(ShipCountry2GatewayProviderDto)},
                
                {12, typeof(ShipRateTierDto)},
                
                {13, typeof(InvoiceStatusDto)},  
                {14, typeof(InvoiceDto)},                   
                {15, typeof(InvoiceItemDto)},

                {16, typeof(OrderStatusDto)},
                {17, typeof(OrderDto)}, 
                              
               
                {18, typeof(ShipmentDto)},                 
                {19, typeof(OrderItemDto)},
    
                {20, typeof(PaymentDto)},                
                {21, typeof(ProductDto)},
                {22, typeof(ProductVariantDto)},
                {23, typeof(ProductOptionDto)},
                {24, typeof(ProductAttributeDto)},
                {25, typeof(Product2ProductOptionDto)},
                //{26, typeof(WarehouseInventoryDto)},
                {26, typeof(CatalogInventoryDto)},
                {27, typeof(CountryTaxRateDto)},
                {28, typeof(ProductVariant2ProductAttributeDto)},
           
                {29, typeof(AppliedPaymentDto)},
                {30, typeof(ProductVariantIndexDto)},
                {31, typeof(StoreSettingDto)}
            };

        #endregion

        /// <summary>
        /// Drops all Merchello tables in the db
        /// </summary>
        internal void UninstallDatabaseSchema()
        {
            LogHelper.Info<DatabaseSchemaCreation>("Start UninstallDataSchema");

            foreach (var item in OrderedTables.OrderByDescending(x => x.Key))
            {
                var tableNameAttribute = item.Value.FirstAttribute<TableNameAttribute>();

                string tableName = tableNameAttribute == null ? item.Value.Name : tableNameAttribute.Value;

                LogHelper.Info<DatabaseSchemaCreation>("Uninstall" + tableName);

                try
                {
                    _database.DropTable(tableName);
                }
                catch (Exception ex)
                {
                    //swallow this for now, not sure how best to handle this with diff databases... though this is internal
                    // and only used for unit tests. If this fails its because the table doesn't exist... generally!
                    LogHelper.Error<DatabaseSchemaCreation>("Could not drop table " + tableName, ex);
                }
            }
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
                foreach (var item in OrderedTables.OrderBy(x => x.Key))
                {
                    _database.CreateTable(false, item.Value);
                }
            }

            FireAfterCreation(e);
        }

        /// <summary>
        /// Validates the schema of the current database
        /// </summary>
        public PluginDatabaseSchemaResult ValidateSchema()
        {
            var result = new PluginDatabaseSchemaResult();

            foreach (var item in OrderedTables.OrderBy(x => x.Key))
            {
                var tableDefinition = DefinitionFactory.GetTableDefinition(item.Value);
                result.TableDefinitions.Add(tableDefinition);
            }

            //Check tables in configured database against tables in schema
            var tablesInDatabase = SqlSyntaxContext.SqlSyntaxProvider.GetTablesInSchema(_database).ToList();
            var tablesInSchema = result.TableDefinitions.Select(x => x.Name).ToList();
            //Add valid and invalid table differences to the result object
            var validTableDifferences = tablesInDatabase.Intersect(tablesInSchema);
            foreach (var tableName in validTableDifferences)
            {
                result.ValidTables.Add(tableName);
            }
            var invalidTableDifferences =
                tablesInDatabase.Except(tablesInSchema)
                                .Union(tablesInSchema.Except(tablesInDatabase));
            foreach (var tableName in invalidTableDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Table", tableName));
            }

            //Check columns in configured database against columns in schema
            var columnsInDatabase = SqlSyntaxContext.SqlSyntaxProvider.GetColumnsInSchema(_database);
            var columnsPerTableInDatabase = columnsInDatabase.Select(x => string.Concat(x.TableName, ",", x.ColumnName)).ToList();
            var columnsPerTableInSchema = result.TableDefinitions.SelectMany(x => x.Columns.Select(y => string.Concat(y.TableName, ",", y.Name))).ToList();
            //Add valid and invalid column differences to the result object
            var validColumnDifferences = columnsPerTableInDatabase.Intersect(columnsPerTableInSchema);
            foreach (var column in validColumnDifferences)
            {
                result.ValidColumns.Add(column);
            }
            var invalidColumnDifferences = columnsPerTableInDatabase.Except(columnsPerTableInSchema);
            foreach (var column in invalidColumnDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Column", column));
            }

            //MySql doesn't conform to the "normal" naming of constraints, so there is currently no point in doing these checks.
            //NOTE: At a later point we do other checks for MySql, but ideally it should be necessary to do special checks for different providers.
            if (SqlSyntaxContext.SqlSyntaxProvider is MySqlSyntaxProvider)
                return result;

            //Check constraints in configured database against constraints in schema
            var constraintsInDatabase = SqlSyntaxContext.SqlSyntaxProvider.GetConstraintsPerColumn(_database).DistinctBy(x => x.Item3).ToList();
            var foreignKeysInDatabase = constraintsInDatabase.Where(x => x.Item3.StartsWith("FK_")).Select(x => x.Item3).ToList();
            var primaryKeysInDatabase = constraintsInDatabase.Where(x => x.Item3.StartsWith("PK_")).Select(x => x.Item3).ToList();
            var indexesInDatabase = constraintsInDatabase.Where(x => x.Item3.StartsWith("IX_")).Select(x => x.Item3).ToList();
            var unknownConstraintsInDatabase =
                constraintsInDatabase.Where(
                    x =>
                    x.Item3.StartsWith("FK_") == false && x.Item3.StartsWith("PK_") == false &&
                    x.Item3.StartsWith("IX_") == false).Select(x => x.Item3).ToList();
            var foreignKeysInSchema = result.TableDefinitions.SelectMany(x => x.ForeignKeys.Select(y => y.Name)).ToList();
            var primaryKeysInSchema = result.TableDefinitions.SelectMany(x => x.Columns.Select(y => y.PrimaryKeyName)).ToList();
            var indexesInSchema = result.TableDefinitions.SelectMany(x => x.Indexes.Select(y => y.Name)).ToList();
            //Add valid and invalid foreign key differences to the result object
            foreach (var unknown in unknownConstraintsInDatabase)
            {
                if (foreignKeysInSchema.Contains(unknown) || primaryKeysInSchema.Contains(unknown) || indexesInSchema.Contains(unknown))
                {
                    result.ValidConstraints.Add(unknown);
                }
                else
                {
                    result.Errors.Add(new Tuple<string, string>("Unknown", unknown));
                }
            }
            var validForeignKeyDifferences = foreignKeysInDatabase.Intersect(foreignKeysInSchema);
            foreach (var foreignKey in validForeignKeyDifferences)
            {
                result.ValidConstraints.Add(foreignKey);
            }
            var invalidForeignKeyDifferences = foreignKeysInDatabase.Except(foreignKeysInSchema);
            foreach (var foreignKey in invalidForeignKeyDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Constraint", foreignKey));
            }
            //Add valid and invalid primary key differences to the result object
            var validPrimaryKeyDifferences = primaryKeysInDatabase.Intersect(primaryKeysInSchema);
            foreach (var primaryKey in validPrimaryKeyDifferences)
            {
                result.ValidConstraints.Add(primaryKey);
            }
            var invalidPrimaryKeyDifferences = primaryKeysInDatabase.Except(primaryKeysInSchema);
            foreach (var primaryKey in invalidPrimaryKeyDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Constraint", primaryKey));
            }
            //Add valid and invalid index differences to the result object
            var validIndexDifferences = indexesInDatabase.Intersect(indexesInSchema);
            foreach (var index in validIndexDifferences)
            {
                result.ValidConstraints.Add(index);
            }
            var invalidIndexDifferences = indexesInDatabase.Except(indexesInSchema);
            foreach (var index in invalidIndexDifferences)
            {
                result.Errors.Add(new Tuple<string, string>("Constraint", index));
            }

            return result;
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
