namespace Merchello.Core.Persistence
{
    using System;
    using System.Web;

    using Merchello.Core.Acquired;
    using Merchello.Core.Configuration;
    using Merchello.Core.Logging;

    using NPoco;

    /// <summary>
	/// The default implementation for the IDatabaseFactory
	/// </summary>
	/// <remarks>
	/// If we are running in an http context
	/// it will create one per context, otherwise it will be a global singleton object which is NOT thread safe
	/// since we need (at least) a new instance of the database object per thread.
	/// </remarks>
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

        /// <summary>
        ///  Name of the connection string in web.config.
        /// </summary>
        private readonly string _connectionStringName;

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
			_connectionStringName = connectionStringName;
	        _logger = logger;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDatabaseFactory"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// Name of the connection string in web.config
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if either the logger or connection string are null.
        /// </exception>
        public DefaultDatabaseFactory(string connectionString, string providerName, ILogger logger)
		{
	        if (logger == null) throw new ArgumentNullException(nameof(logger));
	        Ensure.ParameterNotNullOrEmpty(connectionString, nameof(connectionString));
            Ensure.ParameterNotNullOrEmpty(providerName, nameof(providerName));
			ConnectionString = connectionString;
			ProviderName = providerName;
            _logger = logger;
		}

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public string ProviderName { get; private set; }

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
						//double check
                        if (_nonHttpInstance == null)
						{
                            //_nonHttpInstance = string.IsNullOrEmpty(ConnectionString) == false && string.IsNullOrEmpty(ProviderName) == false
                            //                      ? new UmbracoDatabase(ConnectionString, ProviderName, _logger)
                            //                      : new UmbracoDatabase(_connectionStringName, _logger);
						}
					}
				}
                return _nonHttpInstance;
			}

			// we have an http context, so only create one per request
			if (HttpContext.Current.Items.Contains(typeof(DefaultDatabaseFactory)) == false)
			{
			    //HttpContext.Current.Items.Add(typeof (DefaultDatabaseFactory),
			    //                              string.IsNullOrEmpty(ConnectionString) == false && string.IsNullOrEmpty(ProviderName) == false
       //                                           ? new Database(ConnectionString, ProviderName, _logger)
       //                                           : new UmbracoDatabase(_connectionStringName, _logger));
			}
			return (MerchelloDatabase)HttpContext.Current.Items[typeof(DefaultDatabaseFactory)];
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
	}
}