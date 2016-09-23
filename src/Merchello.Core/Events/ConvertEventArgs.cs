namespace Merchello.Core.Events
{
    using System.Collections.Generic;

    /// <summary>
    /// EventArgs for Customer conversion
    /// </summary>
    /// <typeparam name="T">
    /// The type of the event argument
    /// </typeparam>
    public class ConvertEventArgs<T> : CancellableObjectEventArgs<IEnumerable<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public ConvertEventArgs(T eventObject)
            : base(new List<T> { eventObject })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="canCancel">
        /// The can cancel.
        /// </param>
        public ConvertEventArgs(T eventObject, bool canCancel)
            : base(new List<T> { eventObject }, canCancel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public ConvertEventArgs(IEnumerable<T> eventObject)
            : base(eventObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="canCancel">
        /// The can cancel.
        /// </param>
        public ConvertEventArgs(IEnumerable<T> eventObject, bool canCancel)
            : base(eventObject, canCancel)
        {
        }

        /// <summary>
        /// Gets the converted entities.
        /// </summary>
        public IEnumerable<T> CovertedEntities
        {
            get
            {
                return EventObject;
            }
        }
    }
}
