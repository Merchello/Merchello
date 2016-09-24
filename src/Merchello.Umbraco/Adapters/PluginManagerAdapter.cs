namespace Merchello.Umbraco.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using global::Umbraco.Core.Plugins;

    using Merchello.Core;
    using Merchello.Core.Plugins;

    /// <inheritdoc/>
    internal sealed class PluginManagerAdapter : IPluginManager, IUmbracoAdapter
    {
        /// <summary>
        /// Umbraco's PluginManager.
        /// </summary>
        private readonly PluginManager _pluginManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerAdapter"/> class.
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin manager.
        /// </param>
        public PluginManagerAdapter(PluginManager pluginManager)
        {
            Ensure.ParameterNotNull(pluginManager, nameof(pluginManager));

            this._pluginManager = pluginManager;
        }

        /// <inheritdoc/>
        public void ClearPluginCache()
        {
            this._pluginManager.ClearPluginCache();
        }

        /// <inheritdoc/>
        public IEnumerable<Type> ResolveAttributedTypes<TAttribute>(bool cacheResult = true, IEnumerable<Assembly> specificAssemblies = null) where TAttribute : Attribute
        {
            return this._pluginManager.ResolveAttributedTypes<TAttribute>(cacheResult, specificAssemblies);
        }

        /// <inheritdoc/>
        public IEnumerable<Type> ResolveTypes<T>(bool cacheResult = true, IEnumerable<Assembly> specificAssemblies = null)
        {
            return this._pluginManager.ResolveTypes<T>(cacheResult, specificAssemblies);
        }

        /// <inheritdoc/>
        public IEnumerable<Type> ResolveTypesWithAttribute<T, TAttribute>(bool cacheResult = true, IEnumerable<Assembly> specificAssemblies = null) where TAttribute : Attribute
        {
            return this._pluginManager.ResolveTypesWithAttribute<T, TAttribute>(cacheResult, specificAssemblies);
        }
    }
}