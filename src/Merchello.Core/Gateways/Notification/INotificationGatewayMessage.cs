using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Defines a base notification
    /// </summary>
    public interface INotificationGatewayMessage : IGatewayMethod
    {
        /// <summary>
        /// The <see cref="INotificationMessage"/>
        /// </summary>
        INotificationMessage NotificationMessage { get;  }

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
        /// The notification message
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The status of the formatted message
        /// </summary>
        FormatStatus FormatStatus { get; }
    }
}