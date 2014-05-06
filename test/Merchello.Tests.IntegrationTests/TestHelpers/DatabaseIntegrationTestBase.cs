using Examine;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Web;
using Moq;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    public abstract class DatabaseIntegrationTestBase
    {
        protected IMerchelloContext MerchelloContext { get; private set; }
        
        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            var mockGatewayProviderResolver = new Mock<IGatewayProviderResolver>(MockBehavior.Loose);
            var gatewayContextMock = new Mock<IGatewayContext>(MockBehavior.Default);

            var serviceContext = new ServiceContext(new PetaPocoUnitOfWorkProvider());
            MerchelloContext = new MerchelloContext(serviceContext,
                gatewayContextMock.Object,
                new CacheHelper(new NullCacheProvider(),
                                    new NullCacheProvider(),
                                    new NullCacheProvider()));
            AutoMapperMappings.BindMappings();
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();  
        }

        protected DbPreTestDataWorker PreTestDataWorker {
            get { return new DbPreTestDataWorker(); }
        }

    }
}
