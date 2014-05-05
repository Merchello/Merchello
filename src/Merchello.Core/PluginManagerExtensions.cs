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
        /// Return all available NotificationGatewayProvider
        /// </summary>
        internal static IEnumerable<Type> ResolveNotificationGatewayProviders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<NotificationGatewayProviderBase, GatewayProviderActivationAttribute>();
        }

        /// <summary>
        /// Return all available PaymentGatewayProvider
        /// </summary>
        internal static IEnumerable<Type> ResolvePaymentGatewayProviders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<PaymentGatewayProviderBase, GatewayProviderActivationAttribute>();
        }

        /// <summary>
        /// Returns all available ShippingGatewayProvider
        /// </summary>
        internal static IEnumerable<Type> ResolveShippingGatewayProviders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<ShippingGatewayProviderBase, GatewayProviderActivationAttribute>();
        }

        /// <summary>
        /// Returns all available TaxationGatewayProvider
        /// </summary>        
        internal static IEnumerable<Type> ResolveTaxationGatewayProviders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<TaxationGatewayProviderBase, GatewayProviderActivationAttribute>();
        }
    }
}