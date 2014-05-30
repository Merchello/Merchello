using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    /// <summary>
    /// Defines the ReplaceElement
    /// </summary>
    public class ReplaceElement : ConfigurationElement
    {

        /// <summary>
        /// Gets/sets the alias (key) value for the settings collection element.  
        /// </summary>
        [ConfigurationProperty("alias", IsKey = true)]
        public string Alias
        {
            get { return (string)this["alias"]; }
            set { this["alias"] = value; }
        }

        /// <summary>
        /// Gets/sets the pattern value for the settings collection element.  
        /// </summary>
        [ConfigurationProperty("pattern", IsRequired = true)]
        public string Pattern
        {
            get { return (string)this["pattern"]; }
            set { this["pattern"] = value; }
        }

        [ConfigurationProperty("replacement", IsRequired = false, DefaultValue = "")]
        public string Replacement
        {
            get { return (string)this["replacement"]; }
            set { this["replacement"] = value; }
        }

        [ConfigurationProperty("replacementInMonitor", IsRequired = false, DefaultValue = true)]
        public bool ReplacementInMonitor
        {
            get { return (bool)this["replacementInMonitor"]; }
            set { this["replacementInMonitor"] = value; }
        }
    }
}