using System;
using Merchello.Core.Configuration;
using Merchello.Core.Persistence.Migrations.Initial;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using Umbraco.Core.Logging;
using umbraco.interfaces;

namespace Merchello.Web.PackageActions
{
    /// <summary>
    /// This package action will create the database tables and initial data for Merchello.
    /// </summary>
    public class CreateDatabase : IPackageAction
    {
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
                var creation = new DatabaseSchemaCreation(Umbraco.Core.ApplicationContext.Current.DatabaseContext.Database);
                creation.InitializeDatabaseSchema();

                var creationData = new BaseDataCreation(Umbraco.Core.ApplicationContext.Current.DatabaseContext.Database);
                bool dataCreationResult = CreateInitialMerchelloData(creationData);

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
                var deletions = new DatabaseSchemaCreation(Umbraco.Core.ApplicationContext.Current.DatabaseContext.Database);
                deletions.UninstallDatabaseSchema();

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