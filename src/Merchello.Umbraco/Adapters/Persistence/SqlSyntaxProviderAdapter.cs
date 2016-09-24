namespace Merchello.Umbraco.Adapters.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;
    using Merchello.Core.Acquired.Persistence.DatabaseModelDefinitions;
    using Merchello.Core.Acquired.Persistence.Querying;
    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    using ColumnInfo = Merchello.Core.Persistence.SqlSyntax.ColumnInfo;

    /// <summary>
    /// An adapter for adapting Umbraco's <see>
    ///         <cref>global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider</cref>
    ///     </see> to <see cref="ISqlSyntaxProvider"/>.
    /// </summary>
    internal class SqlSyntaxProviderAdapter : ISqlSyntaxProvider, IUmbracoAdapter
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
            this._provider = provider;
        }

        /// <summary>
        /// Gets the create table string.
        /// </summary>
        public string CreateTable
        {
            get
            {
                return this._provider.CreateTable;
            }
        }

        /// <summary>
        /// Gets the drop table string.
        /// </summary>
        public string DropTable
        {
            get
            {
                return this._provider.DropTable;
            }
        }

        /// <summary>
        /// Gets the add column string.
        /// </summary>
        public string AddColumn
        {
            get
            {
                return this._provider.AddColumn;
            }
        }

        /// <summary>
        /// Gets the drop column string.
        /// </summary>
        public string DropColumn
        {
            get
            {
                return this._provider.DropColumn;
            }
        }

        /// <summary>
        /// Gets the alter column string.
        /// </summary>
        public string AlterColumn
        {
            get
            {
                return this._provider.AlterColumn;
            }
        }

        /// <summary>
        /// Gets the rename column string.
        /// </summary>
        public string RenameColumn
        {
            get
            {
                return this._provider.RenameColumn;
            }
        }

        /// <summary>
        /// Gets the rename table.
        /// </summary>
        public string RenameTable
        {
            get
            {
                return this._provider.RenameTable;
            }
        }

        /// <summary>
        /// Gets the create schema string.
        /// </summary>
        public string CreateSchema
        {
            get
            {
                return this._provider.CreateSchema;
            }
        }

        /// <summary>
        /// Gets the alter schema string.
        /// </summary>
        public string AlterSchema
        {
            get
            {
                return this._provider.AlterSchema;
            }
        }

        /// <summary>
        /// Gets the drop schema string.
        /// </summary>
        public string DropSchema
        {
            get
            {
                return this._provider.DropSchema;
            }
        }

        /// <summary>
        /// Gets the create index string.
        /// </summary>
        public string CreateIndex
        {
            get
            {
                return this._provider.CreateIndex;
            }
        }

        /// <summary>
        /// Gets the drop index string.
        /// </summary>
        public string DropIndex
        {
            get
            {
                return this._provider.DropIndex;
            }
        }

        /// <summary>
        /// Gets the insert data string.
        /// </summary>
        public string InsertData
        {
            get
            {
                return this._provider.InsertData;
            }
        }

        /// <summary>
        /// Gets the update data string.
        /// </summary>
        public string UpdateData
        {
            get
            {
                return this._provider.UpdateData;
            }
        }

        /// <summary>
        /// Gets the delete data string.
        /// </summary>
        public string DeleteData
        {
            get
            {
                return this._provider.DeleteData;
            }
        }

        /// <summary>
        /// Gets the truncate table string.
        /// </summary>
        public string TruncateTable
        {
            get
            {
                return this._provider.TruncateTable;
            }
        }

        /// <summary>
        /// Gets the create constraint string.
        /// </summary>
        public string CreateConstraint
        {
            get
            {
                return this._provider.CreateConstraint;
            }
        }

        /// <summary>
        /// Gets the delete constraint string.
        /// </summary>
        public string DeleteConstraint
        {
            get
            {
                return this._provider.DeleteConstraint;
            }
        }

        /// <summary>
        /// Gets the create foreign key constraint.
        /// </summary>
        public string CreateForeignKeyConstraint
        {
            get
            {
                return this._provider.CreateForeignKeyConstraint;
            }
        }

        /// <summary>
        /// Gets the delete default constraint string.
        /// </summary>
        public string DeleteDefaultConstraint
        {
            get
            {
                return this._provider.DeleteDefaultConstraint;
            }
        }

        /// <inheritdoc/>
        public string ConvertIntegerToOrderableString
        {
            get
            {
                return this._provider.ConvertIntegerToOrderableString;
            }
        }

        /// <inheritdoc/>
        public string ConvertDateToOrderableString
        {
            get
            {
                return this._provider.ConvertDateToOrderableString;
            }
        }

        /// <inheritdoc/>
        public string ConvertDecimalToOrderableString
        {
            get
            {
                return this._provider.ConvertDecimalToOrderableString;
            }
        }

        string ISqlSyntaxProvider.CreateTable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.DropTable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.AddColumn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.DropColumn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.AlterColumn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.RenameColumn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.RenameTable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.CreateSchema
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.AlterSchema
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.DropSchema
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.CreateIndex
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.DropIndex
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.InsertData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.UpdateData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.DeleteData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.TruncateTable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.CreateConstraint
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.DeleteConstraint
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.CreateForeignKeyConstraint
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.DeleteDefaultConstraint
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.ConvertIntegerToOrderableString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.ConvertDateToOrderableString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ISqlSyntaxProvider.ConvertDecimalToOrderableString
        {
            get
            {
                throw new NotImplementedException();
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
            return this._provider.EscapeString(val);
        }

        /// <summary>
        /// Gets a wildcard placeholder.
        /// </summary>
        /// <returns>
        /// The placeholder.
        /// </returns>
        public string GetWildcardPlaceholder()
        {
            return this._provider.GetWildcardPlaceholder();
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
            return this._provider.GetStringColumnEqualComparison(column, paramIndex, Converter.Convert(columnType));
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
            return this._provider.GetStringColumnWildcardComparison(column, paramIndex, Converter.Convert(columnType));
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
        public string GetConcat(params string[] args)
        {
            return this._provider.GetConcat(args);
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
            return this._provider.GetQuotedTableName(tableName);
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
            return this._provider.GetQuotedColumnName(columnName);
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
            return this._provider.GetQuotedName(name);
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
        public bool DoesTableExist(Database db, string tableName)
        {
            return this._provider.DoesTableExist(db, tableName);
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
            return this._provider.GetIndexType(Converter.Convert(indexTypes));
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
            return this._provider.GetSpecialDbType(Converter.Convert(dbTypes));
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
            return this._provider.FormatDateTime(date, includeTime);
        }

        /// <summary>
        /// Formats a table definition.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public string Format(TableDefinition table)
        {
            return this._provider.Format(Converter.Convert(table));
        }

        /// <summary>
        /// Formats column definitions.
        /// </summary>
        /// <param name="columns">
        /// The columns.
        /// </param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public string Format(IEnumerable<ColumnDefinition> columns)
        {
            var cdefs = columns.Select(Converter.Convert);
            return this._provider.Format(cdefs);
        }

        /// <summary>
        /// Formats index definitions.
        /// </summary>
        /// <param name="indexes">
        /// The indexes.
        /// </param>
        /// <returns>
        /// A list of formatted index definitions.
        /// </returns>
        public List<string> Format(IEnumerable<IndexDefinition> indexes)
        {
            var idxdefs = indexes.Select(Converter.Convert);
            return this._provider.Format(idxdefs);
        }

        /// <summary>
        /// Formats foreign keys.
        /// </summary>
        /// <param name="foreignKeys">
        /// The foreign keys.
        /// </param>
        /// <returns>
        /// The formatted list of foreign keys.
        /// </returns>
        public List<string> Format(IEnumerable<ForeignKeyDefinition> foreignKeys)
        {
            var fkdefs = foreignKeys.Select(Converter.Convert);
            return this._provider.Format(fkdefs);
        }

        /// <summary>
        /// Formats a primary key.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <returns>
        /// The formatted primary key.
        /// </returns>
        public string FormatPrimaryKey(TableDefinition table)
        {
            return this._provider.FormatPrimaryKey(Converter.Convert(table));
        }

        /// <summary>
        /// Gets a quoted value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The quoted value.
        /// </returns>
        public string GetQuotedValue(string value)
        {
            return this._provider.GetQuotedValue(value);
        }

        /// <summary>
        /// Formats a column definition.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The formatted column definition.
        /// </returns>
        public string Format(ColumnDefinition column)
        {
            return this._provider.Format(Converter.Convert(column));
        }

        /// <summary>
        /// Formats an index definition.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The formatted index definition.
        /// </returns>
        public string Format(IndexDefinition index)
        {
            return this._provider.Format(Converter.Convert(index));
        }

        /// <summary>
        /// Formats a foreign key definition.
        /// </summary>
        /// <param name="foreignKey">
        /// The foreign key.
        /// </param>
        /// <returns>
        /// The formatted foreign key definition.
        /// </returns>
        public string Format(ForeignKeyDefinition foreignKey)
        {
            return this._provider.Format(Converter.Convert(foreignKey));
        }

        /// <summary>
        /// Formats a column rename string.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="oldName">
        /// The old name.
        /// </param>
        /// <param name="newName">
        /// The new name.
        /// </param>
        /// <returns>
        /// The formatted column rename string.
        /// </returns>
        public string FormatColumnRename(string tableName, string oldName, string newName)
        {
            return this._provider.FormatColumnRename(tableName, oldName, newName);
        }

        /// <summary>
        /// Formats a table rename string.
        /// </summary>
        /// <param name="oldName">
        /// The old name.
        /// </param>
        /// <param name="newName">
        /// The new name.
        /// </param>
        /// <returns>
        /// The formatted table rename string.
        /// </returns>
        public string FormatTableRename(string oldName, string newName)
        {
            return this._provider.FormatTableRename(oldName, newName);
        }

        /// <summary>
        /// Returns a value indicating whether the database supports clustered indexes.
        /// </summary>
        /// <returns>
        /// A value indicating whether the database supports clustered indexes.
        /// </returns>
        public bool SupportsClustered()
        {
            return this._provider.SupportsClustered();
        }

        /// <summary>
        /// Returns a value indicating whether the database supports identity insert.
        /// </summary>
        /// <returns>
        /// A value indicating whether the database supports identity insert.
        /// </returns>
        public bool SupportsIdentityInsert()
        {
            return this._provider.SupportsIdentityInsert();
        }

        /// <summary>
        /// Returns a value indication whether the database supports case insensitive queries.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// A a value indication whether the database supports case insensitive queries or null.
        /// </returns>
        public bool? SupportsCaseInsensitiveQueries(Database db)
        {
            return this._provider.SupportsCaseInsensitiveQueries(db);
        }

        /// <summary>
        /// Gets the tables in the database schema.
        /// </summary>
        /// <param name="db">
        /// The database.
        /// </param>
        /// <returns>
        /// The collection of tables.
        /// </returns>
        public IEnumerable<string> GetTablesInSchema(Database db)
        {
            return this._provider.GetTablesInSchema(db);
        }

        /// <summary>
        /// Gets the columns in the database schema.
        /// </summary>
        /// <param name="db">
        /// The database.
        /// </param>
        /// <returns>
        /// The collection of <see cref="ColumnInfo"/> in the database schema.
        /// </returns>
        public IEnumerable<ColumnInfo> GetColumnsInSchema(Database db)
        {
            return this._provider.GetColumnsInSchema(db).Select(Converter.Convert);
        }

        /// <summary>
        /// Gets a collection of constraints for a table.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The collection of table constraints.
        /// </returns>
        public IEnumerable<Tuple<string, string>> GetConstraintsPerTable(Database db)
        {
            return this._provider.GetConstraintsPerTable(db);
        }

        /// <summary>
        /// Gets a collection of constraints per column.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The collection of constraints per column.
        /// </returns>
        public IEnumerable<Tuple<string, string, string>> GetConstraintsPerColumn(Database db)
        {
            return this._provider.GetConstraintsPerColumn(db);
        }

        /// <summary>
        /// Gets a collection of defined indexes.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The collection of defined indexes.
        /// </returns>
        public IEnumerable<Tuple<string, string, string, bool>> GetDefinedIndexes(Database db)
        {
            return this._provider.GetDefinedIndexes(db);
        }

        string ISqlSyntaxProvider.EscapeString(string val)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetWildcardPlaceholder()
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetStringColumnEqualComparison(string column, int paramIndex, TextColumnType columnType)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetStringColumnWildcardComparison(string column, int paramIndex, TextColumnType columnType)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetConcat(params string[] args)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetQuotedTableName(string tableName)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetQuotedColumnName(string columnName)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetQuotedName(string name)
        {
            throw new NotImplementedException();
        }

        bool ISqlSyntaxProvider.DoesTableExist(Database db, string tableName)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetIndexType(IndexTypes indexTypes)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetSpecialDbType(SpecialDbTypes dbTypes)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.FormatDateTime(DateTime date, bool includeTime)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.Format(TableDefinition table)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.Format(IEnumerable<ColumnDefinition> columns)
        {
            throw new NotImplementedException();
        }

        List<string> ISqlSyntaxProvider.Format(IEnumerable<IndexDefinition> indexes)
        {
            throw new NotImplementedException();
        }

        List<string> ISqlSyntaxProvider.Format(IEnumerable<ForeignKeyDefinition> foreignKeys)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.FormatPrimaryKey(TableDefinition table)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.GetQuotedValue(string value)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.Format(ColumnDefinition column)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.Format(IndexDefinition index)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.Format(ForeignKeyDefinition foreignKey)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.FormatColumnRename(string tableName, string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        string ISqlSyntaxProvider.FormatTableRename(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        bool ISqlSyntaxProvider.SupportsClustered()
        {
            throw new NotImplementedException();
        }

        bool ISqlSyntaxProvider.SupportsIdentityInsert()
        {
            throw new NotImplementedException();
        }

        bool? ISqlSyntaxProvider.SupportsCaseInsensitiveQueries(Database db)
        {
            throw new NotImplementedException();
        }

        IEnumerable<string> ISqlSyntaxProvider.GetTablesInSchema(Database db)
        {
            throw new NotImplementedException();
        }

        IEnumerable<ColumnInfo> ISqlSyntaxProvider.GetColumnsInSchema(Database db)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Tuple<string, string>> ISqlSyntaxProvider.GetConstraintsPerTable(Database db)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Tuple<string, string, string>> ISqlSyntaxProvider.GetConstraintsPerColumn(Database db)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Tuple<string, string, string, bool>> ISqlSyntaxProvider.GetDefinedIndexes(Database db)
        {
            throw new NotImplementedException();
        }
    }
}