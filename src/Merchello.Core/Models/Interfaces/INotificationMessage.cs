namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a notification message
    /// </summary>
    public interface INotificationMessage : IEntity
    {
        /// <summary>
        /// Gets or sets an optional key for Notification Monitor Rule
        /// </summary>
        Guid? MonitorKey { get; set; }

        /// <summary>
        /// Gets the <see cref="INotificationMethod"/> key
        /// </summary>
        Guid MethodKey { get; }

        /// <summary>
        /// Gets or sets the name of the notification
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the  a brief description of the notification
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets the sender's "from" address
        /// </summary>
        string FromAddress { get; }

        /// <summary>
        /// Gets or sets the Reply To address
        /// </summary>
        string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the path or text source
        /// </summary>
        string BodyText { get; set; }

        /// <summary>
        /// Gets or sets the  maximum length of the message to be sent
        /// </summary>
        int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the string value of Message is actually a path to a file to read
        /// </summary>
        bool BodyTextIsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the recipients of the notification
        /// </summary>
        string Recipients { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this notification should be sent to the customer
        /// </summary>
        bool SendToCustomer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this notification is disabled
        /// </summary>
        bool Disabled { get; set; }

        /// <summary>
        /// Clones the message.
        /// </summary>
        /// <returns>
        /// The <see cref="INotificationMessage"/>.
        /// </returns>
        INotificationMessage ShallowCopy();
    }
}