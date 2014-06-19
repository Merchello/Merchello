using System;
using System.Collections.Generic;
using Merchello.Core.Formatters;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Defines the NotificationContext
    /// </summary>
    public interface INotificationContext : IGatewayProviderTypedContextBase<NotificationGatewayProviderBase>
    {
        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s by a Monitor Key (Guid)
        /// </summary>
        /// <param name="monitorKey">The monitor key</param>
        /// <returns>A collection of NotificationMessage</returns>
        IEnumerable<INotificationMessage> GetNotificationMessagesByMonitorKey(Guid monitorKey);

        /// <summary>
        /// Sends a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationMessage"/> to be sent</param>
        void Send(INotificationMessage message);

        /// <summary>
        /// Sends a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationMessage"/> to be sent</param>
        /// <param name="formatter">The <see cref="IFormatter"/> to use when formatting the message</param>
        void Send(INotificationMessage message, IFormatter formatter);

    }
}