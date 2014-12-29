namespace Merchello.Web.PackageActions
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Web.Configuration;
    using Core.Configuration;
    using Core.Configuration.Outline;
    using umbraco.cms.businesslogic.packager.standardPackageActions;
    using Umbraco.Core.IO;
    using Umbraco.Core.Logging;
    using umbraco.interfaces;

    /// <summary>
    /// This package action will Add a the Merchello configuration section to the web.config file and establish wire up the configSource attribute to point
    /// to the App_Plugins directory
    /// </summary>
    /// <remarks>
    /// This package action has been customized from the PackageActionsContrib Project.
    /// http://packageactioncontrib.codeplex.com
    /// </remarks>
    public class AddConfigSection : IPackageAction
    {
        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML.
        /// </summary>
        /// <returns>The Alias of the package action.</returns>
        public string Alias()
        {
            return string.Concat(MerchelloConfiguration.ApplicationName, "_AddConfigSection");
        }

        /// <summary>
        /// Executes the specified package name.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <returns>True or false inicating success</returns>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            try
            {
                var webConfig = WebConfigurationManager.OpenWebConfiguration("~/");
                if (webConfig.Sections[MerchelloConfiguration.ConfigurationName] == null)
                {
                    webConfig.Sections.Add(MerchelloConfiguration.ConfigurationName, new MerchelloSection());

                    var configPath = string.Concat("App_Plugins", Path.DirectorySeparatorChar, MerchelloConfiguration.ApplicationName, Path.DirectorySeparatorChar, "Config", Path.DirectorySeparatorChar, MerchelloConfiguration.ConfigurationName, ".config");
                    var xmlPath = IOHelper.MapPath(string.Concat("~/", configPath));

                    string xml;

                    using (var reader = new StreamReader(xmlPath))
                    {
                        xml = reader.ReadToEnd();
                    }
                                        
                    webConfig.Sections[MerchelloConfiguration.ConfigurationName].SectionInformation.ConfigSource = configPath;
                    webConfig.Save(ConfigurationSaveMode.Minimal);
                    
                    // TODO - this is a quick fix for M-149
                    using (var writer = new StreamWriter(xmlPath))
                    {
                        writer.Write(xml);
                    }                    
                }

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Concat("Error at install ", this.Alias(), " package action: ", ex);
                LogHelper.Error(typeof(AddConfigSection), message, ex);
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
        /// <returns>True or false inicating success</returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            try
            {
                var webConfig = WebConfigurationManager.OpenWebConfiguration("~/");
                if (webConfig.Sections[MerchelloConfiguration.ConfigurationName] != null)
                {
                    webConfig.Sections.Remove(MerchelloConfiguration.ConfigurationName);

                    webConfig.Save(ConfigurationSaveMode.Modified);
                }

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Concat("Error at undo ", this.Alias(), " package action: ", ex);
                LogHelper.Error(typeof(AddConfigSection), message, ex);
            }

            return false;
        }
    }
}