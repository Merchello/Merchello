namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The region element.
    /// </summary>
    public class RegionElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the region 'code' (key) for the region collection element.
        /// </summary>
        [ConfigurationProperty("code", IsKey = true)]
        public string Code
        {
            get { return (string)this["code"]; }
            set { this["code"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the region requires a postal code.
        /// </summary>
        [ConfigurationProperty("requirePostalCode", DefaultValue = false, IsRequired = false)]
        public bool RequirePostalCode
        {
            get { return (bool)this["requirePostalCode"]; }
            set { this["requirePostalCode"] = value; }
        }


        /// <summary>
        /// Gets or sets the label for provinces (for US this may be 'States')
        /// </summary>
        [ConfigurationProperty("provinceLabel", DefaultValue = "Provinces", IsRequired = false)]
        public string ProvinceLabel
        {
            get { return (string) this["provinceLabel"]; }
            set { this["provinceLabel"] = value; }
        }

        /// <summary>
        /// Gets the province collection
        /// </summary>
        [ConfigurationProperty("provinces", IsRequired = true), ConfigurationCollection(typeof(ProvinceConfigurationCollection), AddItemName = "province")]
        public ProvinceConfigurationCollection ProvincesConfiguration
        {
            get { return (ProvinceConfigurationCollection)this["provinces"]; }
        }
    }
}