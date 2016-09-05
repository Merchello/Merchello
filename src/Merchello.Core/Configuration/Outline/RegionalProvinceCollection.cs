using System.Configuration;

namespace Merchello.Core.Configuration.Outline 
{
    public class RegionalProvinceCollection  : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RegionElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RegionElement)element).Code;
        }

        /// <summary>
        /// Default. Returns the RegionElement with the index of index from the collection
        /// </summary>
        public RegionElement this[object index]
        {
            get { return (RegionElement)this.BaseGet(index); }
        }
    }
}


