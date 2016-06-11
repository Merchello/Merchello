namespace Merchello.Core.Events
{
    /// <summary>
    /// The new event args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object
    /// </typeparam>
    public class InitializedEventArgs<T> : ObjectEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializedEventArgs{T}"/> class. 
        /// Constructor accepting entities in a creating operation
        /// </summary>
        /// <param name="eventObject">
        /// The object associated with the event
        /// </param>
        public InitializedEventArgs(T eventObject)
            : base(eventObject)
        {
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public T Entity
        {
            get { return this.EventObject; }
        }
    }
}
