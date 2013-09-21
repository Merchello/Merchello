using System;
using System.Configuration;
using Merchello.Core.Cache;
using Merchello.Core.Configuration;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;


namespace Merchello.Core
{
    /// <summary>
    /// A bootstrapper for the Merchello Plugin which initializes all objects to be used in the Merchello Core
    /// </summary>
    /// <remarks>
    /// We needed our own bootstrapper to resolve third party Plugins such as Payment, Taxation, and Shipping Providers and provision the MerchelloPluginContext.ServiceContext
    /// This does not provide any startup functionality relating to web objects
    /// </remarks>
    internal class CoreBootManager : BootManagerBase, IBootManager
    {
        private DisposableTimer _timer;
        private bool _isInitialized = false;
        private bool _isStarted = false;
        private bool _isComplete = false;
        
        private MerchelloContext MerchelloContext { get; set; }       

        
        public override IBootManager Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException("The Merchello core boot manager has already been initialized");

            OnMerchelloInit();

            _timer = DisposableTimer.DebugDuration<CoreBootManager>("Merchello plugin starting", "Merchello plugin startup complete");

 
            // create the service context for the MerchelloAppContext   
            var connString = ConfigurationManager.ConnectionStrings[MerchelloConfiguration.Current.Section.DefaultConnectionStringName].ConnectionString;
            var providerName = ConfigurationManager.ConnectionStrings[MerchelloConfiguration.Current.Section.DefaultConnectionStringName].ProviderName;                
            var serviceContext = new ServiceContext(new PetaPocoUnitOfWorkProvider(connString, providerName));
            

            CreateMerchelloContext(serviceContext);

            // TODO: this is where we need to resolve shipping, tax, and payment providers
            // TODO: Then wire in the Resolution

            _isInitialized = true;

            return this;
        }

                
        /// <summary>
        /// Creates the MerchelloPluginContext (singleton)
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <remarks>
        /// Since we load fire our boot manager after Umbraco fires its "started" even, Merchello gets the benefit
        /// of allowing Umbraco to manage the various caching providers via the Umbraco CoreBootManager or WebBootManager
        /// depending on the context.
        /// </remarks>
        protected void CreateMerchelloContext(ServiceContext serviceContext)
        {
           
            // TODO: Mock the ApplicationContext.  ApplicationContext should never be null but we need this for unit testing at this point  
            var cache = ApplicationContext.Current == null
                            ? new CacheHelper(
                                    new ObjectCacheRuntimeCacheProvider(),
                                    new StaticCacheProvider(),
                                    new NullCacheProvider())
                            : ApplicationContext.Current.ApplicationCache;

            MerchelloContext = MerchelloContext.Current = new MerchelloContext(serviceContext, cache);
        }

        /// <summary>
        /// Fires after initialization and calls the callback to allow for customizations to occur
        /// </summary>
        /// <param name="afterStartup"></param>
        /// <returns></returns>
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

            FreezeResolution();

            if (afterComplete != null)
            {
                afterComplete(MerchelloContext.Current);
                
            }

            _isComplete = true;

            MerchelloContext.IsReady = true;

            return this;
        }

        protected virtual void FreezeResolution()
        {
            Resolution.Freeze();
        }


    }
}
