using System;
using System.Configuration;
using System.Web;
using Merchello.Core.Configuration;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Mappers;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core
{
    internal class CoreBootManager : IBootManager
    {
        private DisposableTimer _timer;
        private bool _isInitialized = false;
        private readonly MerchelloPluginBase _merchelloMerchello;
        private MerchelloPluginContext MerchelloPluginContext { get; set; }
        protected CacheHelper MerchelloCache { get; set; }


        public CoreBootManager(MerchelloPluginBase merchelloMerchello)
        {
            Mandate.ParameterNotNull(merchelloMerchello, "merchelloPlugin");

            _merchelloMerchello = merchelloMerchello;
        }

        public virtual IBootManager Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException("The boot manager has already been initialized");

            _timer = DisposableTimer.DebugDuration<CoreBootManager>("Merchello package starting", "Merchello package startup complete");

            // create the database and service contexts for the MerchelloAppContext
            
            Database.Mapper = new PetaPocoMapper();
            //var dbContext = new PluginDatabaseContext(dbFactory);

            var connString = ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName].ConnectionString;
            var providerName = ConfigurationManager.ConnectionStrings[PluginConfiguration.Section.DefaultConnectionStringName].ProviderName;

            var serviceContext = new ServiceContext(new PetaPocoUnitOfWorkProvider(connString, providerName));

            return null;
        }

        /// <summary>
        /// Creates and assigns the ApplicationCache based on a new instance of System.Web.Caching.Cache
        /// </summary>
        protected virtual void CreateApplicationCache()
        {
            var cacheHelper = new CacheHelper(HttpContext.Current.Cache);

            MerchelloCache = cacheHelper;
        }

        public IBootManager Complete(Action<MerchelloPluginContext> afterComplete)
        {
            throw new NotImplementedException();
        }



        public IBootManager Startup(Action<MerchelloPluginContext> afterStartup)
        {
            throw new NotImplementedException();
        }
    }
}
