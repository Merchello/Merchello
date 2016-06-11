namespace Merchello.Core.Events
{
    /// <summary>
    /// The initializing event args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of argument
    /// </typeparam>
    public class InitializingEventArgs<T> : ObjectEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializingEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public InitializingEventArgs(T eventObject)
            : base(eventObject)
        {
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public T Entity
        {
            get
            {
                return EventObject;
            }
        }
    }
}