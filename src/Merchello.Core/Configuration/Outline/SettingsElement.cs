namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The settings element.
    /// </summary>
    public class SettingsElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the alias (key) value for the settings collection element.  Presumably this is an enumerated value.
        /// </summary>
        [ConfigurationProperty("alias", IsKey = true)]
        public string Alias
        {
            get { return (string)this["alias"]; }
            set { this["alias"] = value; }
        }

        /// <summary>
        /// Gets or sets the value associated with the setting.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
