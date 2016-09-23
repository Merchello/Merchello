namespace Merchello.Umbraco.DependencyInjection
{
    using LightInject;

    using Merchello.Core.Cache;
    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.SqlSyntax;
    using Merchello.Core.Plugins;
    using Merchello.Umbraco.Adapters;
    using Merchello.Umbraco.Mapping;

    using global::Umbraco.Core;

    /// <summary>
    /// Adds Umbraco native class mappings to the container
    /// </summary>
    public class UmbracoCompositionRoot : ICompositionRoot
    {
        /// <inheritdoc/>
        public void Compose(IServiceRegistry container)
        {
            // AutoMapper mappings used in adapters
            container.Register<UmbracoAdapterAutoMapperMappings>();

            container.Register<global::Umbraco.Core.Persistence.UmbracoDatabase>(factory => factory.GetInstance<DatabaseContext>().Database);
            container.Register<global::Umbraco.Core.Persistence.DatabaseSchemaHelper>();

            // Adapters
            container.RegisterSingleton<IPluginManager, PluginManagerAdapter>();
            container.RegisterSingleton<ISqlSyntaxProvider, SqlSyntaxProviderAdapter>();
            container.RegisterSingleton<ICacheHelper, CacheHelperAdapter>();
            container.RegisterSingleton<IProfilingLogger, ProfilingLoggerAdapter>();
            container.RegisterSingleton<ILogger, LoggerAdapter>();
        }
    }
}