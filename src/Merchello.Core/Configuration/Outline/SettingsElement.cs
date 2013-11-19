using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class SettingsElement : ConfigurationElement
    {
        /// <summary>
        /// Gets/sets the alias (key) value for the settings collection element.  Presumably this is an enum value.
        /// </summary>
        [ConfigurationProperty("alias", IsKey = true)]
        public string Alias
        {
            get { return (string)this["alias"]; }
            set { this["alias"] = value; }
        }

        /// <summary>
        /// Gets/sets the value associated with the setting.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
