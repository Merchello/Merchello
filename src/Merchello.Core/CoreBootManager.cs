using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Merchello.Core.Cache;
using Merchello.Core.Configuration;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Mappers;
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
        protected CacheHelper MerchelloCache { get; set; }


        public override IBootManager Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException("The Merchello core boot manager has already been initialized");

            OnMerchelloInit();

            _timer = DisposableTimer.DebugDuration<CoreBootManager>("Merchello plugin starting", "Merchello plugin startup complete");

            CreatePackageCache();

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
        protected void CreateMerchelloContext(ServiceContext serviceContext)
        {           
            MerchelloContext = MerchelloContext.Current = new MerchelloContext(serviceContext, MerchelloCache);
        }


        /// <summary>
        /// Creates and assigns the ApplicationCache based on a new instance of System.Web.Caching.Cache
        /// </summary>        
        protected virtual void CreatePackageCache()
        {
            var cacheHelper = new CacheHelper(
                    new ObjectCacheRuntimeCacheProvider(),
                    new StaticCacheProvider(),
                    //we have no request based cache when not running in web-based context
                    new NullCacheProvider()
                );

            MerchelloCache = cacheHelper;
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
