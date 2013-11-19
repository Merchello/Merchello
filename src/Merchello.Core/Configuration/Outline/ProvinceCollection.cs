using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class ProvinceCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProvinceElement();
        }

        /// <summary>
        /// Gets the code 'key' for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element"><see cref="TypeFieldElement"/></param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProvinceElement)element).Code;
        }

        /// <summary>
        /// Default. Returns the ProvinceElement with the index of index from the collection
        /// </summary>
        public ProvinceElement this[object index]
        {
            get { return (ProvinceElement)this.BaseGet(index); }
        } 
    }
}