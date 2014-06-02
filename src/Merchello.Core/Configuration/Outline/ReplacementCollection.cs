using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Merchello.Core.Configuration.Outline
{
    /// <summary>
    /// Defines a PatternElement
    /// </summary>
    public class ReplacementCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new ReplaceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ReplaceElement) element).Alias;

        }
        /// <summary>
        /// Default. Returns the ReplacementElement with the index of index from the collection
        /// </summary>
        public ReplaceElement this[object index]
        {
            get { return (ReplaceElement)BaseGet(index); }
        }

        public IEnumerable<ReplaceElement> GetReplacements()
        {
            return this.Cast<ReplaceElement>();
        }
    }
}