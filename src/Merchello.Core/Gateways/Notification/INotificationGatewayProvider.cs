namespace Merchello.Core.Gateways.Notification
{
    using System;
    using System.Collections.Generic;
    using Models;

    /// <summary>
    /// Defines a notification gateway provider - a class responsible for sending notifications
    /// </summary>
    public interface INotificationGatewayProvider : IProvider
    {
        /// <summary>
        /// Gets a collection of all <see cref="INotificationMethod"/>'s associated with this provider
        /// </summary>
        IEnumerable<INotificationMethod> NotificationMethods { get; }

        /// <summary>
        /// Creates a <see cref="INotificationGatewayMethod"/>
        /// </summary>
        /// <param name="gatewayResource">The <see cref="IGatewayResource"/> implemented by this method</param>
        /// <param name="name">The name of the notification method</param>
        /// <param name="serviceCode">The description of the notification method</param>
        /// <returns>The newly created <see cref="INotificationGatewayMethod"/></returns>
        INotificationGatewayMethod CreateNotificationMethod(IGatewayResource gatewayResource, string name, string serviceCode);

        /// <summary>
        /// Saves a <see cref="INotificationGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="INotificationGatewayMethod"/> to be saved</param>
        void SaveNotificationMethod(INotificationGatewayMethod method);

        /// <summary>
        /// Deletes a <see cref="INotificationGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="INotificationGatewayMethod"/> to be deleted</param>
        void DeleteNotificationMethod(INotificationGatewayMethod method);

        /// <summary>
        /// Gets a <see cref="INotificationGatewayMethod"/> by it's unique Key (Guid)
        /// </summary>
        /// <param name="notificationGatewayMethodKey">
        /// The unique key (Guid) of the <see cref="INotificationGatewayMethod"/>
        /// </param>
        /// <returns>
        /// The <see cref="INotificationGatewayMethod"/>.
        /// </returns>
        INotificationGatewayMethod GetNotificationGatewayMethodByKey(Guid notificationGatewayMethodKey);

        /// <summary>
        /// Gets a collection of all <see cref="INotificationGatewayMethod"/>s for this provider
        /// </summary>
        /// <returns>A collection of <see cref="INotificationGatewayMethod"/></returns>
        IEnumerable<INotificationGatewayMethod> GetAllNotificationGatewayMethods();            
    }
}