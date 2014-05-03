using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Payment;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Resolves NotificationGatewayProviders
    /// </summary>
    internal sealed class NotificationGatewayProviderResolver : LazyManyObjectsResolverBase<NotificationGatewayProviderResolver, NotificationGatewayProviderBase>
    {
        internal NotificationGatewayProviderResolver(Func<IEnumerable<Type>> providers)
            : base(providers)
        { }

        public IEnumerable<Type> ProviderTypes
        {
            get { return InstanceTypes; }
        }
    }
}