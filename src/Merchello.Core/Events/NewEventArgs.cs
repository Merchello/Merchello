namespace Merchello.Core.Events
{
    /// <summary>
    /// The new event args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object
    /// </typeparam>
    public class NewEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewEventArgs{T}"/> class. 
        /// Constructor accepting entities in a creating operation
        /// </summary>
        /// <param name="eventObject">
        /// The object associated with the event
        /// </param>
        public NewEventArgs(T eventObject)
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
