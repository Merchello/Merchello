using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class RegionElement : ConfigurationElement
    {
        /// <summary>
        /// Gets/sets the region 'code' (key) for the region collection element.  Presumably this is an enum value.
        /// </summary>
        [ConfigurationProperty("code", IsKey = true)]
        public string Code
        {
            get { return (string)this["code"]; }
            set { this["code"] = value; }
        }

        /// <summary>
        /// Gets the province collection
        /// </summary>
        [ConfigurationProperty("provinces", IsRequired = true), ConfigurationCollection(typeof(ProvinceCollection), AddItemName = "province")]
        public ProvinceCollection Provinces
        {
            get { return (ProvinceCollection)this["provinces"]; }
        }
    }
}