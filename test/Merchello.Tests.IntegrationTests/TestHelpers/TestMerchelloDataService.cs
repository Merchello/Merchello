using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Examine.DataServices;
using Umbraco.Core;


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