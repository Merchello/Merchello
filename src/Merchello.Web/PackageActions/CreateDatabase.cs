namespace Merchello.Web.PackageActions
{
    using System;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.Migrations;
    using Merchello.Core.Persistence.Migrations.Initial;

    using umbraco.cms.businesslogic.packager.standardPackageActions;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;

    using umbraco.interfaces;

    /// <summary>
    /// This package action will create the database tables and initial data for Merchello.
    /// </summary>
    public class CreateDatabase : IPackageAction
    {
        /// <summary>
        /// The schema helper.
        /// </summary>
        private readonly MerchelloDatabaseSchemaHelper _schemaHelper;

        /// <summary>
        /// The _database.
        /// </summary>
        private readonly Database _database;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDatabase"/> class.
        /// </summary>
        public CreateDatabase() 
            : this(ApplicationContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDatabase"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        public CreateDatabase(ApplicationContext applicationContext)
            : this(applicationContext.DatabaseContext.Database, applicationContext.DatabaseContext.SqlSyntax, LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDatabase"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="sqlSyntaxProvider">
        /// The sql Syntax Provider.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        internal CreateDatabase(Database database, ISqlSyntaxProvider sqlSyntaxProvider, ILogger logger)
        {
            _database = database;
            _logger = logger;
            _schemaHelper = new MerchelloDatabaseSchemaHelper(database, logger, sqlSyntaxProvider);
        }

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML.
        /// </summary>
        /// <returns>The Alias of the package action.</returns>
        public string Alias()
        {
            return string.Concat(MerchelloConfiguration.ApplicationName, "_CreateDatabase");
        }

        /// <summary>
        /// Executes the specified package name.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <returns></returns>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {

            try
            {
                //// RSS - removed the database install portion of the package action
                //// to rely on tests in CoreMigrationManager for UaaS deploys
                //// Uninstall will need to be done manually.

                //_schemaHelper.CreateDatabaseSchema();

                //var creationData = new BaseDataCreation(_database, _logger);
                //var dataCreationResult = CreateInitialMerchelloData(creationData);

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Concat("Error at install ", this.Alias(), " package action: ", ex);
                LogHelper.Error(typeof(CreateDatabase), message, ex);
            }

            return false;
        }

        /// <summary>
        /// Returns a Sample XML Node
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public System.Xml.XmlNode SampleXml()
        {
            var xml = string.Concat("<Action runat=\"install\" undo=\"true\" alias=\"", this.Alias(), "\" />");
            return helper.parseStringToXmlNode(xml);
        }

        /// <summary>
        /// Undoes the specified package name.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <returns></returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            try
            {
                _schemaHelper.UninstallDatabaseSchema();

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Concat("Error at undo ", this.Alias(), " package action: ", ex);
                LogHelper.Error(typeof(CreateDatabase), message, ex);
            }

            return false;
        }

        /// <summary>
        /// Installs Merchello default data.
        /// </summary>
        /// <param name="baseDataCreation">
        /// The base data creation.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CreateInitialMerchelloData(BaseDataCreation baseDataCreation)
        {
            baseDataCreation.InitializeBaseData("merchTypeField");
            baseDataCreation.InitializeBaseData("merchInvoiceStatus");
            baseDataCreation.InitializeBaseData("merchOrderStatus");
            baseDataCreation.InitializeBaseData("merchWarehouse");
            baseDataCreation.InitializeBaseData("merchGatewayProviderSettings");
            baseDataCreation.InitializeBaseData("merchStoreSetting");
            baseDataCreation.InitializeBaseData("merchShipmentStatus");
            return true;
        }
    }
}