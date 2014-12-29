using System;
using Examine;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways;
using Merchello.Core.Observation;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.Base.TestHelpers;
using Merchello.Web;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    public abstract class DatabaseIntegrationTestBase
    {
        protected IMerchelloContext MerchelloContext { get; private set; }
        private DbPreTestDataWorker _dbPreTestDataWorker;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            var serviceContext = new ServiceContext(new PetaPocoUnitOfWorkProvider());

            _dbPreTestDataWorker = new DbPreTestDataWorker(serviceContext);


            if (!GatewayProviderResolver.HasCurrent)    
            GatewayProviderResolver.Current = new GatewayProviderResolver(
            PluginManager.Current.ResolveGatewayProviders(),
            serviceContext.GatewayProviderService,
            new NullCacheProvider());                
        


            MerchelloContext = new MerchelloContext(serviceContext,
                new GatewayContext(serviceContext, GatewayProviderResolver.Current),
                new CacheHelper(new NullCacheProvider(),
                                    new NullCacheProvider(),
                                    new NullCacheProvider()));

            if (!TriggerResolver.HasCurrent)
                TriggerResolver.Current = new TriggerResolver(PluginManager.Current.ResolveObservableTriggers());

            if (!MonitorResolver.HasCurrent)
                MonitorResolver.Current = new MonitorResolver(MerchelloContext.Gateways.Notification, PluginManager.Current.ResolveObserverMonitors());

            AutoMapperMappings.CreateMappings();
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();  
            ExamineManager.Instance.IndexProviderCollection["MerchelloCustomerIndexer"].RebuildIndex();
            
        }

        protected DbPreTestDataWorker PreTestDataWorker {
            get { return _dbPreTestDataWorker; }
        }

    }
}
