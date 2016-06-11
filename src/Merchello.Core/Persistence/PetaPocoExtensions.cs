namespace Merchello.Core.Persistence
{
    using System.Configuration;

    using umbraco;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// PetaPoco extensions.
    /// </summary>
    /// <remarks>
    /// This is to fix broken Umbraco extension
    /// </remarks>
    public static class PetaPocoExtensions
    {
        /// <summary>
        /// The get merchello database provider.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseProviders"/>.
        /// </returns>
        public static DatabaseProviders GetMerchelloDatabaseProvider(this Database database)
        {
            string dbtype = database.Connection == null ? 
                ConfigurationManager.ConnectionStrings[Constants.DefaultConnectionStringName].ProviderName : 
                database.Connection.GetType().Name;

            if (dbtype.StartsWith("MySql")) return DatabaseProviders.MySql;
            if (dbtype.StartsWith("SqlCe") || dbtype.Contains("SqlServerCe")) return DatabaseProviders.SqlServerCE;
            if (dbtype.StartsWith("Npgsql")) return DatabaseProviders.PostgreSQL;
            if (dbtype.StartsWith("Oracle") || dbtype.Contains("OracleClient")) return DatabaseProviders.Oracle;
            if (dbtype.StartsWith("SQLite")) return DatabaseProviders.SQLite;
            if (dbtype.Contains("Azure")) return DatabaseProviders.SqlAzure;

            return DatabaseProviders.SqlServer;
        } 
         
    }
}