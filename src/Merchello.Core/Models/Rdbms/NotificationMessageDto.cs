namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchNotificationMessage" table.
    /// </summary>
    internal class NotificationMessageDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the method key.
        /// </summary>
        public Guid MethodKey { get; set; }

        /// <summary>
        /// Gets or sets the monitor key.
        /// </summary>
        [CanBeNull]
        public Guid? MonitorKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [CanBeNull]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the from address.
        /// </summary>
        [CanBeNull]
        public string FromAddress { get; set; }

        /// <summary>
        /// Gets or sets the reply to.
        /// </summary>
        [CanBeNull]
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        [CanBeNull]
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

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}