using System;

namespace Merchello.Core.Observation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ObservationChannelForAttribute : Attribute
    {
        /// <summary>
        /// Gets the mandatory key for the trigger
        /// </summary>        
        public Guid Key { get; private set; }
       
        /// <summary>
        /// The "area" or category of the trigger
        /// </summary>
        public ObservationChannelType Area { get; private set; }

        /// <summary>
        /// The name of the trigger
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The name of the broadcaster
        /// </summary>
        public string Alias { get; private set; }

        // ctor
        public ObservationChannelForAttribute(string key, string name, string alias, ObservationChannelType area)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(alias, "alias");

            Key = new Guid(key);
            Alias = alias;
            Name = name;
            Area = area;            
        }
    }
}