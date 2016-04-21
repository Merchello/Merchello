namespace Merchello.Core.Observation
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// Decorates notification monitors.
    /// </summary>
    public class MonitorForAttribute : Attribute 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorForAttribute"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="observableTrigger">
        /// The observable trigger.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="useCodeEditor">
        /// The use Code Editor.
        /// </param>
        public MonitorForAttribute(string key, Type observableTrigger, string name, bool useCodeEditor = false)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNull(observableTrigger, "observableTrigger");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            Key = new Guid(key);
            ObservableTrigger = observableTrigger;
            Name = name;
            this.UseCodeEditor = useCodeEditor;
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

        /// <summary>
        /// Gets the route path.
        /// </summary>
        public bool UseCodeEditor { get; private set; }
    }
}