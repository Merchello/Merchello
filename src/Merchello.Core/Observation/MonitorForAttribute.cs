using System;

namespace Merchello.Core.Observation
{
    public class MonitorForAttribute : Attribute 
    {
        public Type ObservableTrigger { get; private set; }

        /// <summary>
        /// Gets the mandatory key for the monitor
        /// </summary>        
        public Guid Key { get; private set; }

        /// <summary>
        /// The name of the trigger
        /// </summary>
        public string Name { get; private set; }

        public MonitorForAttribute(string key, Type observableTrigger, string name)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNull(observableTrigger, "observableTrigger");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            Key = new Guid(key);
            ObservableTrigger = observableTrigger;
            Name = name;            
        }
    }
}