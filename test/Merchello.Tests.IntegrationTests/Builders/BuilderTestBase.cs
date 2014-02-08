using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.Builders
{
    public class BuilderTestBase : DatabaseIntegrationTestBase
    {
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            MerchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));

        }

        protected IMerchelloContext MerchelloContext { get; private set; }
    }
}