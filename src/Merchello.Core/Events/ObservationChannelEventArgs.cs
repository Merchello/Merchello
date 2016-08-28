namespace Merchello.Core.Events
{
    /// <summary>
    /// Defines ObservationChannel event args
    /// </summary>
    /// <typeparam name="T">The type of the event argument</typeparam>
    public class ObservationChannelEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservationChannelEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public ObservationChannelEventArgs(T eventObject)
            : base(eventObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservationChannelEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="canCancel">
        /// The can cancel.
        /// </param>
        public ObservationChannelEventArgs(T eventObject, bool canCancel)
            : base(eventObject, canCancel)
        {
        }

        /// <summary>
        /// Gets the entity to be observed
        /// </summary>
        public T ObservationEntity
        {
            get
            {
                return EventObject;
            }
        }
    }
}