using System.Collections.Generic;
using Umbraco.Core.Events;

namespace Merchello.Core.Events
{
    /// <summary>
    /// Defines ObservationChannal event args
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservationChannelEventArgs<T> : CancellableObjectEventArgs<T>
    {
        public ObservationChannelEventArgs(T eventObject)
            : base(eventObject)
        { }

        public ObservationChannelEventArgs(T eventObject, bool canCancel)
            : base(eventObject, canCancel)
        { }

        /// <summary>
        /// The entity to be observered
        /// </summary>
        public T ObservationEntity { get { return EventObject; } }
    }
}