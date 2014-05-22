using System.Collections.Generic;
using Umbraco.Core.Events;

namespace Merchello.Core.Events
{
    /// <summary>
    /// Generic announcement event args
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BroadcastEventArgs<T> : CancellableObjectEventArgs<T>
    {
        public BroadcastEventArgs(T eventObject)
            : base(eventObject)
        { }

        public BroadcastEventArgs(T eventObject, bool canCancel)
            : base(eventObject, canCancel)
        { }

        /// <summary>
        /// The entity to be announced
        /// </summary>
        public T BroadcastEntity { get { return EventObject; } }
    }
}