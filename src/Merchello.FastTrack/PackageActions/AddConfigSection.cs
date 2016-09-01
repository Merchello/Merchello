namespace Merchello.FastTrack.PackageActions
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Web.Configuration;
    using Configuration;
    using umbraco.cms.businesslogic.packager.standardPackageActions;
    using umbraco.interfaces;
    using Umbraco.Core.IO;
    using Umbraco.Core.Logging;

    /// <summary>
    /// This package action will Add a the FastTrack configuration section to the web.config file and establish wire up the configSource attribute to point
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
            return string.Concat(FastTrackConfiguration.ApplicationName, "_AddConfigSection");
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
                if (webConfig.Sections[FastTrackConfiguration.ConfigurationName] == null)
                {
                    webConfig.Sections.Add(FastTrackConfiguration.ConfigurationName, new FastTrackSection());

                    var configPath = string.Concat("App_Plugins", Path.DirectorySeparatorChar, FastTrackConfiguration.ApplicationName, Path.DirectorySeparatorChar, "Config", Path.DirectorySeparatorChar, FastTrackConfiguration.ConfigurationName, ".config");
                    var xmlPath = IOHelper.MapPath(string.Concat("~/", configPath));

                    string xml;

                    using (var reader = new StreamReader(xmlPath))
                    {
                        xml = reader.ReadToEnd();
                    }

                    webConfig.Sections[FastTrackConfiguration.ConfigurationName].SectionInformation.ConfigSource = configPath;
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
                if (webConfig.Sections[FastTrackConfiguration.ConfigurationName] != null)
                {
                    webConfig.Sections.Remove(FastTrackConfiguration.ConfigurationName);

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