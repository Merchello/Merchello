using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Notification
{
    public class NotificationGatewayProviderBase : GatewayProviderBase, INotificationGatewayProvider
    {
        public NotificationGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        {
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            throw new System.NotImplementedException();
        }
    }
}