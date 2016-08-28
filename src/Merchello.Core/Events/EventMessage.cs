namespace Merchello.Core.Events
{
    /// <summary>
    /// An event message
    /// </summary>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Events/EventMessage.cs"/>
    public sealed class EventMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessage"/> class.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageType">
        /// The message Type.
        /// </param>
        public EventMessage(string category, string message, EventMessageType messageType = EventMessageType.Default)
        {
            Category = category;
            Message = message;
            MessageType = messageType;
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public EventMessageType MessageType { get; private set; }

        /// <summary>
        /// Gets or sets a value used to track if this message should be used as a default message so that Merchello doesn't also append it's own default messages
        /// </summary>
        internal bool IsDefaultEventMessage { get; set; }
    }
}