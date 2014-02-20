using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.Taxation
{
    public class TaxationProviderTestBase : DatabaseIntegrationTestBase
    {
        protected IGatewayProviderService GatewayProviderService;
        protected IMerchelloContext MerchelloContext;

        [TestFixtureSetUp]
        public void FixtureInit()
        {
            var dtos = PreTestDataWorker.Database.Query<GatewayProviderDto>("SELECT * FROM merchGatewayProvider");

            if (!dtos.Any())
            {
                Assert.Ignore("Default GatewayProviders are not installed.");
            }

            GatewayProviderService = PreTestDataWorker.GatewayProviderService;

            MerchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));

        }
    }
}