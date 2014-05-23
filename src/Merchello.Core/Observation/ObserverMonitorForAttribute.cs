using System;

namespace Merchello.Core.Observation
{
    public class ObserverMonitorForAttribute : Attribute 
    {
        public Type ObservableTrigger { get; private set; }

        /// <summary>
        /// The name of the trigger
        /// </summary>
        public string Name { get; private set; }

        public ObserverMonitorForAttribute(Type observableTrigger, string name)
        {
            Mandate.ParameterNotNull(observableTrigger, "observableTrigger");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            ObservableTrigger = observableTrigger;
            Name = name;            
        }
    }
}