namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// Defines the ReplaceElement
    /// </summary>
    public class ReplaceElement : ConfigurationElement
    {

        /// <summary>
        /// Gets or sets the alias (key) value for the settings collection element.  
        /// </summary>
        [ConfigurationProperty("alias", IsKey = true)]
        public string Alias
        {
            get { return (string)this["alias"]; }
            set { this["alias"] = value; }
        }

        /// <summary>
        /// Gets or sets the pattern value for the settings collection element.  
        /// </summary>
        [ConfigurationProperty("pattern", IsRequired = true)]
        public string Pattern
        {
            get { return (string)this["pattern"]; }
            set { this["pattern"] = value; }
        }

        /// <summary>
        /// Gets or sets the replacement.
        /// </summary>
        [ConfigurationProperty("replacement", IsRequired = false, DefaultValue = "")]
        public string Replacement
        {
            get { return (string)this["replacement"]; }
            set { this["replacement"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether replacement in monitor.
        /// </summary>
        [ConfigurationProperty("replacementInMonitor", IsRequired = false, DefaultValue = true)]
        public bool ReplacementInMonitor
        {
            get { return (bool)this["replacementInMonitor"]; }
            set { this["replacementInMonitor"] = value; }
        }
    }
}