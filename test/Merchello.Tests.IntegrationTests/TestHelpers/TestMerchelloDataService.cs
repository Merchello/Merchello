using System;
using System.Configuration;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Observation;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Examine.DataServices;
using Merchello.Tests.Base.SqlSyntax;
using Moq;
using Umbraco.Core;


namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    using Merchello.Core.Events;
    using Merchello.Core.Persistence;

    using Umbraco.Core.Logging;

    internal class DataServiceMerchelloContext
    {
        public static IMerchelloContext GetMerchelloContext()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);
            // sets up the Umbraco SqlSyntaxProvider Singleton
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax(syntax);

            var sqlSyntax = SqlSyntaxProviderTestHelper.SqlSyntaxProvider(syntax);

            //AutoMapperMappings.CreateMappings();
            var logger = Logger.CreateWithDefaultLog4NetConfiguration();
            var cache = new CacheHelper(
                new ObjectCacheRuntimeCacheProvider(),
                new StaticCacheProvider(),
                new NullCacheProvider());

            var serviceContext = new ServiceContext(new RepositoryFactory(cache, logger, sqlSyntax), new PetaPocoUnitOfWorkProvider(new Mock<ILogger>().Object), new Mock<ILogger>().Object, new TransientMessageFactory());
            return  new MerchelloContext(serviceContext,
                new GatewayContext(serviceContext, GatewayProviderResolver.Current),
                new CacheHelper(new NullCacheProvider(),
                                    new NullCacheProvider(),
                                    new NullCacheProvider()));        
        }
    }

    public class TestMerchelloDataService : MerchelloDataService
    {
        public TestMerchelloDataService()
            : base(DataServiceMerchelloContext.GetMerchelloContext())
        {
        }
    }
}
    