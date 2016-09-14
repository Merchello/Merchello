namespace Merchello.Core.Persistence
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Text;

    using Merchello.Core.Logging;

    using NPoco;

    using StackExchange.Profiling;

    /// <summary>
    /// Represents the Merchello implementation of the NPoco Database object
    /// </summary>
    /// <remarks>
    /// Currently this object exists for 'future proofing' our implementation. By having our own inherited implementation we
    /// can then override any additional execution (such as additional logging, functionality, etc...) that we need to without breaking compatibility since we'll always be exposing
    /// this object instead of the base NPoco database object.
    /// </remarks>
    public class MerchelloDatabase : Database
    {
        // Merchello's default isolation level is RepeatableRead
        private const IsolationLevel DefaultIsolationLevel = IsolationLevel.RepeatableRead;

        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// A unique id for the current instance
        /// </summary>
        private readonly Guid _instanceId = Guid.NewGuid();


        private bool _enableCount;

        // used by DefaultDatabaseFactory
        // creates one instance per request
        // also used by DatabaseContext for creating DBs and upgrading
        public MerchelloDatabase(string connectionString, DatabaseType databaseType, DbProviderFactory provider, ILogger logger)
            : base(connectionString, databaseType, provider, DefaultIsolationLevel)
        {
            this._logger = logger;
            this.EnableSqlTrace = false;
        }

        // INTERNAL FOR UNIT TESTS
        internal MerchelloDatabase(DbConnection connection, DatabaseType databaseType, DbProviderFactory provider,
            ILogger logger)
            : base(connection, databaseType, provider, DefaultIsolationLevel)
        {
            this._logger = logger;
            this.EnableSqlTrace = false;
        }

        /// <summary>
        /// Gets the instance id - Used for testing
        /// </summary>
        internal Guid InstanceId
        {
            get { return this._instanceId; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to enable SQL tracing.
        /// Generally used for testing, will output all SQL statements executed to the logger
        /// </summary>
        internal bool EnableSqlTrace { get; set; }

        /// <summary>
        /// Gets the SQL count - Used for testing
        /// </summary>
        internal int SqlCount { get; private set; }

        /// <summary>
        /// Used for testing
        /// </summary>
        internal void EnableSqlCount()
        {
            this._enableCount = true;
        }

        /// <summary>
        /// Used for testing
        /// </summary>
        internal void DisableSqlCount()
        {
            this._enableCount = false;
            this.SqlCount = 0;
        }

        /// <summary>
        /// Wrap the connection with a profiling connection that tracks timings.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="DbConnection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the connection is null.
        /// </exception>
        protected override DbConnection OnConnectionOpened(DbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Wrap the connection with a profiling connection that tracks timings
            connection = new StackExchange.Profiling.Data.ProfiledDbConnection(connection, MiniProfiler.Current);

            return connection;
        }

        /// <summary>
        /// Logs an exception when it occurs.
        /// </summary>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        protected override void OnException(Exception ex)
        {
            _logger.Error<MerchelloDatabase>("Database exception occurred", ex);
            base.OnException(ex);
        }

        /// <summary>
        /// Optionally enables tracing and extends command time out when executing a command.
        /// </summary>
        /// <param name="cmd">
        /// The <see cref="DbCommand"/>.
        /// </param>
        protected override void OnExecutingCommand(DbCommand cmd)
        {
            // if no timeout is specified, and the connection has a longer timeout, use it
            if (OneTimeCommandTimeout == 0 && CommandTimeout == 0 && cmd.Connection.ConnectionTimeout > 30)
                cmd.CommandTimeout = cmd.Connection.ConnectionTimeout;

            if (EnableSqlTrace)
            {
                var sb = new StringBuilder();
                sb.Append(cmd.CommandText);
                foreach (DbParameter p in cmd.Parameters)
                {
                    sb.Append(" - ");
                    sb.Append(p.Value);
                }

                _logger.Debug<MerchelloDatabase>(sb.ToString());
            }

            base.OnExecutingCommand(cmd);
        }

        /// <summary>
        /// Increments the SQL count when a command executes.
        /// </summary>
        /// <param name="cmd">
        /// The <see cref="DbCommand"/>.
        /// </param>
        protected override void OnExecutedCommand(DbCommand cmd)
        {
            if (_enableCount)
            {
                SqlCount++;
            }

            base.OnExecutedCommand(cmd);
        }
    }
}