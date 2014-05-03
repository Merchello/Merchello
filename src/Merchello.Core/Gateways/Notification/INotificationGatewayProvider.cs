using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Defines a notification gateway provider - a class responsible for sending notifications
    /// </summary>
    public interface INotificationGatewayProvider : IProvider
    {
        

        /// <summary>
        /// Gets a collection of all <see cref="INotificationMethod"/>'s associated with this provider
        /// </summary>
        IEnumerable<INotificationMethod> NotificationMethods { get; }
    }
}