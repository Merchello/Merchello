using System;
using System.Collections.Generic;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Represents a NotificationContext
    /// </summary>
    internal class NotificationContext : GatewayProviderTypedContextBase<NotificationGatewayProviderBase>, INotificationContext
    {
        public NotificationContext(IGatewayProviderService gatewayProviderService, IGatewayProviderResolver resolver) 
            : base(gatewayProviderService, resolver)
        { }


    }
}