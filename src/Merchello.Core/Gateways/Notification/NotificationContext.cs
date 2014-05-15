using System;
using System.Linq;
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

        /// <summary>
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (Guid) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (Guid) of the <see cref="IGatewayMethod"/></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public override NotificationGatewayProviderBase GetProviderByMethodKey(Guid gatewayMethodKey)
        {
            return GetAllActivatedProviders()
                .FirstOrDefault(x => ((NotificationGatewayProviderBase)x)
                    .NotificationMethods.Any(y => y.Key == gatewayMethodKey)) as NotificationGatewayProviderBase;
        }
    }
}