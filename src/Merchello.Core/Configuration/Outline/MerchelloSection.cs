using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    /// <summary>
    /// Defines Merchello's main configuration section.
    /// </summary>
    public class MerchelloSection : ConfigurationSection
    {
        /// <summary>
        /// Gets/Sets the default connectionstring name for Merchello database connectivity
        /// </summary>
        [ConfigurationProperty("defaultConnectionStringName", DefaultValue = "umbracoDbDSN", IsRequired = false)]
        public string DefaultConnectionStringName
        {
            get { return (string)this["defaultConnectionStringName"]; }
            set { this["defaultConnectionStringName"] = value; }
        }

        /// <summary>
        /// Gets/Sets teh default country code, primarily used for UI controls
        /// </summary>
        [ConfigurationProperty("defaultCountryCode", IsRequired = false)]
        public string DefaultCountryCode
        {
            get { return (string)this["defaultCountryCode"]; }
            set { this["defaultCountryCode"] = value; }
        }

        /// <summary>
        /// Gets/Sets the enableLogging property setting
        /// </summary>
        [ConfigurationProperty("enableLogging", DefaultValue = false, IsRequired = false)]
        public bool EnableLogging
        {
            get { return (bool)this["enableLogging"]; }
            set { this["enableLogging"] = value; }
        }

        /// <summary>
        /// Gets/Sets the Merchello Version
        /// </summary>
        [ConfigurationProperty("version", IsRequired = true)]
        public string Version
        {
           get { return (string) this["version"];  }
           set { this["version"] = value;  }
        }

        [ConfigurationProperty("typeFieldDefinitions", IsRequired = true)]
        public TypeFieldDefinitionsElement TypeFields
        {
            get { return (TypeFieldDefinitionsElement) this["typeFieldDefinitions"];  }
        }


    }
}
