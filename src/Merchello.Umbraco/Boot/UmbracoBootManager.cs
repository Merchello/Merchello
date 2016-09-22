namespace Merchello.Umbraco.Boot
{
    using LightInject;

    using Merchello.Core.Cache;
    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.SqlSyntax;
    using Merchello.Core.Plugins;
    using Merchello.Umbraco.Adapters;
    using Merchello.Umbraco.DependencyInjection;
    using Merchello.Web.Boot;

    using global::Umbraco.Core;

    using global::Umbraco.Core.Plugins;

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
        /// Umbraco's <see cref="PluginManager"/>.
        /// </summary>
        private readonly PluginManager _pluginManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoBootManager"/> class.
        /// </summary>
        public UmbracoBootManager()
            : this(new BootSettings(), ApplicationContext.Current, PluginManager.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoBootManager"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="appContext">
        /// The app context.
        /// </param>
        public UmbracoBootManager(IBootSettings settings, ApplicationContext appContext)
            : this(settings, appContext, PluginManager.Current)
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
        /// <param name="pluginManager">
        /// Umbraco's PluginManager
        /// </param>
        public UmbracoBootManager(IBootSettings settings, ApplicationContext appContext, PluginManager pluginManager)
            : base(settings)
        {
            Core.Ensure.ParameterNotNull(appContext, nameof(appContext));
            Core.Ensure.ParameterNotNull(pluginManager, nameof(pluginManager));

            _appContext = appContext;
            _pluginManager = pluginManager;
        }

        /// <inheritdoc/>
        internal override void ConfigureCmsServices(IServiceContainer container)
        {
            base.ConfigureCmsServices(container);

            container.RegisterFrom<UmbracoNativeMappingCompositionRoot>();

            // CMS provided
            container.RegisterSingleton<global::Umbraco.Core.Persistence.SqlSyntax.ISqlSyntaxProvider>(factory => _appContext.DatabaseContext.SqlSyntax);
            container.RegisterSingleton<global::Umbraco.Core.Cache.CacheHelper>(factory => _appContext.ApplicationCache);
            container.RegisterSingleton<global::Umbraco.Core.Logging.ILogger>(factory => _appContext.ProfilingLogger.Logger);
            container.RegisterSingleton<global::Umbraco.Core.DatabaseContext>(factory => _appContext.DatabaseContext);
            
            container.RegisterSingleton<global::Umbraco.Core.Plugins.PluginManager>(factory => _pluginManager);
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