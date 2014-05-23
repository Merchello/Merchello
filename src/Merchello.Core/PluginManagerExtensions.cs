using System;
using System.Collections.Generic;
using Merchello.Core.Gateways;
using Merchello.Core.Observation;
using Umbraco.Core;

namespace Merchello.Core
{
    internal static class PluginManagerExtensions
    {

        internal static IEnumerable<Type> ResolveObservableTriggers(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<ITrigger, ObservableTriggerForAttribute>();
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