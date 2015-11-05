namespace Merchello.Core.Configuration.Outline
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// The settings collection.
    /// </summary>
    public class SettingsCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default indexer
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="SettingsElement"/>.
        /// </returns>
        public SettingsElement this[object index]
        {
            get { return (SettingsElement)this.BaseGet(index); }
        }

        /// <summary>
        /// The get trees.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="SettingsElement"/>.
        /// </returns>
        public IEnumerable<SettingsElement> AllSettings()
        {
            return this.Cast<SettingsElement>();
        }


        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SettingsElement();
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
            return ((SettingsElement)element).Alias;
        }
    }
}
