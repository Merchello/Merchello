namespace Merchello.Core.Configuration.Outline 
{
    using System.Configuration;

    /// <summary>
    /// The currency format collection.
    /// </summary>
    public class CurrencyFormatCollection : ConfigurationElementCollection
    {

        /// <summary>
        /// Default. Returns the CurrencyFormatElement with the index of index from the collection
        /// </summary>
        public CurrencyFormatElement this[object index]
        {
            get { return (CurrencyFormatElement)this.BaseGet(index); }
        }

        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new CurrencyFormatElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CurrencyFormatElement)element).CurrencyCode;
        }
    }
}