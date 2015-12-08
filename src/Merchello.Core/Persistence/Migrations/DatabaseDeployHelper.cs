namespace Merchello.Core.Persistence.Migrations
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Persistence.Migrations.Initial;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Ensures Merchello database has been deployed on UaaS deploy and creates database on install.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    internal sealed class DatabaseDeployHelper
    {
        private readonly Database _database;

        private readonly ILogger _logger;

        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        public DatabaseDeployHelper(Database database, ILogger logger, ISqlSyntaxProvider syntaxProvider)
        {
            Mandate.ParameterNotNull(database, "database");
            Mandate.ParameterNotNull(logger, "logger");
            Mandate.ParameterNotNull(syntaxProvider, "syntaxProvider");

            _database = database;
            _logger = logger;
            _sqlSyntaxProvider = syntaxProvider;
        }

        /// <summary>
        /// Ensures the Merchello database has been installed on deploy.
        /// </summary>
        public void EnsureDatabase()
        {
            var databaseSchemaCreation = new DatabaseSchemaCreation(_database, _logger, new DatabaseSchemaHelper(_database, _logger, _sqlSyntaxProvider), _sqlSyntaxProvider);
            var schemaResult = databaseSchemaCreation.ValidateSchema();
            var dbVersion = schemaResult.DetermineInstalledVersion();

            if (dbVersion != new Version("0.0.0")) return;

            // install the database
            var schemaHelper = new MerchelloDatabaseSchemaHelper(this._database, this._logger, this._sqlSyntaxProvider);
            schemaHelper.CreateDatabaseSchema();

            var baseDataCreation = new BaseDataCreation(this._database, this._logger);
            baseDataCreation.InitializeBaseData("merchTypeField");
            baseDataCreation.InitializeBaseData("merchInvoiceStatus");
            baseDataCreation.InitializeBaseData("merchOrderStatus");
            baseDataCreation.InitializeBaseData("merchWarehouse");
            baseDataCreation.InitializeBaseData("merchGatewayProviderSettings");
            baseDataCreation.InitializeBaseData("merchStoreSetting");
            baseDataCreation.InitializeBaseData("merchShipmentStatus");
        }
    }
}