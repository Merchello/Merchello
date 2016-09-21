namespace Merchello.Tests.Umbraco.TestHelpers
{
    using System;
    using System.Configuration;
    using System.Data.Common;

    using Merchello.Core.Acquired.Persistence;

    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Persistence;
    using global::Umbraco.Core.Persistence.Querying;
    using global::Umbraco.Core.Persistence.SqlSyntax;

    using NPoco;

    using IDatabaseFactory = global::Umbraco.Core.Persistence.IDatabaseFactory;

    public class TestDatabaseFactory : IDatabaseFactory
    {
        private readonly string _connectionString;

        private readonly string _providerName;

        public TestDatabaseFactory()
        {
            var settings = ConfigurationManager.ConnectionStrings["umbracoDbDSN"];
            _connectionString = settings.ConnectionString;
            _providerName = settings.ProviderName;
        }

        public bool Configured
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CanConnect
        {
            get
            {
                return DbConnectionExtensions.IsConnectionAvailable(_connectionString, _providerName);
            }
        }

        public IQueryFactory QueryFactory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public UmbracoDatabase GetDatabase()
        {            
            var dbProviderFactory = DbProviderFactories.GetFactory(_providerName);
            return new UmbracoDatabase(_connectionString, new SqlCeSyntaxProvider(), DatabaseType.SQLCe, dbProviderFactory, Logger.CreateWithDefaultLog4NetConfiguration());
        }

        public void Configure(string connectionString, string providerName)
        {
            throw new System.NotImplementedException();
        }


        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

    }
}