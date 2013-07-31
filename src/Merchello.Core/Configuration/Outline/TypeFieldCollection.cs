using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Configuration.Outline
{
    public class TypeFieldCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="TypeFieldElement">ConfigurationElement</see>.
        /// CreateNewElement must be overridden in classes that derive from the ConfigurationElementCollection class.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TypeFieldElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element"><see cref="TypeFieldElement"/></param>
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
