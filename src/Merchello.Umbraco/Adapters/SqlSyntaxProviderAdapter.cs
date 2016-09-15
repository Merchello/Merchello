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

    internal class SqlSyntaxProviderAdapter : ISqlSyntaxProvider
    {
        private readonly global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider _provider;

        public SqlSyntaxProviderAdapter(global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider provider)
        {
            Ensure.ParameterNotNull(provider, nameof(provider));
            _provider = provider;
        }

        public string EscapeString(string val)
        {
            throw new NotImplementedException();
        }

        public string GetWildcardPlaceholder()
        {
            throw new NotImplementedException();
        }

        public string GetStringColumnEqualComparison(string column, int paramIndex, TextColumnType columnType)
        {
            throw new NotImplementedException();
        }

        public string GetStringColumnWildcardComparison(string column, int paramIndex, TextColumnType columnType)
        {
            throw new NotImplementedException();
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
    }
}