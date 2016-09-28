namespace Merchello.Core.Boot
{
    using System;

    using AutoMapper;

    using LightInject;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.ObjectResolution;
    using Merchello.Core.Cache;
    using Merchello.Core.Configuration;
    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Logging;
    using Merchello.Core.Mapping;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Core.Persistence.Migrations.Initial;
    using Merchello.Core.Services;

    using Ensure = Merchello.Core.Ensure;


    /// <summary>
    /// Application boot strap for the Merchello Plugin which initializes all objects to be used in the Merchello Core
    /// </summary>
    /// <remarks>
    /// We needed our own boot strap to setup Merchello specific singletons
    /// </remarks>
    internal class CoreBootManager : BootManagerBase, IBootManager
    {
        #region Fields

        /// <summary>
        /// The multi log resolver.
        /// </summary>
        private MultiLogResolver _muliLogResolver;

        /// <summary>
        /// The Logger.
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// The timer.
        /// </summary>
        private IDisposableTimer _timer;


        /// <summary>
        /// The is complete.
        /// </summary>
        private bool _isComplete;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreBootManager"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        internal CoreBootManager(ICoreBootSettings settings)
        {
            Ensure.ParameterNotNull(settings, nameof(settings));

            this.CoreBootSettings = settings;

            // "Service Registry" - singleton to for required application objects needed for the Merchello instance
            var container = new ServiceContainer();
            container.EnableAnnotatedConstructorInjection();
            IoC.Current = new IoC(container);

            this.IsForTesting = settings.IsForTesting;
        }

        /// <summary>
        /// Gets a value indicating whether Merchello is started.
        /// </summary>
        internal bool IsStarted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Merchello is initialized.
        /// </summary>
        internal bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the boot manager is being used for testing.
        /// </summary>
        internal bool IsForTesting { get; }

        /// <summary>
        /// Gets the core boot settings.
        /// </summary>
        protected ICoreBootSettings CoreBootSettings { get; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <returns>
        /// The <see cref="IBootManager"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if Merchello is already initialized
        /// </exception>
        public override IBootManager Initialize()
        {
            if (IsInitialized)
                throw new InvalidOperationException("The Merchello core boot manager has already been initialized");

            OnMerchelloInit();

            // Grab everythin we need from Umbraco
            ConfigureCmsServices(IoC.Container);

            _timer =
                IoC.Container.GetInstance<IProfilingLogger>()
                    .TraceDuration<CoreBootManager>(
                        $"Merchello {MerchelloVersion.GetSemanticVersion()} application starting on {NetworkHelper.MachineName}",
                        "Merchello application startup complete");

            _logger = IoC.Container.GetInstance<ILogger>();

            _muliLogResolver = new MultiLogResolver(GetMultiLogger(_logger));

            ConfigureCoreServices(IoC.Container);

            InitializeAutoMapperMappers();


            // Ensure the Merchello database is installed.
            this.EnsureDatabase(IoC.Container);

            InitializeResolvers();


            this.IsInitialized = true;   

            return this;
        }

        /// <summary>
        /// Fires after initialization and calls the callback to allow for customizations to occur
        /// </summary>
        /// <param name="afterStartup">
        /// The action to call after startup
        /// </param>
        /// <returns>
        /// The <see cref="IBootManager"/>.
        /// </returns>
        public override IBootManager Startup(Action<IMerchelloContext> afterStartup)
        {
            if (this.IsStarted)
                throw new InvalidOperationException("The boot manager has already been initialized");

            //// if (afterStartup != null)
            ////    afterStartup(MerchelloContext.Current);

            this.IsStarted = true;

            return this;
        }

        /// <summary>
        /// Fires after startup and calls the callback once customizations are locked
        /// </summary>
        /// <param name="afterComplete">
        /// The after Complete.
        /// </param>
        /// <returns>
        /// The <see cref="IBootManager"/>.
        /// </returns>
        public override IBootManager Complete(Action<IMerchelloContext> afterComplete)
        {
            if (this._isComplete)
                throw new InvalidOperationException("The boot manager has already been completed");

            FreezeResolution();

            //if (afterComplete != null)
            //{
            //    afterComplete(MerchelloContext.Current);
            //}

            this._isComplete = true;

            // stop the timer and log the output
            _timer.Dispose();
            return this;
        }

        /// <summary>
        /// Allows for injection of CMS Foundation services that Merchello relies on.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        internal virtual void ConfigureCmsServices(IServiceContainer container)
        {
            // Container
            container.Register<IServiceContainer>(factory => container);
        }

        /// <summary>
        /// Build the core container which contains all core things required to build the MerchelloContext
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        internal virtual void ConfigureCoreServices(IServiceContainer container)
        {
            // Logger
            container.RegisterSingleton<MultiLogResolver>(factory => _muliLogResolver);


            // Configuration
            container.RegisterFrom<ConfigurationCompositionRoot>();

            // Cache
            // FYI-ICacheHelper registered in ConfigureSmsServices
            container.RegisterSingleton<IRuntimeCacheProvider>(factory => factory.GetInstance<ICacheHelper>().RuntimeCache);


            // Database related
            container.RegisterFrom<RepositoryCompositionRoot>();
        }

        /// <summary>
        /// The initializes the AutoMapper mappings.
        /// </summary>
        protected void InitializeAutoMapperMappers()
        {
            var container = IoC.Container;

            Mapper.Initialize(configuration =>
                {
                    foreach (var mc in container.GetAllInstances<MerchelloMapperConfiguration>())
                    {
                        mc.ConfigureMappings(configuration);
                    }
                });
        }

        protected virtual void CreateMerchelloContext()
        {

        }

        protected virtual void InitialCurrencyContext()
        {

        }

        /// <summary>
        /// Initializes the logger resolver.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        protected virtual void InitializeLoggerResolver(IMultiLogger logger)
        {
            if (!MultiLogResolver.HasCurrent)
                MultiLogResolver.Current = new MultiLogResolver(logger);
        }



        /// <summary>
        /// Responsible for initializing resolvers.
        /// </summary>
        protected virtual void InitializeResolvers()
        {
            var container = IoC.Container;

            MappingResolver.Current = (MappingResolver)container.GetInstance<IMappingResolver>();

            //if (!TriggerResolver.HasCurrent)
            //TriggerResolver.Current = new TriggerResolver(PluginManager.Current.ResolveObservableTriggers());

            //if (!MonitorResolver.HasCurrent)
            //MonitorResolver.Current = new MonitorResolver(MerchelloContext.Current.Gateways.Notification, PluginManager.Current.ResolveObserverMonitors());            

            //if (!OfferProcessorFactory.HasCurrent)
            //OfferProcessorFactory.Current = new OfferProcessorFactory(PluginManager.Current.ResolveOfferConstraintChains());
        }

        /// <summary>
        /// Initializes value converters.
        /// </summary>
        protected virtual void InitializeValueConverters()
        {
        }

        protected virtual void InitializeGatewayResolver()
        {
        }

        /// <summary>
        /// Responsible initializing observer subscriptions.
        /// </summary>
        protected virtual void InitializeObserverSubscriptions()
        {
            //if (!TriggerResolver.HasCurrent || !MonitorResolver.HasCurrent) return;

            //var monitors = MonitorResolver.Current.GetAllMonitors();

            //LogHelper.Info<CoreBootManager>("Starting subscribing Monitors to Triggers");

            //foreach (var monitor in monitors)
            //{
            //    monitor.Subscribe(TriggerResolver.Current);
            //}

            //LogHelper.Info<Umbraco.Core.CoreBootManager>("Finished subscribing Monitors to Triggers");            
        }

        /// <summary>
        /// Ensures the Merchello database is present.
        /// </summary>
        /// <param name="container">
        /// The IoC container.
        /// </param>
        /// <remarks>
        /// We actually removed the package action to install the database in around version 1.12(?) so that we 
        /// could push installs to UAAS more easily.  For this version, we are going a bit further and getting rid
        /// of ALL package actions so that we can more easily offer straight NuGet based install (without requiring installation through
        /// the CMS back office).
        /// </remarks>
        protected virtual void EnsureDatabase(IServiceContainer container)
        {
            MultiLogHelper.Info<CoreBootManager>("Verifying Merchello database is present.");
            var schemaCreation = container.GetInstance<IDatabaseSchemaCreation>();
            var result = schemaCreation.ValidateSchema();

            var dbVersion = result.DetermineInstalledVersion();

            if (dbVersion != MerchelloVersion.Current)
            {
                // TODO initial migration
                if (dbVersion == new Version(0, 0, 0))
                {
                    _logger.Info<CoreBootManager>("Merchello database not installed.  Initial migration");
                }
                else
                {
                    _logger.Info<CoreBootManager>("Merchello version did not match, find migration(s).");
                }
            }
            else
            {
                _logger.Info<CoreBootManager>("Merchello database is the current version");
            }
        }

        /// <summary>
        /// Gets the <see cref="MultiLogger"/>.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="IMultiLogger"/>.
        /// </returns>
        protected virtual IMultiLogger GetMultiLogger(ILogger logger)
        {
            return new MultiLogger(logger);
        }

        /// <summary>
        /// Freeze resolution to not allow Resolvers to be modified
        /// </summary>
        protected virtual void FreezeResolution()
        {
            Resolution.Freeze();
        }
    }
}
