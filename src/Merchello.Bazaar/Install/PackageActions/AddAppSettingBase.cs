namespace Merchello.Bazaar.Install.PackageActions
{
    using System.Configuration;

    /// <summary>
    /// Base class for package actions that add/remove App_Settings.
    /// </summary>
    public abstract class AddAppSettingBase
    {
        #region helpers

        /// <summary>
        /// Adds the App_Setting.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        protected static void CreateAppSettingsKey(string key, string value)
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings = (AppSettingsSection)config.GetSection("appSettings");

            appSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Removes the App_Setting.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        protected static void RemoveAppSettingsKey(string key)
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings = (AppSettingsSection)config.GetSection("appSettings");

            appSettings.Settings.Remove(key);

            config.Save(ConfigurationSaveMode.Modified);
        }
        #endregion
    }
}