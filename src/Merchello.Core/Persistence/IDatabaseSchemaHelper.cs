namespace Merchello.Core.Persistence
{
    using System;

    /// <summary>
    /// Represents a database schema manager.
    /// </summary>
    /// <remarks>
    /// This cannot be adapted since we are not using Umbraco's attributes.
    /// </remarks>
    internal interface IDatabaseSchemaHelper
    {
       //// REFACTOR - these two methods are the entry point for installing the database schema
       //// and we don't need the application context
       // void CreateDatabaseSchema(ApplicationContext applicationContext);
       // void CreateDatabaseSchema(bool guardConfiguration, ApplicationContext applicationContext);

        /// <summary>
        /// Creates a table in the database.
        /// </summary>
        /// <param name="overwrite">
        /// A value indicating whether or not to drop the table and recreate it.
        /// </param>
        /// <param name="modelType">
        /// The Dto that defines.
        /// </param>
        void CreateTable(bool overwrite, Type modelType);
        void CreateTable<T>() where T : new();
        void CreateTable<T>(bool overwrite) where T : new();
        void DropTable(string tableName);
        void DropTable<T>() where T : new();
        bool TableExist(string tableName);
    }
}