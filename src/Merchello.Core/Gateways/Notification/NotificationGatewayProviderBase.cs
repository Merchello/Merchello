namespace Merchello.Core.Gateways.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Services;
    using Umbraco.Core.Cache;

    /// <summary>
    /// Defines the base NotificationGatewayProvider
    /// </summary>
    public abstract class NotificationGatewayProviderBase : GatewayProviderBase, INotificationGatewayProvider
    {
        private IEnumerable<INotificationMethod> _notificationMethods;

        protected NotificationGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {            
        }

        /// <summary>
        /// Gets or sets a collection of all <see cref="INotificationMethod"/>s associated with this provider
        /// </summary>
        public IEnumerable<INotificationMethod> NotificationMethods
        {
            get
            {
                return _notificationMethods ??
                      (_notificationMethods = GatewayProviderService.GetNotificationMethodsByProviderKey(GatewayProviderSettings.Key));
            }

            protected set
            {
                _notificationMethods = value;
            }
        }

        /// <summary>
        /// Creates a <see cref="INotificationGatewayMethod"/>
        /// </summary>
        /// <param name="gatewayResource">The <see cref="IGatewayResource"/> implemented by this method</param>
        /// <param name="name">The name of the notification method</param>
        /// <param name="serviceCode">The description of the notification method</param>        
        /// <returns>The <see cref="INotificationGatewayMethod"/> created</returns>
        public abstract INotificationGatewayMethod CreateNotificationMethod(IGatewayResource gatewayResource, string name, string serviceCode);
    
        /// <summary>
        /// Saves a <see cref="INotificationGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="INotificationGatewayMethod"/> to be saved</param>
        public void SaveNotificationMethod(INotificationGatewayMethod method)
        {
            GatewayProviderService.Save(method.NotificationMethod);

            NotificationMethods = null;
        }

        /// <summary>
        /// Deletes a <see cref="INotificationGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="INotificationGatewayMethod"/> to be deleted</param>
        public void DeleteNotificationMethod(INotificationGatewayMethod method)
        {
            GatewayProviderService.Delete(method.NotificationMethod);

            NotificationMethods = null;
        }

        /// <summary>
        /// Gets a <see cref="INotificationGatewayMethod"/> by it's unique Key (Guid)
        /// </summary>
        /// <param name="notificationGatewayMethodKey">
        /// The unique key (Guid) of the <see cref="INotificationGatewayMethod"/>
        /// </param>
        /// <returns>
        /// The <see cref="INotificationGatewayMethod"/> or null
        /// </returns>
        public INotificationGatewayMethod GetNotificationGatewayMethodByKey(Guid notificationGatewayMethodKey)
        {
            return GetAllNotificationGatewayMethods().FirstOrDefault(x => x.NotificationMethod.Key == notificationGatewayMethodKey);
        }

        /// <summary>
        /// Gets a collection of all <see cref="INotificationGatewayMethod"/>s for this provider
        /// </summary>
        /// <returns>A collection of <see cref="INotificationGatewayMethod"/></returns>
        public abstract IEnumerable<INotificationGatewayMethod> GetAllNotificationGatewayMethods();
      
        /// <summary>
        /// Used for testing
        /// </summary>
        internal void DeleteAllNotificationGatewayMethods()
        {
            foreach (var method in GetAllNotificationGatewayMethods())
            {
                DeleteNotificationMethod(method);
            }
        }
    }
}