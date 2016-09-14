namespace Merchello.Core.Persistence
{
    using System;

    /// <summary>
    /// Used to create the UmbracoDatabase for use in the DatabaseContext
    /// 
    /// </summary>
    internal interface IDatabaseFactory : IDisposable
    {
        /// <summary>
        /// Gets (creates or retrieves) the "ambient" database connection.
        /// </summary>
        /// <returns>The "ambient" database connection.</returns>
		MerchelloDatabase GetDatabase();

        void Configure(string connectionString, string providerName);

        bool Configured { get; }

        bool CanConnect { get; }
    }
}