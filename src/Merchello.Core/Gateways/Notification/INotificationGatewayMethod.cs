namespace Merchello.Core.Gateways.Notification
{
    using System.Collections.Generic;
    using Formatters;
    using Models;

    /// <summary>
    /// Defines a notification gateway method
    /// </summary>
    public interface INotificationGatewayMethod : IGatewayMethod
    {
        /// <summary>
        /// Gets the <see cref="INotificationMethod"/>
        /// </summary>
        INotificationMethod NotificationMethod { get; }

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s associated with this NotificationMethod
        /// </summary>
        IEnumerable<INotificationMessage> NotificationMessages { get; } 

        /// <summary>
        /// Creates a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="name">A name for the message (used in Back Office UI)</param>
        /// <param name="description">A description for the message (used in Back Office UI)</param>
        /// <param name="fromAddress">The senders or "From Address"</param>
        /// <param name="recipients">A collection of recipients</param>
        /// <param name="bodyText">The body text for the message</param>
        /// <returns>A <see cref="INotificationMessage"/></returns>
        INotificationMessage CreateNotificationMessage(string name, string description, string fromAddress, IEnumerable<string> recipients, string bodyText);

        /// <summary>
        /// Saves a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationMessage"/> to be saved</param>
        void SaveNotificationMessage(INotificationMessage message);

        /// <summary>
        /// Deletes a <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationMessage"/> to be deleted</param>        
        void DeleteNotificationMessage(INotificationMessage message);

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        void Send(INotificationMessage notificationMessage);

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        /// <param name="formatter">The <see cref="IFormatter"/> to use to format the message</param>
        void Send(INotificationMessage notificationMessage, IFormatter formatter);
    }
}