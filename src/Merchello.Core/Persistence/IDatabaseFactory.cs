namespace Merchello.Core.Persistence
{
    using System;

    using Merchello.Core.Acquired.Persistence.Querying;
    using Merchello.Core.Persistence.Querying;

    using NPoco;

    /// <summary>
    /// Used to create the Database for use in the DatabaseContext
    /// </summary>
    internal interface IDatabaseFactory : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the database is configured.
        /// </summary>
        bool Configured { get; }

        /// <summary>
        /// Gets a value indicating whether a database connection can be established.
        /// </summary>
        bool CanConnect { get; }

        /// <summary>
        /// Gets the factory responsible for creating queries respecting the SQL syntax of the connected database.
        /// </summary>
        IQueryFactory QueryFactory { get; }

        /// <summary>
        /// Gets (creates or retrieves) the "ambient" database connection.
        /// </summary>
        /// <returns>The "ambient" database connection.</returns>
		IMerchelloDatabase GetDatabase();
    }
}