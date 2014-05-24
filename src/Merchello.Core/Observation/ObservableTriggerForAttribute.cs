using System;

namespace Merchello.Core.Observation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ObservableTriggerForAttribute : Attribute
    {       
        /// <summary>
        /// The name of the trigger
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// The "area" or category of the trigger
        /// </summary>
        public ObservableTopic Area { get; private set; }

        // ctor
        public ObservableTriggerForAttribute(string alias, ObservableTopic area)
        {
                        
            Mandate.ParameterNotNullOrEmpty(alias, "alias");
            Alias = alias;
            Area = area;            
        }
    }
}