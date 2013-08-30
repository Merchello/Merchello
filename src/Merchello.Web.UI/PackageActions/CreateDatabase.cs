﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;
using Merchello.Core;
using Merchello.Core.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Persistence.Migrations.Initial;

namespace Merchello.Web.UI.PackageActions
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


        private bool CreateInitialMerchelloData(BaseDataCreation baseDataCreation)
        {
            baseDataCreation.InitializeBaseData("merchDBTypeField");
            baseDataCreation.InitializeBaseData("merchInvoiceStatus");

            return true;
        }
    }
}