namespace Merchello.Core.Persistence
{
    using System;
    using System.Linq;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.Persistence.DatabaseModelDefinitions;
    using Merchello.Core.Events;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Migrations.Initial;
    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    /// <summary>
    /// Represents a database schema manager.
    /// </summary>
    internal abstract class DatabaseSchemaManagerBase : IDatabaseSchemaManager
    {
        /// <summary>
        /// The database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="IDatabaseFactory"/>.
        /// </summary>
        private readonly IDatabaseFactory _dbFactory;

        /// <summary>
        /// The SQL syntax provider for translating SQL specific to the database.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntax;

        /// <summary>
        /// A class for adding the default data.
        /// </summary>
        private Lazy<BaseDataCreation> _baseDataCreation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchemaManagerBase"/> class.
        /// </summary>
        /// <param name="dbFactory">
        /// The database query factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        protected DatabaseSchemaManagerBase(IDatabaseFactory dbFactory, ILogger logger)
        {
            Ensure.ParameterNotNull(dbFactory, nameof(dbFactory));
            Ensure.ParameterNotNull(logger, nameof(logger));

            _dbFactory = dbFactory;
            _database = dbFactory.GetDatabase();
            _sqlSyntax = dbFactory.QueryFactory.SqlSyntax;
            _logger = logger;

            this.Initialize();
        }

        /// <inheritdoc/>
        internal BaseDataCreation BaseDataCreation
        {
            get
            {
                return _baseDataCreation.Value;
            }
        }

        /// <inheritdoc/>
        public abstract bool TableExist(string tableName);

        /// <inheritdoc/>
        public abstract void CreateTable<T>(bool overwrite) where T : new();

        /// <inheritdoc/>
        public abstract void CreateTable<T>() where T : new();

        /// <inheritdoc/>
        public void CreateTable(bool overwrite, Type modelType)
        {
            // FYI - can't use this directly since Merchello implements it's own Definition factory using
            // internal types
            var tableDefinition = DefinitionFactory.GetTableDefinition(modelType, this._sqlSyntax);
            var tableName = tableDefinition.Name;

            var createSql = this._sqlSyntax.Format(tableDefinition);
            var createPrimaryKeySql = this._sqlSyntax.FormatPrimaryKey(tableDefinition);
            var foreignSql = this._sqlSyntax.Format(tableDefinition.ForeignKeys);
            var indexSql = this._sqlSyntax.Format(tableDefinition.Indexes);

            var tableExist = this.TableExist(tableName);
            if (overwrite && tableExist)
            {
                this.DropTable(tableName);
                tableExist = false;
            }

            if (tableExist == false)
            {
                using (var transaction = this._database.GetTransaction())
                {
                    // Execute the Create Table sql
                    var created = this._database.Execute(new Sql(createSql));
                    this._logger.Info<Database>($"Create Table sql {created}:\n {createSql}");

                    // If any statements exists for the primary key execute them here
                    if (string.IsNullOrEmpty(createPrimaryKeySql) == false)
                    {
                        var createdPk = this._database.Execute(new Sql(createPrimaryKeySql));
                        this._logger.Info<Database>($"Primary Key sql {createdPk}:\n {createPrimaryKeySql}");
                    }

                    // Turn on identity insert if db provider is not mysql
                    if (this._sqlSyntax.SupportsIdentityInsert() && tableDefinition.Columns.Any(x => x.IsIdentity))
                        this._database.Execute(new Sql($"SET IDENTITY_INSERT {this._sqlSyntax.GetQuotedTableName(tableName)} ON "));

                    // Turn off identity insert if db provider is not mysql
                    if (this._sqlSyntax.SupportsIdentityInsert() && tableDefinition.Columns.Any(x => x.IsIdentity))
                        this._database.Execute(new Sql($"SET IDENTITY_INSERT {this._sqlSyntax.GetQuotedTableName(tableName)} OFF;"));


                    // Loop through index statements and execute sql
                    foreach (var sql in indexSql)
                    {
                        var createdIndex = this._database.Execute(new Sql(sql));
                        this._logger.Info<Database>($"Create Index sql {createdIndex}:\n {sql}");
                    }

                    // Loop through foreignkey statements and execute sql
                    foreach (var sql in foreignSql)
                    {
                        var createdFk = this._database.Execute(new Sql(sql));
                        this._logger.Info<Database>($"Create Foreign Key sql {createdFk}:\n {sql}");
                    }

                    transaction.Complete();
                }

                //this._baseDataCreation.Value.InitializeBaseData(tableName);
            }

            this._logger.Info<Database>($"New table '{tableName}' was created");
        }

        /// <inheritdoc/>
        public void DropTable<T>()
            where T : new()
        {
            var type = typeof(T);
            var tableNameAttribute = type.FirstAttribute<TableNameAttribute>();
            if (tableNameAttribute == null)
                throw new Exception($"The Type '{type.Name}' does not contain a TableNameAttribute, which is used"
                    + " to find the name of the table to drop. The operation could not be completed.");

            var tableName = tableNameAttribute.Value;
            this.DropTable(tableName);
        }

        /// <inheritdoc/>
        public abstract void DropTable(string tableName);

        /// <summary>
        /// Installs the Merchello database schema.
        /// </summary>
        public void InstallDatabaseSchema()
        {
            var creation = new DatabaseSchemaCreation(_dbFactory, _logger, this);
            creation.InitializeDatabaseSchema();

            _baseDataCreation.Value.InitializeBaseData();
        }

        /// <summary>
        /// Uninstalls the Merchello database schema.
        /// </summary>
        public void UninstallDatabaseSchema()
        {
            var creation = new DatabaseSchemaCreation(_dbFactory, _logger, this);
            creation.UninstallDatabaseSchema();
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {
            _baseDataCreation = new Lazy<BaseDataCreation>(() => new BaseDataCreation(_dbFactory.GetDatabase(), _logger));
        }
    }
}