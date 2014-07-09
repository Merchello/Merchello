namespace Merchello.Core
{
    using System;
    using System.Configuration;
    using Cache;
    using Configuration;
    using Gateways;
    using Observation;
    using Persistence.UnitOfWork;
    using Services;
    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// A bootstrapper for the Merchello Plugin which initializes all objects to be used in the Merchello Core
    /// </summary>
    /// <remarks>
    /// We needed our own boot strap to setup Merchello specific singletons
    /// </remarks>
    internal class CoreBootManager : BootManagerBase, IBootManager
    {
        #region Fields

        /// <summary>
        /// The timer.
        /// </summary>
        private DisposableTimer _timer;

        /// <summary>
        /// The is complete.
        /// </summary>
        private bool _isComplete;

        /// <summary>
        /// The merchello context.
        /// </summary>
        private MerchelloContext _merchelloContext;

        /// <summary>
        /// The peta poco unit of work provider.
        /// </summary>
        private PetaPocoUnitOfWorkProvider _unitOfWorkProvider;

        /// <summary>
        /// Gets a value indicating whether Merchello is started.
        /// </summary>
        internal bool IsStarted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Merchello is initialized.
        /// </summary>
        internal bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this is a unit test
        /// </summary>
        internal bool IsUnitTest { get; set; }

        #endregion

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

            _timer = DisposableTimer.DebugDuration<CoreBootManager>("Merchello starting", "Merchello startup complete");
 
            // create the service context for the MerchelloAppContext   
            var connString = ConfigurationManager.ConnectionStrings[MerchelloConfiguration.Current.Section.DefaultConnectionStringName].ConnectionString;
            var providerName = ConfigurationManager.ConnectionStrings[MerchelloConfiguration.Current.Section.DefaultConnectionStringName].ProviderName;

            _unitOfWorkProvider = new PetaPocoUnitOfWorkProvider(connString, providerName);

            var serviceContext = new ServiceContext(_unitOfWorkProvider);


            var cache = ApplicationContext.Current == null
                            ? new CacheHelper(
                                    new ObjectCacheRuntimeCacheProvider(),
                                    new StaticCacheProvider(),
                                    new NullCacheProvider())
                            : ApplicationContext.Current.ApplicationCache;
            
            InitializeGatewayResolver(serviceContext, cache);
            
            CreateMerchelloContext(serviceContext, cache);

            InitializeResolvers();

            InitializeObserverSubscriptions();

            IsInitialized = true;            

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
        public override IBootManager Startup(Action<MerchelloContext> afterStartup)
        {
            if (IsStarted)
                throw new InvalidOperationException("The boot manager has already been initialized");

            if (afterStartup != null)
                afterStartup(MerchelloContext.Current);

            IsStarted = true;

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
        public override IBootManager Complete(Action<MerchelloContext> afterComplete)
        {
            if (_isComplete)
                throw new InvalidOperationException("The boot manager has already been completed");

            if (afterComplete != null)
            {
                afterComplete(MerchelloContext.Current);
            }

            _isComplete = true;

            return this;
        }  
       
        /// <summary>
        /// Creates the MerchelloPluginContext (singleton)
        /// </summary>
        /// <param name="serviceContext">The sevice context</param>
        /// <param name="cache">The cache helper</param>
        /// <remarks>
        /// Since we load fire our boot manager after Umbraco fires its "started" even, Merchello gets the benefit
        /// of allowing Umbraco to manage the various caching providers via the Umbraco CoreBootManager or WebBootManager
        /// depending on the context.
        /// </remarks>
        protected void CreateMerchelloContext(ServiceContext serviceContext, CacheHelper cache)
        {
            var gateways = new GatewayContext(serviceContext, GatewayProviderResolver.Current);
            _merchelloContext = MerchelloContext.Current = new MerchelloContext(serviceContext, gateways, cache);
        }

        /// <summary>
        /// Responsible for initializing resolvers.
        /// </summary>
        protected virtual void InitializeResolvers()
        {
            if (!TriggerResolver.HasCurrent)
            TriggerResolver.Current = new TriggerResolver(PluginManager.Current.ResolveObservableTriggers());

            if (!MonitorResolver.HasCurrent)
            MonitorResolver.Current = new MonitorResolver(MerchelloContext.Current.Gateways.Notification, PluginManager.Current.ResolveObserverMonitors());
        }

        /// <summary>
        /// Responsible initializing observer subscriptions.
        /// </summary>
        protected virtual void InitializeObserverSubscriptions()
        {
            if (!TriggerResolver.HasCurrent || !MonitorResolver.HasCurrent) return;

            var monitors = MonitorResolver.Current.GetAllMonitors();

            LogHelper.Info<CoreBootManager>("Starting subscribing Monitors to Triggers");

            foreach (var monitor in monitors)
            {
                monitor.Subscribe(TriggerResolver.Current);
            }

            LogHelper.Info<Umbraco.Core.CoreBootManager>("Finished subscribing Monitors to Triggers");            
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
        private void InitializeGatewayResolver(IServiceContext serviceContext, CacheHelper cache)
        {
            if (!GatewayProviderResolver.HasCurrent)
                GatewayProviderResolver.Current = new GatewayProviderResolver(
                PluginManager.Current.ResolveGatewayProviders(),
                serviceContext.GatewayProviderService,
                cache.RuntimeCache);
        }

        ////protected void BindEventTriggers()
        ////{
        ////    LogHelper.Info<CoreBootManager>("Beginning Merchello Trigger Binding");
        ////    foreach (var trigger in TriggerResolver.Current.GetAllTriggers())
        ////    {
        ////        var att = trigger.GetType().GetCustomAttributes<TriggerForAttribute>(false).FirstOrDefault();

        ////        if (att == null) continue;

        ////        var bindTo = att.Type.GetEvent(att.HandleEvent);

        ////        if (bindTo == null) continue;

        ////        var mi = trigger.GetType().GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);

        ////        bindTo.AddEventHandler(trigger, Delegate.CreateDelegate(bindTo.EventHandlerType, trigger, mi));

        ////        LogHelper.Info<CoreBootManager>(string.Format("Binding {0} to {1} - {2} event", trigger.GetType().Name, att.Type.Name, att.HandleEvent));
        ////    }
        ////}      
    }
}
