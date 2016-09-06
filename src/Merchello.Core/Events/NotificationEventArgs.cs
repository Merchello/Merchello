namespace Merchello.Core.Events
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Generic notify event args
    /// </summary>
    public class NotificationEventArgs : ObservationChannelEventArgs<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        /// <param name="triggerKey">
        /// The trigger key.
        /// </param>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public NotificationEventArgs(Guid triggerKey, object eventObject)
            : this(triggerKey, eventObject, new List<string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        /// <param name="triggerKey">
        /// The trigger key.
        /// </param>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        public NotificationEventArgs(Guid triggerKey, object eventObject, IEnumerable<string> contacts)
            : this(triggerKey, eventObject, contacts, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        /// <param name="triggerKey">
        /// The trigger key.
        /// </param>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        /// <param name="canCancel">
        /// The can cancel.
        /// </param>
        public NotificationEventArgs(Guid triggerKey, object eventObject, IEnumerable<string> contacts, bool canCancel)
            : base(eventObject, canCancel)
        {
            this.TriggerKey = triggerKey;
            this.Contacts = contacts;
        }

        /// <summary>
        /// Gets the notification trigger key.
        /// </summary>
        public Guid TriggerKey { get; }

        /// <summary>
        /// Gets the contacts.
        /// </summary>
        public IEnumerable<string> Contacts { get; }
    }
}