namespace Merchello.Umbraco.Adapters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

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

        /// <inheritdoc/>
        public string ConvertIntegerToOrderableString
        {
            get
            {
                return _provider.ConvertIntegerToOrderableString;
            }
        }

        /// <inheritdoc/>
        public string ConvertDateToOrderableString
        {
            get
            {
                return _provider.ConvertDateToOrderableString;
            }
        }

        /// <inheritdoc/>
        public string ConvertDecimalToOrderableString
        {
            get
            {
                return _provider.ConvertDecimalToOrderableString;
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
        public string GetConcat(params string[] args)
        {
            return _provider.GetConcat(args);
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
        public bool DoesTableExist(Database db, string tableName)
        {
            return _provider.DoesTableExist(db, tableName);
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
            return _provider.Format(Converter.Convert(table));
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
            return _provider.Format(cdefs);
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
            return _provider.Format(idxdefs);
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
            return _provider.Format(fkdefs);
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
            return _provider.FormatPrimaryKey(Converter.Convert(table));
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
            return _provider.GetQuotedValue(value);
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
            return _provider.Format(Converter.Convert(column));
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
            return _provider.Format(Converter.Convert(index));
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
            return _provider.Format(Converter.Convert(foreignKey));
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
            return _provider.FormatColumnRename(tableName, oldName, newName);
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
            return _provider.FormatTableRename(oldName, newName);
        }

        /// <summary>
        /// Returns a value indicating whether the database supports clustered indexes.
        /// </summary>
        /// <returns>
        /// A value indicating whether the database supports clustered indexes.
        /// </returns>
        public bool SupportsClustered()
        {
            return _provider.SupportsClustered();
        }

        /// <summary>
        /// Returns a value indicating whether the database supports identity insert.
        /// </summary>
        /// <returns>
        /// A value indicating whether the database supports identity insert.
        /// </returns>
        public bool SupportsIdentityInsert()
        {
            return _provider.SupportsIdentityInsert();
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
            return _provider.SupportsCaseInsensitiveQueries(db);
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
            return _provider.GetTablesInSchema(db);
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
            return _provider.GetColumnsInSchema(db).Select(Converter.Convert);
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
            return _provider.GetConstraintsPerTable(db);
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
            return _provider.GetConstraintsPerColumn(db);
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
            return _provider.GetDefinedIndexes(db);
        }
    }
}