using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Notification.Formatters;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification
{
    public interface INotificationGatewayMethod : IGatewayMethod
    {
        /// <summary>
        /// Creates a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="name">A name for the message (used in Back Office UI)</param>
        /// <param name="description">A description for the message (used in Back Office UI)</param>
        /// <param name="fromAddress"></param>
        /// <param name="recipients"></param>
        /// <param name="bodyText"></param>
        /// <returns></returns>
        INotificationMessage CreateNotificationMessage(string name, string description, string fromAddress, IEnumerable<string> recipients, string bodyText);

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/> given it's unique Key (Guid)
        /// </summary>
        /// <param name="messageKey">The unique key (Guid) of the <see cref="IFormattedNotificationMessage"/></param>
        bool Send(Guid messageKey);

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/> given it's unique Key (Guid)
        /// </summary>
        /// <param name="messageKey">The unique key (Guid) of the <see cref="IFormattedNotificationMessage"/></param>
        /// <param name="formatter">The <see cref="INotificationFormatter"/> to use to format the message</param>
        bool Send(Guid messageKey, INotificationFormatter formatter);

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        bool Send(INotificationMessage notificationMessage);

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        /// <param name="formatter">The <see cref="INotificationFormatter"/> to use to format the message</param>
        bool Send(INotificationMessage notificationMessage, INotificationFormatter formatter);

        /// <summary>
        /// Gets the <see cref="INotificationMethod"/>
        /// </summary>
        INotificationMethod NotificationMethod { get; }
    }
}