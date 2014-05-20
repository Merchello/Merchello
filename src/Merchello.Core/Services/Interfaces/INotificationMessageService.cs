using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines a NotificationMessageService
    /// </summary>
    public interface INotificationMessageService : IService
    {
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
    }
}