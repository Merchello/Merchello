using System.Collections.Generic;
using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class TaskChainsCollection: ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskChainElement();
        }

        /// <summary>
        /// Gets the code 'key' for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element"><see cref="TypeFieldElement"/></param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TaskChainElement)element).Alias;
        }
 

        /// <summary>
        /// Default. Returns the ProvinceElement with the index of index from the collection
        /// </summary>
        public TaskChainElement this[object index]
        {
            get { return (TaskChainElement)BaseGet(index); }
        } 
         
    }
}