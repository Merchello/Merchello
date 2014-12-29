namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a NotificationMessageService
    /// </summary>
    internal interface INotificationMessageService : IService
    {

        /// <summary>
        /// Creates a <see cref="INotificationMessage"/> and saves it to the database
        /// </summary>
        /// <param name="methodKey">The <see cref="INotificationMethod"/> key</param>
        /// <param name="name">The name of the message (primarily used in the back office UI)</param>
        /// <param name="description">The name of the message (primarily used in the back office UI)</param>
        /// <param name="fromAddress">The senders or "from" address</param>
        /// <param name="recipients">A collection of recipient address</param>
        /// <param name="bodyText">The body text of the message</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Attempt{INotificationMessage}</returns>
        Attempt<INotificationMessage> CreateNotificationMethodWithKey(Guid methodKey, string name, string description, string fromAddress, IEnumerable<string> recipients, string bodyText, bool raiseEvents = true);

        /// <summary>
        /// Saves a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="NotificationMessage"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(INotificationMessage notificationMessage, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="INotificationMessage"/>s
        /// </summary>
        /// <param name="notificationMessages">The collection of <see cref="INotificationMessage"/>s to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<INotificationMessage> notificationMessages, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="INotificationMessage"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(INotificationMessage notificationMessage, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="INotificationMessage"/> by it's unique key (Guid)
        /// </summary>
        /// <param name="key">The key (Guid) for the <see cref="INotificationMessage"/> to be retrieved</param>
        /// <returns>Optional boolean indicating whether or not to raise events</returns>
        INotificationMessage GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s base on the notification method
        /// </summary>
        /// <param name="notificationMethodKey">The <see cref="INotificationMethod"/> key</param>
        /// <returns>Optional boolean indicating whether or not to raise events</returns>
        IEnumerable<INotificationMessage> GetNotificationMessagesByMethodKey(Guid notificationMethodKey);

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s based on a monitor key
        /// </summary>
        /// <param name="monitorKey">The Notification Monitor Key (Guid)</param>
        /// <returns>A collection of <see cref="INotificationMessage"/></returns>
        IEnumerable<INotificationMessage> GetNotificationMessagesByMonitorKey(Guid monitorKey);
    }
}