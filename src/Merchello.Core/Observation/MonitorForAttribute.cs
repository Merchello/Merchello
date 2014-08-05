namespace Merchello.Core.Observation
{
    using System;

    public class MonitorForAttribute : Attribute 
    {
        public MonitorForAttribute(string key, Type observableTrigger, string name)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNull(observableTrigger, "observableTrigger");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            Key = new Guid(key);
            ObservableTrigger = observableTrigger;
            Name = name;            
        }

        /// <summary>
        /// Gets the observable trigger
        /// </summary>
        public Type ObservableTrigger { get; private set; }

        /// <summary>
        /// Gets the mandatory key for the monitor
        /// </summary>        
        public Guid Key { get; private set; }

        /// <summary>
        /// Gets the name of the monitor
        /// </summary>
        public string Name { get; private set; }
    }
}