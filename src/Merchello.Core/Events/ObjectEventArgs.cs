namespace Merchello.Core.Events
{
    using System;

    /// <summary>
    /// The object event args base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object
    /// </typeparam>
    public class ObjectEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public ObjectEventArgs(T eventObject)
        {
            this.EventObject = eventObject;
        }

        /// <summary>
        /// Gets the event object.
        /// </summary>
        protected T EventObject { get; private set; }

    }
}