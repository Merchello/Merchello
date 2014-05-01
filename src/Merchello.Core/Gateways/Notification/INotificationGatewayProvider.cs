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
        /// Creates a <see cref="INotificationGatewayMessage"/>
        /// </summary>
        /// <param name="gatewayResource">The <see cref="IGatewayResource"/></param>
        /// <param name="name">The name of the message (for the UI)</param>
        /// <param name="description">A description of the message (for the UI)</param>
        /// <param name="ruleKey">The option Rule / Trigger key</param>
        /// <returns></returns>
        INotificationGatewayMessage CreateNotificationMessage(IGatewayResource gatewayResource, string name, string description, Guid? ruleKey = null);

        /// <summary>
        /// Saves a <see cref="INotificationGatewayMessage"/>
        /// </summary>
        /// <param name="gatewayMessage">The <see cref="INotificationGatewayMessage"/></param>
        void SaveNotificationMessage(INotificationGatewayMessage gatewayMessage);

        /// <summary>
        /// Deletes a <see cref="INotificationGatewayMessage"/>
        /// </summary>
        /// <param name="gatewayMessage">The <see cref="INotificationGatewayMessage"/> to be deleted</param>
        void DeleteNotificationMessage(INotificationGatewayMessage gatewayMessage);

        /// <summary>
        /// Gets a <see cref="INotificationGatewayMessage"/> by the <see cref="INotificationMessage"/> key
        /// </summary>
        /// <param name="notificationMessageKey">The <see cref="INotificationMessage"/> key</param>
        /// <returns>The <see cref="INotificationGatewayMessage"/></returns>
        INotificationGatewayMessage GetNotificationGatewayMessageByKey(Guid notificationMessageKey);

        /// <summary>
        /// Gets a collection of all <see cref="INotificationMessage"/>'s associated with this provider
        /// </summary>
        IEnumerable<INotificationMessage> NotificationMessages { get; }
    }
}