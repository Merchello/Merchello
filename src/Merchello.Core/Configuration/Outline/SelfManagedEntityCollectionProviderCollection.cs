namespace Merchello.Core.Configuration.Outline
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// The self managed entity collection provider collection.
    /// </summary>
    public class SelfManagedEntityCollectionProviderCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderElement"/>.
        /// </returns>
        public EntityCollectionProviderElement this[object index]
        {
            get { return (EntityCollectionProviderElement)this.BaseGet(index); }
        }

        /// <summary>
        /// The get trees.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="TreeElement"/>.
        /// </returns>
        public IEnumerable<EntityCollectionProviderElement> EntityCollectionProviders()
        {
            return this.Cast<EntityCollectionProviderElement>();
        }

        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new EntityCollectionProviderElement();
        }

        /// <summary>
        /// The get element key.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EntityCollectionProviderElement)element).Key;
        }
    }
}