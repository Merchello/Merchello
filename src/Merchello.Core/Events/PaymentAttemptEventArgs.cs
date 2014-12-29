namespace Merchello.Core.Events
{
    using Umbraco.Core.Events;

    /// <summary>
    /// The payment attempt args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to return in the event args
    /// </typeparam>
    public class PaymentAttemptEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAttemptEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public PaymentAttemptEventArgs(T eventObject) 
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