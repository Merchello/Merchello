namespace Merchello.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gateways;

    using Merchello.Core.Chains.OfferConstraints;
    using Merchello.Core.Configuration;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Persistence.Migrations;
    using Merchello.Core.ValueConverters.ValueOverrides;

    using Observation;
    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Extension methods for the <see cref="PluginManager"/>
    /// </summary>
    internal static class PluginManagerExtensions
    {
        /// <summary>
        /// Resolves the <see cref="DetachedValueOverriderBase"/>.
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin manager.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IDetachedValueOverrider"/>.
        /// </returns>
        internal static IEnumerable<Type> ResolveDetachedValueOverriders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<DetachedValueOverriderBase, DetachedValueOverriderAttribute>();
        }

        /// <summary>
        /// Returns all available GatewayProvider
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin Manager.
        /// </param>
        /// <returns>
        /// The collection of gateway providers resolved
        /// </returns>
        internal static IEnumerable<Type> ResolveGatewayProviders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<GatewayProviderBase, GatewayProviderActivationAttribute>();
        }

        /// <summary>
        /// Returns a collection of all <see cref="IMonitor"/> types decorated with the <see cref="MonitorForAttribute"/>
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin Manager.
        /// </param>
        /// <returns>
        /// The collection of monitor types resolved
        /// </returns>
        internal static IEnumerable<Type> ResolveObserverMonitors(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<IMonitor, MonitorForAttribute>();
        }

        /// <summary>
        /// Returns a collection of all <see cref="ITrigger"/> types decorated with the <see cref="TriggerForAttribute"/>
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin Manager.
        /// </param>
        /// <returns>
        /// The collection of trigger types resolved
        /// </returns>
        internal static IEnumerable<Type> ResolveObservableTriggers(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<ITrigger, TriggerForAttribute>();
        }

        /// <summary>
        /// The resolve offer constraint chains.
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin manager.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IOfferProcessor"/> types
        /// </returns>
        internal static IEnumerable<Type> ResolveOfferConstraintChains(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<IOfferProcessor, OfferConstraintChainForAttribute>();
        }

        /// <summary>
        /// The resolve entity collection providers.
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin manager.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        internal static IEnumerable<Type> ResolveEnityCollectionProviders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<IEntityCollectionProvider, EntityCollectionProviderAttribute>();
        }

        /// <summary>
        /// Resolves Merchello specific migrations.
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin manager.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        internal static IEnumerable<Type> ResolveMerchelloMigrations(this PluginManager pluginManager)
        {
            return
                pluginManager.ResolveTypesWithAttribute<IMerchelloMigration, MigrationAttribute>();
        } 
    }
}