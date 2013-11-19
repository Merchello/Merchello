using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class ProvinceElement : ConfigurationElement
    {
        /// <summary>
        /// Gets/sets the province 'code' (key) for the province collection element.  Presumably this is an enum value.
        /// </summary>
        [ConfigurationProperty("code", IsKey = true)]
        public string Code
        {
            get { return (string)this["code"]; }
            set { this["code"] = value; }
        }

        /// <summary>
        /// Gets/sets the name associated with the province.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
         
    }
}