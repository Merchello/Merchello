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
        /// Returns a collection of all <see cref="ITrigger"/> types decorated with the <see cref="TriggerForAttribute"/>
        /// </summary>        
        internal static IEnumerable<Type> ResolveObservableTriggers(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<ITrigger, TriggerForAttribute>();
        }

        /// <summary>
        /// Returns a collection of all <see cref="IMonitor"/> types decorated with the <see cref="MonitorForAttribute"/>
        /// </summary> 
        internal static IEnumerable<Type> ResolveObserverMonitors(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<IMonitor, MonitorForAttribute>();
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