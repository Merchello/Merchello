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

        /// <summary>
        /// Resolves all active payment gateway providers
        /// </summary>
        /// <returns>A collection of all active payment gateway providers</returns>
        public override IEnumerable<NotificationGatewayProviderBase> CreateInstances()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves a payment gateway provider by it's unique key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A payment gateway provider</returns>
        public override NotificationGatewayProviderBase CreateInstance(Guid key)
        {
            throw new NotImplementedException();
        }
    }
}