namespace Merchello.Core.Events
{
    using Umbraco.Core.Events;

    /// <summary>
    /// The sales preparation event args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to return in the event args
    /// </typeparam>
    public class SalesPreparationEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesPreparationEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public SalesPreparationEventArgs(T eventObject) 
            : base(eventObject, true)
        {
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public T Entity
        {
            get { return EventObject; }
        }
    }
}