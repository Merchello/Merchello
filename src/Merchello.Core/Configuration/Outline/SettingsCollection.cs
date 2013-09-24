using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class SettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SettingsElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element"><see cref="TypeFieldElement"/></param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SettingsElement)element).Alias;
        }

        /// <summary>
        /// Default. Returns the SettingsItemsElement with the index of index from the collection
        /// </summary>
        public SettingsElement this[object index]
        {
            get { return (SettingsElement)this.BaseGet(index); }
        }
    }
}
