namespace Merchello.Core.Events
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Security.Permissions;

    /// <summary>
    /// Event args for that can support cancellation
    /// </summary>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Events/CancellableEventArgs.cs"/>
    [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
    public class CancellableEventArgs : EventArgs
    {
        /// <summary>
        /// A value indicating whether or not the event can be cancelled.
        /// </summary>
        private bool _cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableEventArgs"/> class.
        /// </summary>
        /// <param name="canCancel">
        /// The can cancel.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <param name="additionalData">
        /// The additional data.
        /// </param>
        public CancellableEventArgs(bool canCancel, EventMessages messages, IDictionary<string, object> additionalData)
        {
            CanCancel = canCancel;
            Messages = messages;
            AdditionalData = new ReadOnlyDictionary<string, object>(additionalData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableEventArgs"/> class.
        /// </summary>
        /// <param name="canCancel">
        /// The can cancel.
        /// </param>
        /// <param name="eventMessages">
        /// The event messages.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the event messages parameter is null
        /// </exception>
        public CancellableEventArgs(bool canCancel, EventMessages eventMessages)
        {
            if (eventMessages == null) throw new ArgumentNullException(nameof(eventMessages));
            CanCancel = canCancel;
            Messages = eventMessages;
            AdditionalData = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableEventArgs"/> class.
        /// </summary>
        /// <param name="canCancel">
        /// A value indicating whether or not this event can be cancelled.
        /// </param>
        public CancellableEventArgs(bool canCancel)
        {
            CanCancel = canCancel;

            //// create a standalone messages
            Messages = new EventMessages();
            AdditionalData = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableEventArgs"/> class.
        /// </summary>
        /// <param name="eventMessages">
        /// The event messages.
        /// </param>
        public CancellableEventArgs(EventMessages eventMessages)
            : this(true, eventMessages)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableEventArgs"/> class.
        /// </summary>
        public CancellableEventArgs()
            : this(true)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance will support being cancellable
        /// </summary>
        public bool CanCancel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance supports cancellation, this gets/sets the cancel value
        /// </summary>
        public bool Cancel
        {
            get
            {
                if (CanCancel == false)
                {
                    throw new InvalidOperationException("This event argument class does not support cancelling.");
                }

                return _cancel;
            }

            set
            {
                if (CanCancel == false)
                {
                    throw new InvalidOperationException("This event argument class does not support cancelling.");
                }

                _cancel = value;
            }
        }

        /// <summary>
        /// Gets the EventMessages object which is used to add messages to the message collection for this event
        /// </summary>
        public EventMessages Messages { get; private set; }


        /// <summary>
        /// Gets additional arbitrary readonly data which can be read by event subscribers
        /// </summary>
        /// <remarks>
        /// This allows for a bit of flexibility in our event raising - it's not pretty but we need to maintain backwards compatibility 
        /// so we cannot change the strongly typed nature for some events.
        /// </remarks>
        public ReadOnlyDictionary<string, object> AdditionalData { get; private set; }

        /// <summary>
        /// if this instance supports cancellation, this will set Cancel to true with an affiliated cancellation message
        /// </summary>
        /// <param name="cancelationMessage">The <see cref="EventMessage"/></param>
        public void CancelOperation(EventMessage cancelationMessage)
        {
            Cancel = true;
            cancelationMessage.IsDefaultEventMessage = true;
            Messages.Add(cancelationMessage);
        }
    }
}
