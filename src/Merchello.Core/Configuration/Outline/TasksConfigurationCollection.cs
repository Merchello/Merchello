namespace Merchello.Core.Configuration.Outline
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// The tasks configuration collection.
    /// </summary>
    public class TasksConfigurationCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default. Returns the ProvinceElement with the index of index from the collection
        /// </summary>
        public TaskElement this[object index]
        {
            get { return (TaskElement)this.BaseGet(index); }
        }

        /// <summary>
        /// The get all types.
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="TaskElement"/>.
        /// </returns>
        public IEnumerable<TaskElement> GetAllTypes()
        {
            foreach (ConfigurationElement type in this)
            {
                yield return type as TaskElement;
            }
        }

        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskElement();
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
            return ((TaskElement) element).Type;
        }
    }
}