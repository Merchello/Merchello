namespace Merchello.Core.Persistence.Migrations
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// The <see cref="MerchelloMigrationContext"/>.
    /// </summary>
    internal class MerchelloMigrationContext : IMigrationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloMigrationContext"/> class.
        /// </summary>
        /// <param name="databaseProvider">
        /// The database provider.
        /// </param>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MerchelloMigrationContext(DatabaseProviders databaseProvider, Database database, ILogger logger)
        {
            Expressions = new Collection<IMigrationExpression>();
            CurrentDatabaseProvider = databaseProvider;
            Database = database;
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the expressions.
        /// </summary>
        public ICollection<IMigrationExpression> Expressions { get; set; }

        /// <summary>
        /// Gets the current database provider.
        /// </summary>
        public DatabaseProviders CurrentDatabaseProvider { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        public Database Database { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger { get; private set; }
    }
}