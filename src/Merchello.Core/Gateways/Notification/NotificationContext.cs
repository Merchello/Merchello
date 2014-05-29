using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Observation;
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
        {
        }

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

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s by a Monitor Key (Guid)
        /// </summary>
        /// <param name="monitorKey"></param>
        /// <returns>A collection of NotificationMessage</returns>
        internal IEnumerable<INotificationMessage> GetNotificationMessagesByMonitorKey(Guid monitorKey)
        {
            return GatewayProviderService.GetNotificationMessagesByMonitorKey(monitorKey);
        }
    }
}