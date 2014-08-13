namespace Merchello.Core.Configuration.Outline
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// The back office collection
    /// </summary>
    public class TreeCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="TreeElement"/>.
        /// </returns>
        public TreeElement this[object index]
        {
            get { return (TreeElement)this.BaseGet(index); }
        }

        /// <summary>
        /// The get trees.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="TreeElement"/>.
        /// </returns>
        public IEnumerable<TreeElement> GetTrees()
        {
            return this.Cast<TreeElement>();
        }

        /// <summary>
        /// Adds an element to the collection
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        internal void Add(TreeElement element)
        {
            BaseAdd(element);
        }

        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TreeElement();
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
            return ((TreeElement)element).Id;
        }
    }
}