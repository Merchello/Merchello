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
            return _provider.GetStringColumnEqualComparison(column, paramIndex, Convert(columnType));
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
            return _provider.GetStringColumnWildcardComparison(column, paramIndex, Convert(columnType));
        }

        public string GetConcat(params string[] args)
        {
            throw new NotImplementedException();
        }

        public string GetQuotedTableName(string tableName)
        {
            throw new NotImplementedException();
        }

        public string GetQuotedColumnName(string columnName)
        {
            throw new NotImplementedException();
        }

        public string GetQuotedName(string name)
        {
            throw new NotImplementedException();
        }

        public bool DoesTableExist(Database db, string tableName)
        {
            throw new NotImplementedException();
        }

        public string GetIndexType(IndexTypes indexTypes)
        {
            throw new NotImplementedException();
        }

        public string GetSpecialDbType(SpecialDbTypes dbTypes)
        {
            throw new NotImplementedException();
        }

        public string CreateTable { get; }

        public string DropTable { get; }

        public string AddColumn { get; }

        public string DropColumn { get; }

        public string AlterColumn { get; }

        public string RenameColumn { get; }

        public string RenameTable { get; }

        public string CreateSchema { get; }

        public string AlterSchema { get; }

        public string DropSchema { get; }

        public string CreateIndex { get; }

        public string DropIndex { get; }

        public string InsertData { get; }

        public string UpdateData { get; }

        public string DeleteData { get; }

        public string TruncateTable { get; }

        public string CreateConstraint { get; }

        public string DeleteConstraint { get; }

        public string CreateForeignKeyConstraint { get; }

        public string DeleteDefaultConstraint { get; }

        public string FormatDateTime(DateTime date, bool includeTime = true)
        {
            throw new NotImplementedException();
        }

        public string Format(TableDefinition table)
        {
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

        private global::Umbraco.Core.Persistence.Querying.TextColumnType Convert(TextColumnType textColumType)
        {
            return textColumType == TextColumnType.NText
                       ? global::Umbraco.Core.Persistence.Querying.TextColumnType.NText
                       : global::Umbraco.Core.Persistence.Querying.TextColumnType.NVarchar;
        }
    }
}