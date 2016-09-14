namespace Merchello.Core.Acquired.Persistence
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Logging;

    internal static class DbConnectionExtensions
    { 
        public static bool IsConnectionAvailable(string connectionString, string providerName)
        {
            if (providerName != Constants.DbProviderNames.SqlCe
                && providerName != Constants.DbProviderNames.SqlServer)
                throw new NotSupportedException($"Provider \"{providerName}\" is not supported.");

            var factory = DbProviderFactories.GetFactory(providerName);
            var conn = factory.CreateConnection();

            if (conn == null)
                throw new InvalidOperationException($"Could not create a connection for provider \"{providerName}\".");

            conn.ConnectionString = connectionString;
            using (var connection = conn)
            {
                return connection.IsAvailable();
            }
        }

        public static bool IsAvailable(this IDbConnection connection)
        {
            try
            {
                connection.Open();
                connection.Close();
            }
            catch (DbException e)
            {
                // Don't swallow this error, the exception is super handy for knowing "why" its not available
                MultiLogHelper.WarnWithException<IDbConnection>("Configured database is reporting as not being available!", e);
                return false;
            }

            return true;
        }
    }
}
