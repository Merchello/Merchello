namespace Merchello.Core.Persistence.SqlSyntax
{
    /// <summary>
    /// Represents column information.
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the ordinal.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the column default.
        /// </summary>
        public string ColumnDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public string DataType { get; set; }
    }
}