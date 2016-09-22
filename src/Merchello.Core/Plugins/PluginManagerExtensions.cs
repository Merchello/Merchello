namespace Merchello.Core.Plugins
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Persistence.Mappers;

    /// <summary>
    /// Extension methods for the <see cref="IPluginManager"/>.
    /// </summary>
    internal static class PluginManagerExtensions
    {
        /// <summary>
        /// Resolves the <see cref="BaseMapper"/> types for mapping entities to DTO classes by attribute.
        /// </summary>
        /// <param name="pluginManager">
        /// The implementation of <see cref="IPluginManager"/>.
        /// </param>
        /// <returns>
        /// The resolved types.
        /// </returns>
        public static IEnumerable<Type> ResolveBaseMappers(this IPluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<BaseMapper, MapperForAttribute>();
        } 
    }
}