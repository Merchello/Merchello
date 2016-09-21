namespace Merchello.Core.Boot
{
    using System;

    using AutoMapper;

    using LightInject;

    using Merchello.Core.Acquired;
    using Merchello.Core.Cache;
    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Logging;
    using Merchello.Core.Mapping;
    using Merchello.Core.Persistence.SqlSyntax;
    using Merchello.Core.Services;

    using NPoco;

    using Ensure = Merchello.Core.Ensure;

    //using Cache;
    //using Configuration;
    //using Gateways;

    //using Merchello.Core.Chains.OfferConstraints;
    //using Merchello.Core.EntityCollections;
    //using Merchello.Core.Events;
    //using Merchello.Core.Logging;
    //using Merchello.Core.ValueConverters;

    //using Observation;
    //using Persistence.UnitOfWork;

    //using RepositoryFactory = Merchello.Core.Persistence.RepositoryFactory;

    /// <summary>
    /// Application boot strap for the Merchello Plugin which initializes all objects to be used in the Merchello Core
    /// </summary>
    /// <remarks>
    /// We needed our own boot strap to setup Merchello specific singletons
    /// </remarks>
    internal class CoreBootManager : BootManagerBase, IBootManager
    {
        #region Fields

        private MultiLogResolver _muliLogResolver;

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
            SR.Current = new SR(container);

            this.IsForTesting = settings.IsForTesting;
            

            this.SetUnitOfWorkProvider();
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
            ConfigureCmsServices(SR.Current.Container);

            _muliLogResolver = new MultiLogResolver(GetMultiLogger(SR.Current.Container.GetInstance<ILogger>()));

            ConfigureCoreServices(SR.Current.Container);

            // this may happen too late since the SqlSyntaxProviderAdapter is done above and it requires
            // automapper mappings - but if nothing kicks off ... needs testing.
            InitializeAutoMapperMappers();

            //var serviceContext = new ServiceContext(new RepositoryFactory(cache, logger, _sqlSyntaxProvider), _unitOfWorkProvider, logger, new TransientMessageFactory());



            //InitializeGatewayResolver(serviceContext, cache);

            //CreateMerchelloContext(serviceContext, cache);

            //InitialCurrencyContext(serviceContext.StoreSettingService);

            //InitializeResolvers();

            //InitializeValueConverters();

            //InitializeObserverSubscriptions();

            //this.InitializeEntityCollectionProviderResolver(MerchelloContext.Current);

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

            //if (afterStartup != null)
            //    afterStartup(MerchelloContext.Current);

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

            //if (afterComplete != null)
            //{
            //    afterComplete(MerchelloContext.Current);
            //}

            this._isComplete = true;

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
        }

        /// <summary>
        /// The initializes the AutoMapper mappings.
        /// </summary>
        protected void InitializeAutoMapperMappers()
        {
            var container = SR.Current.Container;

            Mapper.Initialize(configuration =>
                {
                    foreach (var mc in container.GetAllInstances<MerchelloMapperConfiguration>())
                    {
                        mc.ConfigureMappings(configuration);
                    }
                });
        }

        /// <summary>
        /// Creates the MerchelloPluginContext (singleton)
        /// </summary>
        /// <param name="serviceContext">The service context</param>
        /// <param name="cache">The cache helper</param>
        /// <remarks>
        /// Since we load fire our boot manager after Umbraco fires its "started" even, Merchello gets the benefit
        /// of allowing Umbraco to manage the various caching providers via the Umbraco CoreBootManager or WebBootManager
        /// depending on the context.
        /// </remarks>
        /// QFIX-from   (IServiceContext serviceContext, CacheHelper cache)
        /// QFIX-to     (IServiceContext serviceContext, object cache)
        protected void CreateMerchelloContext(IServiceContext serviceContext, object cache)
        {
            //var gateways = new GatewayContext(serviceContext, GatewayProviderResolver.Current);
            //_merchelloContext = MerchelloContext.Current = new MerchelloContext(serviceContext, gateways, cache);
        }

        ///// <summary>
        ///// Initializes the <see cref="CurrencyContext"/>.
        ///// </summary>
        ///// <param name="storeSettingService">
        ///// The store setting service.
        ///// </param>
        //protected virtual void InitialCurrencyContext(IStoreSettingService storeSettingService)
        //{
        //    CurrencyContext.Current = new CurrencyContext(storeSettingService);
        //}

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
            //// initialize the DetachedPublishedPropertyConverter singleton
            //if (!DetachedValuesConverter.HasCurrent)
            //    DetachedValuesConverter.Current = new DetachedValuesConverter(ApplicationContext.Current, PluginManager.Current.ResolveDetachedValueOverriders());
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
        /// Responsible for the special case initialization of the gateway resolver.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <remarks>
        /// This is a special case due to the fact we need this singleton instantiated prior to 
        /// building the <see cref="MerchelloContext"/>
        /// </remarks>
        //// FYI changed [ CacheHelper cache ] param to [ object cache]
        private void InitializeGatewayResolver(IServiceContext serviceContext, object cache)
        {
            //_logger.Info<CoreBootManager>("Initializing Merchello GatewayResolver");

            //if (!GatewayProviderResolver.HasCurrent)
            //    GatewayProviderResolver.Current = new GatewayProviderResolver(
            //    PluginManager.Current.ResolveGatewayProviders(),
            //    serviceContext.GatewayProviderService,
            //    cache.RuntimeCache);
        }

        /// <summary>
        /// The initialize entity collection provider resolver.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// REFACTOR - start changing use of 'merchelloContext' to mc -> e.g.  mc.Current.Servies - for convention
        private void InitializeEntityCollectionProviderResolver(IMerchelloContext merchelloContext)
        {
            //if (!EntityCollectionProviderResolver.HasCurrent)
            //{
            //    LogHelper.Info<CoreBootManager>("Initializing EntityCollectionProviders");

            //    EntityCollectionProviderResolver.Current = new EntityCollectionProviderResolver(
            //       PluginManager.Current.ResolveEnityCollectionProviders(),
            //       merchelloContext);                 
            //}
        }

        /// <summary>
        /// Sets up unit of work provider.
        /// </summary>
        private void SetUnitOfWorkProvider()
        {
            //var connString = ConfigurationManager.ConnectionStrings[MerchelloConfiguration.Current.Section.DefaultConnectionStringName].ConnectionString;
            //var providerName = ConfigurationManager.ConnectionStrings[MerchelloConfiguration.Current.Section.DefaultConnectionStringName].ProviderName;
            //_unitOfWorkProvider = new PetaPocoUnitOfWorkProvider(_logger, connString, providerName);
        }
    }
}
