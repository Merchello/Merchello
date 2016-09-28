namespace Merchello.Core.Events
{
    using System;

    using Merchello.Core.Sync;

    /// <summary>
    /// Event args for cache refresher updates
    /// </summary>
    public class CacheRefresherEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheRefresherEventArgs"/> class.
        /// </summary>
        /// <param name="msgObject">
        /// The message object.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public CacheRefresherEventArgs(object msgObject, MessageType type)
        {
            this.MessageType = type;
            this.MessageObject = msgObject;
        }

        /// <summary>
        /// Gets the message object.
        /// </summary>
        public object MessageObject { get; private set; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public MessageType MessageType { get; private set; }
    }
}