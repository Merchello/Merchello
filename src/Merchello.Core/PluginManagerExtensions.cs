using System;
using System.Collections.Generic;
using Merchello.Core.Gateways;
using Merchello.Core.Observation;
using Umbraco.Core;

namespace Merchello.Core
{
    internal static class PluginManagerExtensions
    {
        /// <summary>
        /// Returns a collection of all Trigger types decorated with the <see cref="ObservableTriggerForAttribute"/>
        /// </summary>        
        internal static IEnumerable<Type> ResolveObservableNotificationTriggers(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<ITrigger, ObservableTriggerForAttribute>();
        }

        internal static IEnumerable<Type> ResolveObserverNotificationMonitors(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<Monito>()
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