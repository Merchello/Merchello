using System;

namespace Merchello.Core.Observation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TriggerForAttribute : Attribute
    {       
        /// <summary>
        /// The name of the trigger
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// The "area" or category of the trigger
        /// </summary>
        public Topic Topic { get; private set; }

        /// <summary>
        /// The type to defines the event to be handled
        /// </summary>
        /// <remarks>
        /// Placeholder for "auto triggers" 
        /// </remarks>
        internal Type Type { get; private set; }

        /// <summary>
        /// The name of the event to be handled
        /// </summary>
        /// <remarks>
        /// Placeholder for "auto triggers" 
        /// </remarks>
        internal string HandleEvent { get; private set; }

        // ctor
        public TriggerForAttribute(string alias)
            : this(alias, Topic.Notifications, null, null)
        { }

        // ctor
        public TriggerForAttribute(string alias, Topic topic)
            : this(alias, topic, null, null)
        { }

        // ctor
        internal TriggerForAttribute(string alias, Topic topic, Type type, string handleEvent)
        {
            Mandate.ParameterNotNullOrEmpty(alias, "alias");
           
            Alias = alias;
            Topic = topic;
            Type = type;
            HandleEvent = handleEvent;
        }
    }
}