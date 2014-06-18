using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using Merchello.Core.Cache;
using Merchello.Core.Configuration;
using Merchello.Core.Gateways;
using Merchello.Core.Models.MonitorModels;
using Merchello.Core.Observation;
using Merchello.Core.Services;
using Umbraco.Core;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core.Logging;


namespace Merchello.Core
{
    /// <summary>
    /// A bootstrapper for the Merchello Plugin which initializes all objects to be used in the Merchello Core
    /// </summary>
    /// <remarks>
    /// We needed our own bootstrapper to setup Merchello specific singletons
    /// </remarks>
    internal class CoreBootManager : BootManagerBase, IBootManager
    {
        private DisposableTimer _timer;
        private bool _isInitialized;
        private bool _isStarted;
        private bool _isComplete;
        private bool _isTest;

        private MerchelloContext _merchelloContext;
        private PetaPocoUnitOfWorkProvider _unitOfWorkProvider;
        
        public override IBootManager Initialize()
        {
            if (_isInitialized)
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

            _isInitialized = true;            

            return this;
        }
       

        /// <summary>
        /// Creates the MerchelloPluginContext (singleton)
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="cache"></param>
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


        private void InitializeGatewayResolver(IServiceContext serviceContext, CacheHelper cache)
        {            
            if(!GatewayProviderResolver.HasCurrent)
            GatewayProviderResolver.Current = new GatewayProviderResolver(
            PluginManager.Current.ResolveGatewayProviders(),
            serviceContext.GatewayProviderService,
            cache.RuntimeCache);                       
        }

        protected virtual void InitializeResolvers()
        {
            if(!TriggerResolver.HasCurrent)
            TriggerResolver.Current = new TriggerResolver(PluginManager.Current.ResolveObservableTriggers());

            if(!MonitorResolver.HasCurrent)
            MonitorResolver.Current = new MonitorResolver(MerchelloContext.Current.Gateways.Notification, PluginManager.Current.ResolveObserverMonitors());
        }

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

        //protected void BindEventTriggers()
        //{
        //    LogHelper.Info<CoreBootManager>("Beginning Merchello Trigger Binding");
        //    foreach (var trigger in TriggerResolver.Current.GetAllTriggers())
        //    {
        //        var att = trigger.GetType().GetCustomAttributes<TriggerForAttribute>(false).FirstOrDefault();

        //        if (att == null) continue;

        //        var bindTo = att.Type.GetEvent(att.HandleEvent);

        //        if (bindTo == null) continue;

        //        var mi = trigger.GetType().GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);

        //        bindTo.AddEventHandler(trigger, Delegate.CreateDelegate(bindTo.EventHandlerType, trigger, mi));

        //        LogHelper.Info<CoreBootManager>(string.Format("Binding {0} to {1} - {2} event", trigger.GetType().Name, att.Type.Name, att.HandleEvent));
        //    }
        //}

        /// <summary>
        /// Fires after initialization and calls the callback to allow for customizations to occur
        /// </summary>
        /// <param name="afterStartup"></param>
        public override IBootManager Startup(Action<MerchelloContext> afterStartup)
        {
            if (_isStarted)
                throw new InvalidOperationException("The boot manager has already been initialized");

            if (afterStartup != null)
                afterStartup(MerchelloContext.Current);

            _isStarted = true;

            return this;
        }

        /// <summary>
        /// Fires after startup and calls the callback once customizations are locked
        /// </summary>
        public override IBootManager Complete(Action<MerchelloContext> afterComplete)
        {
            if(_isComplete)
                throw new InvalidOperationException("The boot manager has already been completed");
            
            if (afterComplete != null)
            {
                afterComplete(MerchelloContext.Current);                
            }

            _isComplete = true;

            return this;
        }

        internal bool IsStarted
        {
            get { return _isStarted; }
        }


        internal bool IsInitialized
        {
            get { return _isInitialized; }
        }

        /// <summary>
        /// Flag for unit testing
        /// </summary>
        internal bool IsUnitTest
        {
            get { return _isTest; }
            set { _isTest = value; }
        }

    }
}
