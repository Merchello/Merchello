namespace Merchello.Web.PackageActions
{
    using System.Configuration;
    using System.Xml;
    using Core.Configuration;
    using umbraco.cms.businesslogic.packager.standardPackageActions;
    using umbraco.interfaces;

    /// <summary>
    /// Adds a key to the web.config app settings
    /// </summary>
    /// <remarks>
    /// 
    /// Modified verion of https://packageactioncontrib.codeplex.com/SourceControl/latest#PackageActionsContrib/AddAppConfigKey.cs
    /// 
    /// Original contribution from Paul Sterling
    /// </remarks>
    public class AddAppConfigKey : IPackageAction
    {
        #region IPackageAction Members

        /// <summary>
        /// The key.
        /// </summary>
        private const string Key = "merchelloConfigurationStatus";

        /// <summary>
        /// The alias.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Alias()
        {
            return string.Concat(MerchelloConfiguration.ApplicationName, "_AddAppConfigKey");
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="packageName">
        /// The package name.
        /// </param>
        /// <param name="xmlData">
        /// The xml data.
        /// </param>
        /// <returns>
        /// True or false inicating success.
        /// </returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {    
            try
            {
                CreateAppSettingsKey(Key, MerchelloVersion.Current.ToString());

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The sample xml.
        /// </summary>
        /// <returns>
        /// The <see cref="XmlNode"/>.
        /// </returns>
        public XmlNode SampleXml()
        {
            const string Sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"AddAppConfigKey\"></Action>";

            return helper.parseStringToXmlNode(Sample);
        }

        /// <summary>
        /// The undo.
        /// </summary>
        /// <param name="packageName">
        /// The package name.
        /// </param>
        /// <param name="xmlData">
        /// The xml data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Undo(string packageName, XmlNode xmlData)
        {
            try
            {
                RemoveAppSettingsKey(Key);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region helpers

        private static void CreateAppSettingsKey(string key, string value)
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings  = (AppSettingsSection)config.GetSection("appSettings");

            appSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
        }

        private static void RemoveAppSettingsKey(string key)
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings = (AppSettingsSection)config.GetSection("appSettings");

            appSettings.Settings.Remove(key);

            config.Save(ConfigurationSaveMode.Modified);
        }
        #endregion
    }
}