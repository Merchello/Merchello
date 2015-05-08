namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;

    /// <summary>
    /// A configuration collection for pluggable objects.
    /// </summary>
    public class PluggableCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Finds an object in the collection with the index (key alias) provided
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="PluggableObjectElement"/>.
        /// </returns>
        public PluggableObjectElement this[object index]
        {
            get { return (PluggableObjectElement)this.BaseGet(index); }
        }

        /// <summary>
        /// Creates a new PluggableObjectElement
        /// </summary>
        /// <returns>
        /// The <see cref="PluggableObjectElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new PluggableObjectElement();
        }

        /// <summary>
        /// Returns the alias (key) of the configuration element.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PluggableObjectElement)element).Alias;
        }
    }
}