using System.Collections.Generic;
using Umbraco.Core.Events;

namespace Merchello.Core.Events
{
    /// <summary>
    /// Generic notifiy event args
    /// </summary>
    public class NotificationEventArgs : BroadcastEventArgs<object>
    {

        private readonly IEnumerable<string> _contacts; 

        public NotificationEventArgs(object eventObject)
            : this(eventObject, new List<string>())
        { }

        public NotificationEventArgs(object eventObject, IEnumerable<string> contacts)
            : this(eventObject, contacts, true)
        { }

        public NotificationEventArgs(object eventObject, IEnumerable<string> contacts, bool canCancel)
            : base(eventObject, canCancel)
        {
            _contacts = contacts;
        }

        public IEnumerable<string> Contacts 
        {
            get { return _contacts; }
        } 

    }
}