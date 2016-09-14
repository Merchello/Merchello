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
    /// REFACTOR-todo watch Umbraco for changes here.  V8 version has some tricky threading assurances
    /// to assert single new instance of each database object per thread and unique model mappers are present.  Also  retry policies. 
    internal class DefaultDatabaseFactory : DisposableObject, IDatabaseFactory
	{
        /// <summary>
        /// The thread locker.
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// Holds the non http instance.
        /// </summary>
        /// <remarks>
        /// very important to have ThreadStatic:
        /// see: http://issues.umbraco.org/issue/U4-2172
        /// </remarks>
        [ThreadStatic]
        private static volatile MerchelloDatabase _nonHttpInstance;
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
        /// <remarks>
        /// Uses MerchelloConfig.For.Settings.DefaultConnectionStringName which defaults to 'umbracoDbDSN'
        /// </remarks>
        public DefaultDatabaseFactory(ILogger logger) 
            : this(MerchelloConfig.For.Settings.DefaultConnectionStringName, logger)
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
        /// <exception cref="ArgumentNullException">
        /// Throws an exception where the connection string name is null.
        /// </exception>
        public DefaultDatabaseFactory(string connectionStringName, ILogger logger)
		{
	        if (logger == null) throw new ArgumentNullException(nameof(logger));
	        Ensure.ParameterNotNullOrEmpty(connectionStringName, "connectionStringName");

            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (settings == null)
                return; // not configured

           
	        _logger = logger;

            Configure(settings.ConnectionString, settings.ProviderName);
		}

        public bool Configured { get; private set; }

        public bool CanConnect => Configured && DbConnectionExtensions.IsConnectionAvailable(_connectionString, _providerName);

        /// <summary>
        /// Creates an instance of the <see cref="MerchelloDatabase"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="MerchelloDatabase"/>.
        /// </returns>
        public MerchelloDatabase CreateDatabase()
		{
			// no http context, create the singleton global object
			if (HttpContext.Current == null)
			{
                if (_nonHttpInstance == null)
				{
					lock (Locker)
					{
						// double check
                        if (_nonHttpInstance == null)
						{
                            //_nonHttpInstance = string.IsNullOrEmpty(ConnectionString) == false && string.IsNullOrEmpty(ProviderName) == false
                            //                      ? new MerchelloDatabase(ConnectionString, ProviderName, _logger)
                            //                      : new MerchelloDatabase(_connectionStringName, _logger);
                        }
					}
				}
                return _nonHttpInstance;
			}

			// we have an http context, so only create one per request
			if (HttpContext.Current.Items.Contains(typeof(DefaultDatabaseFactory)) == false)
			{
                //HttpContext.Current.Items.Add(
                //    typeof(DefaultDatabaseFactory),
                //    string.IsNullOrEmpty(ConnectionString) == false && string.IsNullOrEmpty(ProviderName) == false
                //        ? new Database(ConnectionString, ProviderName, _logger)
                //        : new UmbracoDatabase(_connectionStringName, _logger));
            }
			return (MerchelloDatabase)HttpContext.Current.Items[typeof(DefaultDatabaseFactory)];
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

                _logger.Debug<DefaultDatabaseFactory>("Created _nonHttpInstance");
                Configured = true;
            }
        }

        /// <summary>
        /// Disposes the database.
        /// </summary>
        protected override void DisposeResources()
		{
			if (HttpContext.Current == null)
			{
                _nonHttpInstance.Dispose();
			}
			else
			{
				if (HttpContext.Current.Items.Contains(typeof(DefaultDatabaseFactory)))
				{
					((Database)HttpContext.Current.Items[typeof(DefaultDatabaseFactory)]).Dispose();
				}
			}
		}

        // method used by NPoco's DatabaseFactory to actually create the database instance
        private MerchelloDatabase CreateDatabaseInstance()
        {
            return new MerchelloDatabase(_connectionString, _databaseType, _dbProviderFactory, _logger);
        }

    }
}