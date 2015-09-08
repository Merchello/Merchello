using System.Configuration;

namespace Merchello.Core.Configuration.Outline 
{
    public class CurrencyFormatCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CurrencyFormatElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CurrencyFormatElement)element).CurrencyCode;
        }

        /// <summary>
        /// Default. Returns the CurrencyFormatElement with the index of index from the collection
        /// </summary>
        public CurrencyFormatElement this[object index]
        {
            get { return (CurrencyFormatElement)this.BaseGet(index); }
        }
    }
}