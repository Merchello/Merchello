using System.Configuration;
using System.Xml;
using Merchello.Core.Configuration;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace Merchello.Web.PackageActions
{

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

        private const string Key = "merchelloConfigurationStatus";

        public string Alias()
        {
            return string.Concat(MerchelloConfiguration.ApplicationName, "_AddAppConfigKey");
        }

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

        public XmlNode SampleXml()
        {
            const string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"AddAppConfigKey\"></Action>";
            return helper.parseStringToXmlNode(sample);
        }

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