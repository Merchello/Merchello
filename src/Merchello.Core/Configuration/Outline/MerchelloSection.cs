namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// Defines the Merchello main configuration section.
    /// </summary>
    public class MerchelloSection : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets the default connection string name for Merchello database connectivity
        /// </summary>
        [ConfigurationProperty("defaultConnectionStringName", DefaultValue = "umbracoDbDSN", IsRequired = false)]
        public string DefaultConnectionStringName
        {
            get { return (string)this["defaultConnectionStringName"]; }
            set { this["defaultConnectionStringName"] = value; }
        }

        /// <summary>
        /// Gets or sets the default country code, primarily used for UI controls
        /// </summary>
        [ConfigurationProperty("defaultCountryCode", IsRequired = false)]
        public string DefaultCountryCode
        {
            get { return (string)this["defaultCountryCode"]; }
            set { this["defaultCountryCode"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether logging is enabled
        /// </summary>
        [ConfigurationProperty("enableLogging", DefaultValue = true, IsRequired = false)]
        public bool EnableLogging
        {
            get { return (bool)this["enableLogging"]; }
            set { this["enableLogging"] = value; }
        }

        /// <summary>
        /// Gets or sets the log localization defaults to "en"
        /// </summary>
        [ConfigurationProperty("logLocalization", DefaultValue = "en", IsRequired = false)]
        public string LogLocalization
        {
            get { return (string)this["logLocalization"]; }
            set { this["logLocalization"] = value; }
        }

        /// <summary>
        /// Gets the settings collection
        /// </summary>
        [ConfigurationProperty("settings", IsRequired = true), ConfigurationCollection(typeof(SettingsCollection), AddItemName = "setting")]
        public SettingsCollection Settings
        {
            get { return (SettingsCollection) this["settings"]; }
        }

        /// <summary>
        /// Gets the customer element.
        /// </summary>
        [ConfigurationProperty("customer", IsRequired = true)]
        public CustomerElement Customer
        {
            get
            {
                return (CustomerElement)this["customer"];
            }
        }

        /// <summary>
        /// Gets the type field definitions element
        /// </summary>
        [ConfigurationProperty("typeFieldDefinitions", IsRequired = true)]
        public TypeFieldDefinitionsElement TypeFields
        {
            get { return (TypeFieldDefinitionsElement) this["typeFieldDefinitions"];  }
        }

        /// <summary>
        /// Gets the RegionalProvince collection
        /// </summary>
        [ConfigurationProperty("regionalProvinces", IsRequired = true), ConfigurationCollection(typeof(RegionalProvinceCollection), AddItemName = "region")]
        public RegionalProvinceCollection RegionalProvinces
        {
            get { return (RegionalProvinceCollection) this["regionalProvinces"]; }
        }

        /// <summary>
        /// Gets the strategies collection
        /// </summary>
        [ConfigurationProperty("strategies", IsRequired = true), ConfigurationCollection(typeof(StrategiesCollection), AddItemName = "strategy")]
        public StrategiesCollection Strategies
        {
            get { return (StrategiesCollection)this["strategies"]; }
        }

        /// <summary>
        /// Gets the task chain collection
        /// </summary>
        [ConfigurationProperty("taskChains", IsRequired = true), ConfigurationCollection(typeof(TaskChainsCollection), AddItemName = "taskChain")]
        public TaskChainsCollection TaskChains
        {
            get { return (TaskChainsCollection)this["taskChains"]; }
        }

        /// <summary>
        /// Gets the tasks collection
        /// </summary>
        [ConfigurationProperty("patternFormatter", IsRequired = false), ConfigurationCollection(typeof(ReplaceElement), AddItemName = "replace")]
        public ReplacementCollection PatternFormatter
        {
            get { return (ReplacementCollection)this["patternFormatter"]; }
        }

        /// <summary>
        /// Gets the sub tree.
        /// </summary>
        [ConfigurationProperty("backoffice", IsRequired = true), ConfigurationCollection(typeof(TreeCollection), AddItemName = "tree")]
        public TreeCollection BackOffice
        {
            get { return (TreeCollection)this["backoffice"]; }
        }
    }
}
