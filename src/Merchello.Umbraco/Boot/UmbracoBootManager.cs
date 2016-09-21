namespace Merchello.Umbraco.Boot
{
    using LightInject;

    using Merchello.Core.Cache;
    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.SqlSyntax;
    using Merchello.Umbraco.Adapters;
    using Merchello.Umbraco.DependencyInjection;
    using Merchello.Web.Boot;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Plugins;

    using Merchello.Core.Plugins;

    /// <summary>
    /// Starts the Merchello Umbraco CMS Package.
    /// </summary>
    internal class UmbracoBootManager : WebBootManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoBootManager"/> class.
        /// </summary>
        public UmbracoBootManager()
            : this(new BootSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoBootManager"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IBootSettings"/>.
        /// </param>
        public UmbracoBootManager(IBootSettings settings)
            : base(settings)
        {
        }

        /// <inheritdoc/>
        internal override void ConfigureCmsServices(IServiceContainer container)
        {
            base.ConfigureCmsServices(container);

            container.RegisterFrom<UmbracoNativeMappingCompositionRoot>();

            // CMS provided
            // TODO figure out if we need to register these as singletons as they already are singletons in Umbraco
            container.Register<global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider>(factory => ApplicationContext.Current.DatabaseContext.SqlSyntax);
            container.Register<global::Umbraco.Core.Cache.CacheHelper>(factory => ApplicationContext.Current.ApplicationCache);
            container.Register<global::Umbraco.Core.Logging.ILogger>(factory => ApplicationContext.Current.ProfilingLogger.Logger);
            container.Register<global::Umbraco.Core.DatabaseContext>(factory => ApplicationContext.Current.DatabaseContext);

            container.Register<global::Umbraco.Core.Plugins.PluginManager>(factory => PluginManager.Current);
            container.RegisterSingleton<IPluginManager, PluginManagerAdapter>();

            container.RegisterSingleton<ISqlSyntaxProvider, SqlSyntaxProviderAdapter>();
            container.RegisterSingleton<ICacheHelper, CacheHelperAdapter>();
            container.RegisterSingleton<ILogger, LoggerAdapter>();
        }

        /// <inheritdoc/>
        /// <param name="container"></param>
        internal override void ConfigureCoreServices(IServiceContainer container)
        {
            base.ConfigureCoreServices(container);
        }
    }
}