namespace Merchello.Umbraco.Boot
{
    using LightInject;

    using Merchello.Core.Cache;
    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.SqlSyntax;
    using Merchello.Core.Plugins;
    using Merchello.Umbraco.Adapters;
    using Merchello.Umbraco.DependencyInjection;
    using Merchello.Web.Boot;

    using global::Umbraco.Core;

    using global::Umbraco.Core.Plugins;

    using Merchello.Core.Persistence;

    /// <summary>
    /// Starts the Merchello Umbraco CMS Package.
    /// </summary>
    internal class UmbracoBootManager : WebBootManager
    {
        /// <summary>
        /// Umbraco's <see cref="ApplicationContext"/>.
        /// </summary>
        private readonly ApplicationContext _appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoBootManager"/> class.
        /// </summary>
        public UmbracoBootManager()
            : this(new BootSettings(), ApplicationContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoBootManager"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IBootSettings"/>.
        /// </param>
        /// <param name="appContext">
        /// Umbraco's ApplicationContext.
        /// </param>
        public UmbracoBootManager(IBootSettings settings, ApplicationContext appContext)
            : base(settings)
        {
            Core.Ensure.ParameterNotNull(appContext, nameof(appContext));
            _appContext = appContext;
        }

        /// <inheritdoc/>
        internal override void ConfigureCmsServices(IServiceContainer container)
        {
            base.ConfigureCmsServices(container);

            container.RegisterFrom<UmbracoNativeMappingCompositionRoot>();

            // CMS provided
            // TODO figure out if we need to register these as singletons as they already are singletons in Umbraco
            container.Register<global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider>(factory => _appContext.DatabaseContext.SqlSyntax);
            container.Register<global::Umbraco.Core.Cache.CacheHelper>(factory => _appContext.ApplicationCache);
            container.Register<global::Umbraco.Core.Logging.ILogger>(factory => _appContext.ProfilingLogger.Logger);
            container.Register<global::Umbraco.Core.DatabaseContext>(factory => _appContext.DatabaseContext);
            
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

            container.RegisterSingleton<IDatabaseFactory, DatabaseContextAdapter>();
        }
    }
}