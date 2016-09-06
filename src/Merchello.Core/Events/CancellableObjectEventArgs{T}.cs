namespace Merchello.Core.Events
{
    using System.Collections.Generic;
    using System.Security.Permissions;

    /// <summary>
    /// Event args for a strongly typed object that can support cancellation
    /// </summary>
    /// <typeparam name="T">The type of the event argument</typeparam>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Events/CancellableObjectEventArgs.cs"/>
    [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
    public class CancellableObjectEventArgs<T> : CancellableEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableObjectEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="canCancel">
        /// A value indicating whether or not the instance can be cancelled.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <param name="additionalData">
        /// The additional data.
        /// </param>
        public CancellableObjectEventArgs(T eventObject, bool canCancel, EventMessages messages, IDictionary<string, object> additionalData)
            : base(canCancel, messages, additionalData)
        {
            EventObject = eventObject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableObjectEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="canCancel">
        /// A value indicating whether or not the instance can be cancelled.
        /// </param>
        /// <param name="eventMessages">
        /// The event messages.
        /// </param>
        public CancellableObjectEventArgs(T eventObject, bool canCancel, EventMessages eventMessages)
            : base(canCancel, eventMessages)
        {
            EventObject = eventObject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableObjectEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="eventMessages">
        /// The event messages.
        /// </param>
        public CancellableObjectEventArgs(T eventObject, EventMessages eventMessages)
            : this(eventObject, true, eventMessages)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableObjectEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <param name="canCancel">
        /// A value indicating whether or not the instance can be cancelled.
        /// </param>
        public CancellableObjectEventArgs(T eventObject, bool canCancel)
            : base(canCancel)
        {
            EventObject = eventObject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableObjectEventArgs{T}"/> class.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        public CancellableObjectEventArgs(T eventObject)
            : this(eventObject, true)
        {
        }

        /// <summary>
        /// Gets or sets the object relating to the event
        /// </summary>
        /// <remarks>
        /// This is protected so that inheritors can expose it with their own name
        /// </remarks>
        protected T EventObject { get; set; }

    }
}