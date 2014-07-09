namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// The notification message display.
    /// </summary>
    public class NotificationMessageDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the from address.
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Gets or sets the reply to.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        public string BodyText { get; set; }

        /// <summary>
        /// Gets or sets the max length.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether body text is file path.
        /// </summary>
        public bool BodyTextIsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the monitor key.
        /// </summary>
        public Guid? MonitorKey { get; set; }

        /// <summary>
        /// Gets or sets the method key.
        /// </summary>
        public Guid MethodKey { get; set; }

        /// <summary>
        /// Gets or sets the recipients.
        /// </summary>
        public string Recipients { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether send to customer.
        /// </summary>
        public bool SendToCustomer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether disabled.
        /// </summary>
        public bool Disabled { get; set; }
    }
}