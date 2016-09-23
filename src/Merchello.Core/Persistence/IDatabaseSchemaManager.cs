namespace Merchello.Core.Persistence
{
    using System;

    using Merchello.Core.Persistence.Migrations.Initial;

    /// <summary>
    /// Represents a database schema manager.
    /// </summary>
    /// <remarks>
    /// This cannot be adapted since we are not using Umbraco's attributes.
    /// </remarks>
    internal interface IDatabaseSchemaManager
    {
        /// <summary>
        /// Checks if a table exists in the database.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the table exists.
        /// </returns>
        bool TableExist(string tableName);

        /// <summary>
        /// Creates a table in the database.
        /// </summary>
        /// <param name="overwrite">
        /// A value indicating whether or not to drop the table and recreate it.
        /// </param>
        /// <param name="modelType">
        /// The type of the DTO that defines the table.
        /// </param>
        void CreateTable(bool overwrite, Type modelType);

        /// <summary>
        /// Creates a table in the database
        /// </summary>
        /// <typeparam name="T">
        /// The type of the DTO that defines the table.
        /// </typeparam>
        void CreateTable<T>() where T : new();

        /// <summary>
        /// Creates a table in the database and installs default data.
        /// </summary>
        /// <param name="overwrite">
        /// A value indicating whether or not to drop the table and recreate it.
        /// </param>
        /// <typeparam name="T">
        /// The type of the DTO that defines the table.
        /// </typeparam>
        void CreateTable<T>(bool overwrite) where T : new();

        /// <summary>
        /// Drops a table from the database.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        void DropTable(string tableName);

        /// <summary>
        /// Drops a table from the database
        /// </summary>
        /// <typeparam name="T">
        /// The type of the DTO that defines the table.
        /// </typeparam>
        void DropTable<T>() where T : new();

        /// <summary>
        /// Installs the Merchello database schema.
        /// </summary>
        void InstallDatabaseSchema();

        /// <summary>
        /// Uninstalls the Merchello database schema.
        /// </summary>
        void UninstallDatabaseSchema();
    }
}