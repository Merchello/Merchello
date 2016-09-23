namespace Merchello.Core.Events
{
    using System;

    /// <summary>
    /// Event arguments for database table creation.
    /// </summary>
    internal class CreateTableEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTableEventArgs"/> class.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        public CreateTableEventArgs(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName { get; }
    }
}