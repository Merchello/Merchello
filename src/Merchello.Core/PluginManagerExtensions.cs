using System;
using System.Collections.Generic;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Triggers;
using Umbraco.Core;

namespace Merchello.Core
{
    internal static class PluginManagerExtensions
    {

        internal static IEnumerable<Type> ResolveEventTriggeredActions(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<IEventTriggeredAction, EventTriggeredActionForAttribute>();
        }

        /// <summary>
        /// Returns all available GatewayProvider
        /// </summary>        
        internal static IEnumerable<Type> ResolveGatewayProviders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<GatewayProviderBase, GatewayProviderActivationAttribute>();
        }
    }
}