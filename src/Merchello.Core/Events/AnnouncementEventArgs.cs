using Umbraco.Core.Events;

namespace Merchello.Core.Events
{
    /// <summary>
    /// Generic announcement event args
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AnnouncementEventArgs<T> : CancellableObjectEventArgs<T>
    {
        public AnnouncementEventArgs(T eventObject, bool canCancel) 
            : base(eventObject, canCancel)
        { }

        public AnnouncementEventArgs(T eventObject) 
            : base(eventObject)
        { }

        /// <summary>
        /// The entity to be announced
        /// </summary>
        public T AnnouncementEntity { get { return EventObject; } }
    }
}