using System.Configuration;

namespace Merchello.Core.Configuration.Outline
{
    /// <summary>
    /// Defines a task chain task configuration element
    /// </summary>
    public class TaskElement : ConfigurationElement
    {
        ///// <summary>
        ///// Gets/sets the 'alias' (key) for the <see cref="TasksConfigurationCollection "/> element. 
        ///// </summary>
        //[ConfigurationProperty("alias", IsKey = true)]
        //public string Alias
        //{
        //    get { return (string)this["alias"]; }
        //    set { this["alias"] = value; }
        //}

        /// <summary>
        /// The type of the task to instantiate
        /// </summary>
        [ConfigurationProperty("type")]
        public string Type
        {
            get { return (string) this["type"]; }
            set { this["type"] = value; }
        }
    }
}