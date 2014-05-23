using System;
using System.Collections.Generic;
using Umbraco.Core.Events;

namespace Merchello.Core.Events
{
    /// <summary>
    /// Generic notifiy event args
    /// </summary>
    public class NotificationEventArgs : ObservationChannelEventArgs<object>
    {
        private readonly Guid _triggerKey;
        private readonly IEnumerable<string> _contacts; 

        public NotificationEventArgs(Guid triggerKey, object eventObject)
            : this(triggerKey, eventObject, new List<string>())
        { }

        public NotificationEventArgs(Guid triggerKey, object eventObject, IEnumerable<string> contacts)
            : this(triggerKey, eventObject, contacts, true)
        { }

        public NotificationEventArgs(Guid triggerKey, object eventObject, IEnumerable<string> contacts, bool canCancel)
            : base(eventObject, canCancel)
        {
            _triggerKey = triggerKey;
            _contacts = contacts;
        }

        public Guid TriggerKey 
        {
            get { return _triggerKey; }
        }

        public IEnumerable<string> Contacts 
        {
            get { return _contacts; }
        } 

    }
}