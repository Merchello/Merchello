namespace Merchello.Bazaar.Install.PackageActions
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;

    using umbraco.cms.businesslogic.packager.standardPackageActions;
    using umbraco.interfaces;

    /// <summary>
    /// The add xpath to store config key.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class AddXpathToStoreConfigKey : IPackageAction
    {
        #region IPackageAction Members

        /// <summary>
        /// The key.
        /// </summary>
        private const string Key = "Bazaar:XpathToStore";

        /// <summary>
        /// The alias.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Alias()
        {
            return "MerchelloBazaar_AddXpathToStoreConfigKey";
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
                CreateAppSettingsKey(Key, "//root/BazaarStore");

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
            const string Sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"MerchelloBazaar_AddXpathToStoreConfigKey\"></Action>";

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
            var appSettings = (AppSettingsSection)config.GetSection("appSettings");

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