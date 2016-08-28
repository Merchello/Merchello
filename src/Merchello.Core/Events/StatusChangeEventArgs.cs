namespace Merchello.Core.Events
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the status changed event args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the event argument
    /// </typeparam>
    public class StatusChangeEventArgs<T> : CancellableObjectEventArgs<IEnumerable<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusChangeEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public StatusChangeEventArgs(IEnumerable<T> eventObject)
            : base(eventObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusChangeEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="canCancel">
        /// The can cancel.
        /// </param>
        public StatusChangeEventArgs(IEnumerable<T> eventObject, bool canCancel)
            : base(eventObject, canCancel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusChangeEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public StatusChangeEventArgs(T eventObject)
            : base(new List<T> { eventObject })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusChangeEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="canCancel">
        /// The can cancel.
        /// </param>
        public StatusChangeEventArgs(T eventObject, bool canCancel)
            : base(new List<T> { eventObject }, canCancel)
        {
        }

        /// <summary>
        /// Gets the status changed entities.
        /// </summary>
        public IEnumerable<T> StatusChangedEntities
        {
            get
            {
                return EventObject;
            }
        }
    }
}
