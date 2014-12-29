namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a NotificationMethodService
    /// </summary>
    internal interface INotificationMethodService : IService
    {
        /// <summary>
        /// Creates a <see cref="INotificationMethod"/> and saves it to the database
        /// </summary>
        /// <param name="providerKey">The <see cref="IGatewayProviderSettings"/> key</param>
        /// <param name="name">The name of the notification (used in back office)</param>
        /// <param name="serviceCode">The notification service code</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>An Attempt{<see cref="INotificationMethod"/>}</returns>
        Attempt<INotificationMethod> CreateNotificationMethodWithKey(Guid providerKey, string name, string serviceCode, bool raiseEvents = true);

        /// <summary>
        /// Saves a single instance of <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethod">The <see cref="INotificationMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(INotificationMethod notificationMethod, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethods">The collection of <see cref="INotificationMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<INotificationMethod> notificationMethods, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single instance os <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethod">The <see cref="INotificationMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(INotificationMethod notificationMethod, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethods">The collection of <see cref="INotificationMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<INotificationMethod> notificationMethods, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="INotificationMethod"/> by it's key
        /// </summary>
        /// <param name="key">The key (Guid) of the <see cref="INotificationMethod"/> to be retrieved</param>
        /// <returns>The <see cref="INotificationMethod"/></returns>
        INotificationMethod GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of all <see cref="INotificationMethod"/> assoicated with a provider
        /// </summary>
        /// <param name="providerKey">The <see cref="IGatewayProviderSettings"/> key</param>
        /// <returns>A collection of all <see cref="INotificationMethod"/> associated with a provider</returns>
        IEnumerable<INotificationMethod> GetNotifcationMethodsByProviderKey(Guid providerKey);
    }
}