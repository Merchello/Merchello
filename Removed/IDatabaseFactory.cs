namespace Merchello.Core.Persistence
{
    using System;

    using NPoco;

    /// <summary>
    /// Used to create the Database for use in the DatabaseContext
    /// </summary>
    internal interface IDatabaseFactory : IDisposable
    {
        /// <summary>
        /// Gets (creates or retrieves) the "ambient" database connection.
        /// </summary>
        /// <returns>The "ambient" database connection.</returns>
		Database GetDatabase();

        void Configure(string connectionString, string providerName);

        bool Configured { get; }

        bool CanConnect { get; }
    }
}