namespace Merchello.Core.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents the plugin manager.
    /// </summary>
    /// <remarks>
    /// Umbraco does not actually have this interface.
    /// </remarks>
    public interface IPluginManager
    {
        /// <summary>
        /// Clears the plugin cache.
        /// </summary>
        void ClearPluginCache();

        /// <summary>
        /// Resolves types with attributes.
        /// </summary>
        /// <param name="cacheResult">
        /// A value indicated whether or not to cache the results.
        /// </param>
        /// <param name="specificAssemblies">
        /// The specific assemblies.
        /// </param>
        /// <typeparam name="TAttribute">
        /// The type of the attribute
        /// </typeparam>
        /// <returns>
        /// The collection of resolved types.
        /// </returns>
        IEnumerable<Type> ResolveAttributedTypes<TAttribute>(bool cacheResult = true, IEnumerable<Assembly> specificAssemblies = null) 
            where TAttribute : Attribute;

        /// <summary>
        /// Resolves types.
        /// </summary>
        /// <param name="cacheResult">
        /// A value indicated whether or not to cache the results.
        /// </param>
        /// <param name="specificAssemblies">
        /// The specific assemblies.
        /// </param>
        /// <typeparam name="T">
        /// The type to resolve
        /// </typeparam>
        /// <returns>
        /// The collection of resolved types.
        /// </returns>
        IEnumerable<Type> ResolveTypes<T>(bool cacheResult = true, IEnumerable<Assembly> specificAssemblies = null);

        /// <summary>
        /// Resolve types with attribute.
        /// </summary>
        /// <param name="cacheResult">
        /// A value indicated whether or not to cache the results.
        /// </param>
        /// <param name="specificAssemblies">
        /// The specific assemblies.
        /// </param>
        /// <typeparam name="T">
        /// The type to resolve
        /// </typeparam>
        /// <typeparam name="TAttribute">
        /// The type of the attribute decorator
        /// </typeparam>
        /// <returns>
        /// The collection of resolved types.
        /// </returns>
        IEnumerable<Type> ResolveTypesWithAttribute<T, TAttribute>(bool cacheResult = true, IEnumerable<Assembly> specificAssemblies = null) 
            where TAttribute : Attribute;
    }
}