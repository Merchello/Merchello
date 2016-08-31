namespace Merchello.FastTrack.Ui
{
    using System.Configuration;
    using Core.Configuration.Outline;

    /// <summary>
    /// Defines the FastTrack main configuration section
    /// </summary>
    public class FastTrackSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the settings collection
        /// </summary>
        [ConfigurationProperty("settings", IsRequired = true), ConfigurationCollection(typeof(SettingsCollection), AddItemName = "setting")]
        public SettingsCollection Settings
        {
            get { return (SettingsCollection)this["settings"]; }
        }
    }
}
