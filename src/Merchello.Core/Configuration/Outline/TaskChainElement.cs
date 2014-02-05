using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    public class TaskChainElement : ConfigurationElement
    {
        /// <summary>
        /// Gets/sets the 'alias' (key) for the <see cref="TaskChainsCollection"/> element. 
        /// </summary>
        [ConfigurationProperty("alias", IsKey = true)]
        public string Alias
        {
            get { return (string)this["alias"]; }
            set { this["alias"] = value; }
        }

        /// <summary>
        /// Gets the tasks collection
        /// </summary>
        [ConfigurationProperty("tasks", IsRequired = true), ConfigurationCollection(typeof(TasksConfigurationCollection), AddItemName = "task")]
        public TasksConfigurationCollection TaskConfigurationCollection
        {
            get { return (TasksConfigurationCollection)this["tasks"]; }
        }
    }
}