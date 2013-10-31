using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Services;
using Merchello.Examine.DataServices;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    public class TestMerchelloDataService : MerchelloDataService
    {
        public TestMerchelloDataService()
            : base(new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
               new CacheHelper(new NullCacheProvider(),
                                   new NullCacheProvider(),
                                   new NullCacheProvider())))
        {}
    }
}