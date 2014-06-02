
using System.Collections.Generic;
using Umbraco.Core.Events;

namespace Merchello.Core.Events
{
    public class StatusChangeEventArgs<T> : CancellableObjectEventArgs<IEnumerable<T>>
    {
        /// <summary>
        /// Constructor for accepting multiple entities that are used in the status changing operation
        /// </summary>
        /// <param name="eventObject"></param>
        public StatusChangeEventArgs(IEnumerable<T> eventObject)
            : base(eventObject)
        { }

        /// <summary>
        /// Constructor for accepting multiple entities that are used in the saving operation
        /// </summary>
        /// <param name="eventObject"></param>
        /// <param name="canCancel"></param>
        public StatusChangeEventArgs(IEnumerable<T> eventObject, bool canCancel)
            : base(eventObject, canCancel)
        { }

        /// <summary>
        /// Constructor accepting a single entity reference
        /// </summary
        public StatusChangeEventArgs(T eventObject)
            : base(new List<T> {eventObject})
        { }

        /// <summary>
        /// Constructor accepting a single entity reference
        /// </summary>
        public StatusChangeEventArgs(T eventObject, bool canCancel)
            : base(new List<T> { eventObject }, canCancel)
        { }

        public IEnumerable<T> StatusChangedEntities { get { return EventObject; } }
    }
}
