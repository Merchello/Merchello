using System;

namespace Merchello.Core.Triggers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class TriggerForAttribute : Attribute
    {
        /// <summary>
        /// Gets the mandatory key for the trigger
        /// </summary>        
        public Guid Key { get; private set; }

        /// <summary>
        /// The name of the trigger
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The "area" or category of the trigger
        /// </summary>
        public string Area { get; private set; }

        /// <summary>
        /// The type to defines the event to be handled
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The name of the event to be handled
        /// </summary>
        public string HandleEvent { get; private set; }

        // ctor
        public TriggerForAttribute(string key, string name, string area, Type type, string handleEvent)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(area, "area");
            Mandate.ParameterNotNull(type, "type");
            Mandate.ParameterNotNullOrEmpty(handleEvent, "handleEvent");

            Key = new Guid(key);
            Name = name;
            Area = area;
            Type = type;
            HandleEvent = handleEvent;
        }

    }
}