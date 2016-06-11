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
    using System.Configuration;

    using Merchello.Core.Events;
    using Merchello.Core.Persistence;
    using Merchello.Tests.Base.SqlSyntax;

    using Moq;

    using umbraco.BusinessLogic;

    using Umbraco.Core.Logging;
    using Umbraco.Web;

    public abstract class DatabaseIntegrationTestBase
    {
        //protected IMerchelloContext MerchelloContext { get; private set; }
        private DbPreTestDataWorker _dbPreTestDataWorker;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);

            // sets up the Umbraco SqlSyntaxProvider Singleton OBSOLETE
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax(syntax);

            var sqlSyntax = SqlSyntaxProviderTestHelper.SqlSyntaxProvider(syntax);

            //AutoMapperMappings.CreateMappings();
            var logger = Logger.CreateWithDefaultLog4NetConfiguration();
            var serviceContext = new ServiceContext(new RepositoryFactory(logger, sqlSyntax), new PetaPocoUnitOfWorkProvider(logger), logger, new TransientMessageFactory());

            _dbPreTestDataWorker = new DbPreTestDataWorker(serviceContext);

            // Umbraco Application
            var applicationMock = new Mock<UmbracoApplication>();

            // Merchello CoreBootStrap
            var bootManager = new Web.WebBootManager(logger, _dbPreTestDataWorker.SqlSyntaxProvider);
            bootManager.Initialize();


            if (MerchelloContext.Current == null) Assert.Ignore("MerchelloContext.Current is null");


            //if (!GatewayProviderResolver.HasCurrent)
            //    GatewayProviderResolver.Current = new GatewayProviderResolver(
            //    PluginManager.Current.ResolveGatewayProviders(),
            //    serviceContext.GatewayProviderService,
            //    new NullCacheProvider());                



            //MerchelloContext = new MerchelloContext(serviceContext,
            //    new GatewayContext(serviceContext, GatewayProviderResolver.Current),
            //    new CacheHelper(new NullCacheProvider(),
            //                        new NullCacheProvider(),
            //                        new NullCacheProvider()));

            //if (!TriggerResolver.HasCurrent)
            //    TriggerResolver.Current = new TriggerResolver(PluginManager.Current.ResolveObservableTriggers());

            //if (!MonitorResolver.HasCurrent)
            //    MonitorResolver.Current = new MonitorResolver(MerchelloContext.Gateways.Notification, PluginManager.Current.ResolveObserverMonitors());

            
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();  
            ExamineManager.Instance.IndexProviderCollection["MerchelloCustomerIndexer"].RebuildIndex();
            
        }

        protected DbPreTestDataWorker PreTestDataWorker {
            get { return _dbPreTestDataWorker; }
        }

    }
}
