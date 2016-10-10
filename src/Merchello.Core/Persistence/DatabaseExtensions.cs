namespace Merchello.Core.Persistence
{
    using Umbraco.Core.Persistence;

    /// <summary>
    /// Extension methods for <see cref="Database"/>.
    /// </summary>
    internal static class DatabaseExtensions
    {
        /// <summary>
        /// Gets the size of a column (database field) for a given table.
        /// </summary>
        /// <param name="database">
        /// The <see cref="Database"/>.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int GetDbTableColumnSize(this Database database, string tableName, string columnName)
        {
            var sql = new Sql("SELECT character_maximum_length FROM information_schema.columns WHERE table_name = @table AND column_name = @column", new { @table = tableName, @column = columnName });
            return database.ExecuteScalar<int>(sql);
        }
    }
}