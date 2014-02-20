using System.Collections.Generic;
using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class TasksConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TaskElement) element).Type;
        }

        public IEnumerable<TaskElement> GetAllTypes()
        {
            foreach (ConfigurationElement type in this)
            {
                yield return type as TaskElement;
            }
        }

        /// <summary>
        /// Default. Returns the ProvinceElement with the index of index from the collection
        /// </summary>
        public TaskElement this[object index]
        {
            get { return (TaskElement)this.BaseGet(index); }
        }
    }
}