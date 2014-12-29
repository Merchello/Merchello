namespace Merchello.Core.Configuration.Outline
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// The type field collection.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class TypeFieldCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="TypeFieldElement">ConfigurationElement</see>.
        /// CreateNewElement must be overridden in classes that derive from the ConfigurationElementCollection class.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TypeFieldElement();
        }
        /// <summary>
        /// The gets the collection of type fields.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{TypeFieldElement}"/>.
        /// </returns>
        public IEnumerable<TypeFieldElement> GetTypeFields()
        {
            return this.Cast<TypeFieldElement>();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">
        /// The <see cref="TypeFieldElement"/>
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TypeFieldElement)element).Alias;
        }

        /// <summary>
        /// Default. Returns the DbTypeFieldElement with the index of index from the collection
        /// </summary>
        public TypeFieldElement this[object index]
        {
            get { return (TypeFieldElement)this.BaseGet(index); }
        }
    }
}
