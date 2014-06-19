namespace Merchello.Core.Observation
{
    using System;
    using Gateways.Notification.Triggering;

    /// <summary>
    /// Represents the TriggerForAttribute used to decorate <see cref="INotificationTrigger"/>s
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TriggerForAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerForAttribute"/> class.
        /// </summary>
        /// <param name="alias">The alias</param>
        public TriggerForAttribute(string alias)
            : this(alias, Topic.Notifications, null, null)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerForAttribute"/> class.
        /// </summary>
        /// <param name="alias">The alias</param>
        /// <param name="topic">The <see cref="Topic"/></param>
        public TriggerForAttribute(string alias, Topic topic)
            : this(alias, topic, null, null)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerForAttribute"/> class.
        /// </summary>
        /// <param name="alias">The alias</param>
        /// <param name="topic">The <see cref="Topic"/></param>
        /// <param name="type">The type</param>
        /// <param name="handleEvent">The event to be handled</param>
        internal TriggerForAttribute(string alias, Topic topic, Type type, string handleEvent)
        {
            Mandate.ParameterNotNullOrEmpty(alias, "alias");
           
            Alias = alias;
            Topic = topic;
            Type = type;
            HandleEvent = handleEvent;
        }

        /// <summary>
        /// Gets the alias of the trigger
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the <see cref="Topic"/> or category of the trigger
        /// </summary>
        public Topic Topic { get; private set; }

        /// <summary>
        /// Gets the type to define the event to be handled
        /// </summary>
        /// <remarks>
        /// Placeholder for "auto triggers" 
        /// </remarks>
        internal Type Type { get; private set; }

        /// <summary>
        /// Gets the name of the event to be handled
        /// </summary>
        /// <remarks>
        /// Placeholder for "auto triggers" 
        /// </remarks>
        internal string HandleEvent { get; private set; }
    }
}