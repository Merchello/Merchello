namespace Merchello.Tests.Umbraco.TestHelpers.Base
{
    using System;
    using System.Configuration;
    using System.Data.Common;

    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Persistence;
    using global::Umbraco.Core.Persistence.Querying;
    using global::Umbraco.Core.Persistence.SqlSyntax;

    using Merchello.Core.Acquired.Persistence;

    using NPoco;

    using IDatabaseFactory = global::Umbraco.Core.Persistence.IDatabaseFactory;

    public class TestUmbracoDatabaseFactory : IDatabaseFactory
    {
        private string _connectionString;

        private string _providerName;

        private DbProviderFactory _dbProviderFactory;

        private DatabaseType _databaseType;

        private ISqlSyntaxProvider _sqlSyntax;

        private readonly ILogger _logger;

        private readonly IQueryFactory _queryFactory;

        public TestUmbracoDatabaseFactory(ILogger logger, IQueryFactory queryFactory)
        {
            this._logger = logger;

            var settings = ConfigurationManager.ConnectionStrings["umbracoDbDSN"];

            this.Configured = false;
            this._connectionString = settings.ConnectionString;
            this._providerName = settings.ProviderName;

           this._queryFactory = queryFactory;

            Configured = false;
        }

        public bool Configured { get; private set; }

        public bool CanConnect
        {
            get
            {
                return DbConnectionExtensions.IsConnectionAvailable(this._connectionString, this._providerName);
            }
        }

        public IQueryFactory QueryFactory
        {
            get
            {
                EnsureConfigured();
                return this._queryFactory;
            }
        }
        
        public UmbracoDatabase GetDatabase()
        {
            EnsureConfigured();
            return new UmbracoDatabase(this._connectionString, this._sqlSyntax, this._databaseType, this._dbProviderFactory, this._logger);
        }

        public void Configure(string connectionString, string providerName)
        {
            this._connectionString = connectionString;
            this._providerName = providerName;
            this._dbProviderFactory = DbProviderFactories.GetFactory(this._providerName);
            this._databaseType = DatabaseType.Resolve(this._dbProviderFactory.GetType().Name, this._providerName);

            this._sqlSyntax = this._databaseType == DatabaseType.SQLCe
                             ? (ISqlSyntaxProvider)new SqlCeSyntaxProvider()
                             : new SqlServerSyntaxProvider(new Lazy<IDatabaseFactory>(() => null));
            this.Configured = true;
        }

        private void EnsureConfigured()
        {
            if (Configured) return;

            Configure(_connectionString, _providerName);
        }


        public void Dispose()
        {
        }

    }
}