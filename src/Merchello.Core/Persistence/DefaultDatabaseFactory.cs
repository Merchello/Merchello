namespace Merchello.Core.Persistence
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Threading;
    using System.Web;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.Persistence;
    using Merchello.Core.Acquired.Persistence.Mappers;
    using Merchello.Core.Acquired.Threading;
    using Merchello.Core.Configuration;
    using Merchello.Core.Logging;

    using NPoco;
    using NPoco.FluentMappings;

    /// <summary>
    /// The default implementation for the IDatabaseFactory
    /// </summary>
    /// <remarks>
    /// This is a simplified version of Umbraco's DefautDatabaseFactory
    /// </remarks>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v8/src/Umbraco.Core/Persistence/DefaultDatabaseFactory.cs"/> 
    internal class DefaultDatabaseFactory : DisposableObject, IDatabaseFactory
	{
        private const string HttpItemKey = "Merchello.Core.Persistence.DefaultDatabaseFactory";

        private readonly IScopeContextAdapter _scopeContextAdapter;
        private IPocoDataFactory _pocoDataFactory;
        private DatabaseFactory _databaseFactory;
        private string _connectionString;
        private string _providerName;
        private DbProviderFactory _dbProviderFactory;
        private DatabaseType _databaseType;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDatabaseFactory"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="scopeContextAdapter">
        /// An adapter for managing the scope that database objects are instantiated or retrieved - either in the HttpContext or 
        /// assures that there is a unique db object per thread.
        /// </param>
        /// <remarks>
        /// Uses MerchelloConfig.For.Settings.DefaultConnectionStringName which defaults to 'umbracoDbDSN'
        /// </remarks>
        public DefaultDatabaseFactory(ILogger logger, IScopeContextAdapter scopeContextAdapter) 
            : this(MerchelloConfig.For.Settings.DefaultConnectionStringName, logger, scopeContextAdapter)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDatabaseFactory"/> class.
        /// </summary>
        /// <param name="connectionStringName">
        /// Name of the connection string in web.config.
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        /// <param name="scopeContextAdapter">
        /// An adapter for managing the scope that database objects are instantiated or retrieved - either in the HttpContext or 
        /// assures that there is a unique db object per thread.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception where the connection string name is null.
        /// </exception>
        public DefaultDatabaseFactory(string connectionStringName, ILogger logger, IScopeContextAdapter scopeContextAdapter)
		{
	        if (logger == null) throw new ArgumentNullException(nameof(logger));
	        Ensure.ParameterNotNullOrEmpty(connectionStringName, nameof(connectionStringName));
            Ensure.ParameterNotNull(scopeContextAdapter, nameof(scopeContextAdapter));

            _scopeContextAdapter = scopeContextAdapter;

            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (settings == null)
                return; // not configured

	        _logger = logger;

            Configure(settings.ConnectionString, settings.ProviderName);
		}

        public bool Configured { get; private set; }

        public bool CanConnect => Configured && DbConnectionExtensions.IsConnectionAvailable(_connectionString, _providerName);

        /// <summary>
        /// Gets (creates or retrieves) the "ambient" database connection.
        /// </summary>
        /// <returns>The "ambient" database connection.</returns>
        public MerchelloDatabase GetDatabase()
        {
            EnsureConfigured();

            // check if it's in scope
            var db = _scopeContextAdapter.Get(HttpItemKey) as MerchelloDatabase;
            if (db != null) return db;
            db = (MerchelloDatabase)_databaseFactory.GetDatabase();
            _scopeContextAdapter.Set(HttpItemKey, db);
            return db;
        }

        public void Configure(string connectionString, string providerName)
        {
            using (new WriteLock(_lock))
            {
                _logger.Debug<DefaultDatabaseFactory>("Configuring!");

                if (Configured) throw new InvalidOperationException("Already configured.");

                if (connectionString.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(connectionString));
                if (providerName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(providerName));

                _connectionString = connectionString;
                _providerName = providerName;


                _dbProviderFactory = DbProviderFactories.GetFactory(_providerName);
                if (_dbProviderFactory == null)
                    throw new Exception($"Can't find a provider factory for provider name \"{_providerName}\".");
                _databaseType = DatabaseType.Resolve(_dbProviderFactory.GetType().Name, _providerName);
                if (_databaseType == null)
                    throw new Exception($"Can't find an NPoco database type for provider name \"{_providerName}\".");


                // ensure we have only 1 set of mappers, and 1 PocoDataFactory, for all database
                // so that everything NPoco is properly cached for the lifetime of the application
                var mappers = new MapperCollection { new PocoMapper() };
                var factory = new FluentPocoDataFactory((type, iPocoDataFactory) => new PocoDataBuilder(type, mappers).Init());
                _pocoDataFactory = factory;
                var config = new FluentConfig(xmappers => factory);

                // create the database factory
                _databaseFactory = DatabaseFactory.Config(x => x
                    .UsingDatabase(CreateDatabaseInstance) // creating UmbracoDatabase instances
                    .WithFluentConfig(config)); // with proper configuration

                if (_databaseFactory == null) throw new NullReferenceException("The call to DatabaseFactory.Config yielded a null DatabaseFactory instance");

                _logger.Debug<DefaultDatabaseFactory>("Created non Http Instance");
                Configured = true;
            }
        }


        private void EnsureConfigured()
        {
            using (new ReadLock(_lock))
            {
                if (Configured == false)
                    throw new InvalidOperationException("Not configured.");
            }
        }

        protected override void DisposeResources()
        {
            // this is weird, because the non http instance is thread-static, so we would need
            // to dispose the factory in each thread where a database has been used - else
            // it only disposes the current thread's database instance.
            //
            // besides, we don't really want to dispose the factory, which is a singleton...

            var db = _scopeContextAdapter.Get(HttpItemKey) as MerchelloDatabase;
            _scopeContextAdapter.Clear(HttpItemKey);
            db?.Dispose();
            Configured = false;
        }

        // method used by NPoco's DatabaseFactory to actually create the database instance
        private MerchelloDatabase CreateDatabaseInstance()
        {
            return new MerchelloDatabase(_connectionString, _databaseType, _dbProviderFactory, _logger);
        }

    }
}