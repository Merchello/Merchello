namespace Merchello.Core.Persistence.Migrations.Initial
{
    /// <summary>
    /// Represents the initial database schema creation by running CreateTable for all DTOs against the db and
    /// provides schema validation information.
    /// </summary>
    internal interface IDatabaseSchemaCreation
    {
        /// <summary>
        /// Validates the database schema.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseSchemaResult"/>.
        /// </returns>
        DatabaseSchemaResult ValidateSchema();

        /// <summary>
        /// Installs the Merchello database schema.
        /// </summary>
        void InitializeDatabaseSchema();

        /// <summary>
        /// Uninstalls the Merchello database schema.
        /// </summary>
        void UninstallDatabaseSchema();
    }
}