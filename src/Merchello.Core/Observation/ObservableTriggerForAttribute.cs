using System;

namespace Merchello.Core.Observation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ObservableTriggerForAttribute : Attribute
    {
        /// <summary>
        /// Gets the mandatory key for the trigger
        /// </summary>        
        public Guid Key { get; private set; }
       

        /// <summary>
        /// The name of the trigger
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// The "area" or category of the trigger
        /// </summary>
        public ObservableTopic Area { get; private set; }

        // ctor
        public ObservableTriggerForAttribute(string key, string alias, ObservableTopic area)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            
            Mandate.ParameterNotNullOrEmpty(alias, "alias");

            Key = new Guid(key);
            Alias = alias;
            Area = area;            
        }
    }
}