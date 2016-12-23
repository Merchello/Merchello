namespace Merchello.Core.Gateways.Notification
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// Defines a base notification
    /// </summary>
    public interface IFormattedNotificationMessage
    {
        /// <summary>
        /// Gets the sender's From address
        /// </summary>
        string From { get; }

        /// <summary>
        /// Gets the optional reply to address
        /// </summary>
        string ReplyTo { get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="INotificationMessage"/>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a list of recipients for the notification.
        /// </summary>
        /// <remarks>
        /// This could be email addresses, mailing addresses, mobile numbers
        /// </remarks>
        IEnumerable<string> Recipients { get; }

        /// <summary>
        /// Gets a value indicating whether the notification should also be sent to the customer
        /// </summary>
        bool SendToCustomer { get; }

        /// <summary>
        /// Gets the notification message body text
        /// </summary>
        string BodyText { get; }

        /// <summary>
        /// Gets the status of the formatted message
        /// </summary>
        FormatStatus FormatStatus { get; }

        /// <summary>
        /// Adds a recipient to the recipient list
        /// </summary>
        /// <param name="value">The recipient to be added</param>
        void AddRecipient(string value);

        /// <summary>
        /// Removes a recipient from the recipient list
        /// </summary>
        /// <param name="value">The recipient to be removed</param>
        void RemoveRecipient(string value);
    }
}