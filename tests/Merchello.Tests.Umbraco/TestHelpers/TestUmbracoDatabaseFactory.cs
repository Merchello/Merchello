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

    using Moq;

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
            _logger = logger;

            var settings = ConfigurationManager.ConnectionStrings["umbracoDbDSN"];

            this.Configured = false;
            _connectionString = settings.ConnectionString;
            _providerName = settings.ProviderName;

           _queryFactory = queryFactory;
        }

        public bool Configured { get; private set; }

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
                return _queryFactory;
            }
        }
        
        public UmbracoDatabase GetDatabase()
        {            
            return new UmbracoDatabase(_connectionString, _sqlSyntax, _databaseType, _dbProviderFactory, _logger);
        }

        public void Configure(string connectionString, string providerName)
        {
            _connectionString = connectionString;
            _providerName = providerName;
            _dbProviderFactory = DbProviderFactories.GetFactory(_providerName);
            _databaseType = DatabaseType.Resolve(_dbProviderFactory.GetType().Name, _providerName);

            _sqlSyntax = _databaseType == DatabaseType.SQLCe
                             ? (ISqlSyntaxProvider)new SqlCeSyntaxProvider()
                             : new SqlServerSyntaxProvider(new Lazy<IDatabaseFactory>(() => null));
            this.Configured = true;
        }


        public void Dispose()
        {
        }

    }
}