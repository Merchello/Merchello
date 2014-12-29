namespace Merchello.Core.Configuration.Outline
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Defines a PatternElement
    /// </summary>
    public class ReplacementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default. Returns the ReplacementElement with the index of index from the collection
        /// </summary>
        public ReplaceElement this[object index]
        {
            get { return (ReplaceElement)BaseGet(index); }
        }

        /// <summary>
        /// The get replacements.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ReplaceElement> GetReplacements()
        {
            return this.Cast<ReplaceElement>();
        }

        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ReplaceElement();
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
            return ((ReplaceElement) element).Alias;

        }
    }
}