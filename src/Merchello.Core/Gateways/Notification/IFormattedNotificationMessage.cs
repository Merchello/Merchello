using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Defines a base notification
    /// </summary>
    public interface IFormattedNotificationMessage
    {
        /// <summary>
        /// The sender's From address
        /// </summary>
        string From { get; }

        /// <summary>
        /// The optional reply to address
        /// </summary>
        string ReplyTo { get; }

        /// <summary>
        /// The name of the <see cref="INotificationMessage"/>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A list of recipients for the notification.
        /// </summary>
        /// <remarks>
        /// This could be email addresses, mailing addresses, mobile numbers
        /// </remarks>
        IEnumerable<string> Recipients { get; }

        /// <summary>
        /// True/false indicating if the notification should also be sent to the customer
        /// </summary>
        bool SendToCustomer { get; }

        /// <summary>
        /// The notification message body text
        /// </summary>
        string BodyText { get; }

        /// <summary>
        /// The status of the formatted message
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