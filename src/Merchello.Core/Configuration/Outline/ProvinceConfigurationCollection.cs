namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// The province configuration collection.
    /// </summary>
    public class ProvinceConfigurationCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProvinceElement();
        }

        /// <summary>
        /// Gets the code 'key' for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">
        /// The <see cref="TypeFieldElement"/>
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProvinceElement)element).Code;
        }

        /// <summary>
        /// Gets the <see cref="ProvinceElement"/> with the index of index from the collection
        /// </summary>
        public ProvinceElement this[object index]
        {
            get { return (ProvinceElement)this.BaseGet(index); }
        } 
    }
}