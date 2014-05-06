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
    public abstract class DatabaseIntegrationTestBase : MerchelloAllInTestBase
    {
        
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            
            //    var mockGatewayProviderResolver = new Mock<IGatewayProviderResolver>(MockBehavior.Loose);


            //    var serviceContext = new ServiceContext(new PetaPocoUnitOfWorkProvider());
            //    MerchelloContext = new MerchelloContext(serviceContext,
            //        new GatewayContext(
            //                new PaymentContext(serviceContext.GatewayProviderService, mockGatewayProviderResolver.Object),
            //                new NotificationContext(serviceContext.GatewayProviderService, mockGatewayProviderResolver.Object),
            //                new ShippingContext(serviceContext.GatewayProviderService, serviceContext.StoreSettingService, mockGatewayProviderResolver.Object),
            //                new TaxationContext(serviceContext.GatewayProviderService, mockGatewayProviderResolver.Object)
            //            ),
            //        new CacheHelper(new NullCacheProvider(),
            //                            new NullCacheProvider(),
            //                            new NullCacheProvider()));
            //    AutoMapperMappings.BindMappings();
            //    //ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();  
        }

        protected DbPreTestDataWorker PreTestDataWorker {
            get { return DbPreTestDataWorker; }
        }

    }
}
