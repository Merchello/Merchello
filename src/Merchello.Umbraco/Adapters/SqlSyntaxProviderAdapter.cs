namespace Merchello.Umbraco.Adapters
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;
    using Merchello.Core.Acquired.Persistence.DatabaseModelDefinitions;
    using Merchello.Core.Acquired.Persistence.Querying;
    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    /// <summary>
    /// An adapter for adapting Umbraco's <see>
    ///         <cref>global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider</cref>
    ///     </see> to <see cref="ISqlSyntaxProvider"/>.
    /// </summary>
    internal class SqlSyntaxProviderAdapter : ISqlSyntaxProvider
    {
        /// <summary>
        /// The Umbraco's SqlSyntaxProvider.
        /// </summary>
        private readonly global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSyntaxProviderAdapter"/> class.
        /// </summary>
        /// <param name="provider">
        /// Umbraco's SqlSyntaxProvider..
        /// </param>
        public SqlSyntaxProviderAdapter(global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider provider)
        {
            Ensure.ParameterNotNull(provider, nameof(provider));
            _provider = provider;
        }

        /// <summary>
        /// Gets the create table string.
        /// </summary>
        public string CreateTable
        {
            get
            {
                return _provider.CreateTable;
            }
        }

        /// <summary>
        /// Gets the drop table string.
        /// </summary>
        public string DropTable
        {
            get
            {
                return _provider.DropTable;
            }
        }

        /// <summary>
        /// Gets the add column string.
        /// </summary>
        public string AddColumn
        {
            get
            {
                return _provider.AddColumn;
            }
        }

        /// <summary>
        /// Gets the drop column string.
        /// </summary>
        public string DropColumn
        {
            get
            {
                return _provider.DropColumn;
            }
        }

        /// <summary>
        /// Gets the alter column string.
        /// </summary>
        public string AlterColumn
        {
            get
            {
                return _provider.AlterColumn;
            }
        }

        /// <summary>
        /// Gets the rename column string.
        /// </summary>
        public string RenameColumn
        {
            get
            {
                return _provider.RenameColumn;
            }
        }

        /// <summary>
        /// Gets the rename table.
        /// </summary>
        public string RenameTable
        {
            get
            {
                return _provider.RenameTable;
            }
        }

        /// <summary>
        /// Gets the create schema string.
        /// </summary>
        public string CreateSchema
        {
            get
            {
                return _provider.CreateSchema;
            }
        }

        /// <summary>
        /// Gets the alter schema string.
        /// </summary>
        public string AlterSchema
        {
            get
            {
                return _provider.AlterSchema;
            }
        }

        /// <summary>
        /// Gets the drop schema string.
        /// </summary>
        public string DropSchema
        {
            get
            {
                return _provider.DropSchema;
            }
        }

        /// <summary>
        /// Gets the create index string.
        /// </summary>
        public string CreateIndex
        {
            get
            {
                return _provider.CreateIndex;
            }
        }

        /// <summary>
        /// Gets the drop index string.
        /// </summary>
        public string DropIndex
        {
            get
            {
                return _provider.DropIndex;
            }
        }

        /// <summary>
        /// Gets the insert data string.
        /// </summary>
        public string InsertData
        {
            get
            {
                return _provider.InsertData;
            }
        }

        /// <summary>
        /// Gets the update data string.
        /// </summary>
        public string UpdateData
        {
            get
            {
                return _provider.UpdateData;
            }
        }

        /// <summary>
        /// Gets the delete data string.
        /// </summary>
        public string DeleteData
        {
            get
            {
                return _provider.DeleteData;
            }
        }

        /// <summary>
        /// Gets the truncate table string.
        /// </summary>
        public string TruncateTable
        {
            get
            {
                return _provider.TruncateTable;
            }
        }

        /// <summary>
        /// Gets the create constraint string.
        /// </summary>
        public string CreateConstraint
        {
            get
            {
                return _provider.CreateConstraint;
            }
        }

        /// <summary>
        /// Gets the delete constraint string.
        /// </summary>
        public string DeleteConstraint
        {
            get
            {
                return _provider.DeleteConstraint;
            }
        }

        /// <summary>
        /// Gets the create foreign key constraint.
        /// </summary>
        public string CreateForeignKeyConstraint
        {
            get
            {
                return _provider.CreateForeignKeyConstraint;
            }
        }

        /// <summary>
        /// Gets the delete default constraint string.
        /// </summary>
        public string DeleteDefaultConstraint
        {
            get
            {
                return _provider.DeleteDefaultConstraint;
            }
        }

        /// <summary>
        /// Escapes a string value.
        /// </summary>
        /// <param name="val">
        /// The value.
        /// </param>
        /// <returns>
        /// The escaped <see cref="string"/>.
        /// </returns>
        public string EscapeString(string val)
        {
            return _provider.EscapeString(val);
        }

        /// <summary>
        /// Gets a wildcard placeholder.
        /// </summary>
        /// <returns>
        /// The placeholder.
        /// </returns>
        public string GetWildcardPlaceholder()
        {
            return _provider.GetWildcardPlaceholder();
        }

        /// <summary>
        /// Gets a string equals comparison.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="paramIndex">
        /// The parameter index.
        /// </param>
        /// <param name="columnType">
        /// The column type.
        /// </param>
        /// <returns>
        /// The string equals comparison.
        /// </returns>
        public string GetStringColumnEqualComparison(string column, int paramIndex, TextColumnType columnType)
        {
            return _provider.GetStringColumnEqualComparison(column, paramIndex, Converter.Convert(columnType));
        }

        /// <summary>
        /// Gets the string column wildcard comparison.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="paramIndex">
        /// The parameter index.
        /// </param>
        /// <param name="columnType">
        /// The column type.
        /// </param>
        /// <returns>
        /// The string column wildcard comparison..
        /// </returns>
        public string GetStringColumnWildcardComparison(string column, int paramIndex, TextColumnType columnType)
        {
            return _provider.GetStringColumnWildcardComparison(column, paramIndex, Converter.Convert(columnType));
        }

        /// <summary>
        /// Concatenates strings.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// REFACTOR- this is new to Umbraco V8
        public string GetConcat(params string[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the quoted table name.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The quoted table name.
        /// </returns>
        public string GetQuotedTableName(string tableName)
        {
            return _provider.GetQuotedTableName(tableName);
        }

        /// <summary>
        /// Get the quoted column name.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The quoted column name.
        /// </returns>
        public string GetQuotedColumnName(string columnName)
        {
            return _provider.GetQuotedColumnName(columnName);
        }

        /// <summary>
        /// Gets a quoted name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The quoted name.
        /// </returns>]
        public string GetQuotedName(string name)
        {
            return _provider.GetQuotedName(name);
        }

        /// <summary>
        /// Returns a value indicating if a table exists.
        /// </summary>
        /// <param name="db">
        /// The database.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// A value indicating whether the table exists in the database.
        /// </returns>
        /// REFACTOR Umbraco V8 changes to NPoco
        public bool DoesTableExist(Database db, string tableName)
        {
            throw new NotImplementedException();
            //// return _provider.DoesTableExist(db, tableName);
        }

        /// <summary>
        /// The get index type.
        /// </summary>
        /// <param name="indexTypes">
        /// The index types.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetIndexType(IndexTypes indexTypes)
        {
            return _provider.GetIndexType(Converter.Convert(indexTypes));
        }

        /// <summary>
        /// Gets the special db types.
        /// </summary>
        /// <param name="dbTypes">
        /// The db types.
        /// </param>
        /// <returns>
        /// The special db type.
        /// </returns>
        public string GetSpecialDbType(SpecialDbTypes dbTypes)
        {
            return _provider.GetSpecialDbType(Converter.Convert(dbTypes));
        }

        /// <summary>
        /// Formats date time.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="includeTime">
        /// The include time.
        /// </param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public string FormatDateTime(DateTime date, bool includeTime = true)
        {
            return _provider.FormatDateTime(date, includeTime);
        }

        public string Format(TableDefinition table)
        {
            //return _provider.Format(table)
            throw new NotImplementedException();
        }

        public string Format(IEnumerable<ColumnDefinition> columns)
        {
            throw new NotImplementedException();
        }

        public List<string> Format(IEnumerable<IndexDefinition> indexes)
        {
            throw new NotImplementedException();
        }

        public List<string> Format(IEnumerable<ForeignKeyDefinition> foreignKeys)
        {
            throw new NotImplementedException();
        }

        public string FormatPrimaryKey(TableDefinition table)
        {
            throw new NotImplementedException();
        }

        public string GetQuotedValue(string value)
        {
            throw new NotImplementedException();
        }

        public string Format(ColumnDefinition column)
        {
            throw new NotImplementedException();
        }

        public string Format(IndexDefinition index)
        {
            throw new NotImplementedException();
        }

        public string Format(ForeignKeyDefinition foreignKey)
        {
            throw new NotImplementedException();
        }

        public string FormatColumnRename(string tableName, string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public string FormatTableRename(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public bool SupportsClustered()
        {
            throw new NotImplementedException();
        }

        public bool SupportsIdentityInsert()
        {
            throw new NotImplementedException();
        }

        public bool? SupportsCaseInsensitiveQueries(Database db)
        {
            throw new NotImplementedException();
        }

        public string ConvertIntegerToOrderableString { get; }

        public string ConvertDateToOrderableString { get; }

        public string ConvertDecimalToOrderableString { get; }

        public IEnumerable<string> GetTablesInSchema(Database db)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ColumnInfo> GetColumnsInSchema(Database db)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<string, string>> GetConstraintsPerTable(Database db)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<string, string, string>> GetConstraintsPerColumn(Database db)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<string, string, string, bool>> GetDefinedIndexes(Database db)
        {
            throw new NotImplementedException();
        }
    }
}