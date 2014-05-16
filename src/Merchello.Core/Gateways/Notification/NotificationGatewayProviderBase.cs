using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Defines the base NotificationGatewayProvider
    /// </summary>
    public abstract class NotificationGatewayProviderBase : GatewayProviderBase, INotificationGatewayProvider
    {
        protected NotificationGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }


        
        private IEnumerable<INotificationMethod> _notificationMethods;

        /// <summary>
        /// Gets a collection of all <see cref="INotificationMethod"/>s associated with this provider
        /// </summary>
        public IEnumerable<INotificationMethod> NotificationMethods
        {
            get
            {
                return _notificationMethods ??
                      (_notificationMethods = GatewayProviderService.GetNotificationMethodsByProviderKey(GatewayProviderSettings.Key));
            }
            protected set { _notificationMethods = value; }
        }
    }
}