namespace Merchello.Core.Configuration.Outline
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Routing;

    /// <summary>
    /// The content finder culture collection.
    /// </summary>
    public class ContentFinderCultureCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets the <see cref="RouteElement"/> with the index of index from the collection
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="RouteElement"/>.
        /// </returns>
        public RouteElement this[object index]
        {
            get { return (RouteElement)this.BaseGet(index); }
        }

        /// <summary>
        /// The get trees.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="TreeElement"/>.
        /// </returns>
        public IEnumerable<RouteElement> Routes()
        {
            return this.Cast<RouteElement>();
        }

        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new RouteElement();
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
            return ((RouteElement)element).CultureName;
        }
    }
}
