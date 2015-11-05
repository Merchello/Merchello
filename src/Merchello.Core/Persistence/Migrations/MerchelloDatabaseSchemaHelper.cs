namespace Merchello.Core.Persistence.Migrations
{
    using System.Web.WebSockets;

    using Merchello.Core.Persistence.Migrations.Initial;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The merchello database schema helper.
    /// </summary>
    public class MerchelloDatabaseSchemaHelper
    {
        /// <summary>
        /// The <see cref="Database"/>.
        /// </summary>
        private readonly Database _db;

        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The _syntax provider.
        /// </summary>
        private readonly ISqlSyntaxProvider _syntaxProvider;

        /// <summary>
        /// The Umbraco <see cref="DatabaseSchemaHelper"/>.
        /// </summary>
        private readonly DatabaseSchemaHelper _umbSchemaHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloDatabaseSchemaHelper"/> class.
        /// </summary>
        public MerchelloDatabaseSchemaHelper()
            : this(ApplicationContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloDatabaseSchemaHelper"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application Context.
        /// </param>
        public MerchelloDatabaseSchemaHelper(ApplicationContext applicationContext)
            : this(applicationContext, LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloDatabaseSchemaHelper"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MerchelloDatabaseSchemaHelper(ApplicationContext applicationContext, ILogger logger)
            : this(applicationContext.DatabaseContext.Database, logger, applicationContext.DatabaseContext.SqlSyntax)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloDatabaseSchemaHelper"/> class.
        /// </summary>
        /// <param name="db">
        /// The DB.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="syntaxProvider">
        /// The syntax provider.
        /// </param>
        internal MerchelloDatabaseSchemaHelper(Database db, ILogger logger, ISqlSyntaxProvider syntaxProvider)
        {
            Mandate.ParameterNotNull(db, "db");
            Mandate.ParameterNotNull(logger, "logger");
            Mandate.ParameterNotNull(syntaxProvider, "syntaxProvider");

            _db = db;
            _logger = logger;
            _syntaxProvider = syntaxProvider;

            this._umbSchemaHelper = new DatabaseSchemaHelper(db, logger, syntaxProvider);            
        }

        /// <summary>
        /// Gets the Umbraco <see cref="DatabaseSchemaHelper"/>.
        /// </summary>
        internal DatabaseSchemaHelper UmbSchemaHelper 
        { 
            get
            {
                return _umbSchemaHelper;
            } 
        }

        /// <summary>
        /// The create database schema.
        /// </summary>
        public void CreateDatabaseSchema()
        {
            _logger.Info<MerchelloDatabaseSchemaHelper>("Initializing database schema creation");
            
            var creation = new DatabaseSchemaCreation(_db, _logger, _umbSchemaHelper, _syntaxProvider);
            creation.InitializeDatabaseSchema();

            _logger.Info<MerchelloDatabaseSchemaHelper>("Finalized database schema creation");
        }

        /// <summary>
        /// The uninstall database schema.
        /// </summary>
        internal void UninstallDatabaseSchema()
        {
            _logger.Info<MerchelloDatabaseSchemaHelper>("Uninstalling Merchello database schema");
            var creation = new DatabaseSchemaCreation(_db, _logger, _umbSchemaHelper, _syntaxProvider);
            creation.UninstallDatabaseSchema();
            _logger.Info<MerchelloDatabaseSchemaHelper>("Merchello database schema removed");
        }
    }
}