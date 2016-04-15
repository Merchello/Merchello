namespace Merchello.Core.Events
{
    using System;

    /// <summary>
    /// The object event args base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object
    /// </typeparam>
    public class ObjectEventArgsBase<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectEventArgsBase{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        protected ObjectEventArgsBase(T eventObject)
        {
            this.EventObject = eventObject;
        }

        /// <summary>
        /// Gets the event object.
        /// </summary>
        protected T EventObject { get; private set; }

    }
}