using Lucene.Net.Documents;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.ObjectResolution;
using Merchello.Tests.Base.SqlSyntax;
using Merchello.Web;
using NUnit.Framework;
using Umbraco.Core;
using CoreBootManager = Merchello.Core.CoreBootManager;


namespace Merchello.Tests.UnitTests.Contexts
{
    [TestFixture]
    public class MerchelloBootstrapperTests
    {
        private bool _initEventCalled;
        private bool _startingEventCalled;
        private bool _completedEventCalled;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {

            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();
        }

        [SetUp]
        public void Setup()
        {
             
            _initEventCalled = false;
            _startingEventCalled = false;
            _completedEventCalled = false;

            MerchelloContext.Current = null;

            BootManagerBase.MerchelloInit += delegate {
                _initEventCalled = true;
            };

            BootManagerBase.MerchelloStarting += delegate {
                _startingEventCalled = true;
            };

            BootManagerBase.MerchelloStarted += delegate {
                _completedEventCalled = true;
            };
        }

        ///// <summary>
        ///// Tests to verify if the <see cref="Core.CoreBootManager"/> can instantiate the MerchelloContext
        ///// </summary>
        //[Test]
        //public void Core_BootManager_Can_Create_MerchelloContext()
        //{
        //    MerchelloBootstrapper.Init(new CoreBootManager() { IsUnitTest = true });

        //    var context = MerchelloContext.Current;
        //    Assert.NotNull(context);
            
        //    var service = context.Services.CustomerService;
        //    Assert.NotNull(service);
             
        //    Assert.IsTrue(_initEventCalled);
        //    Assert.IsTrue(_startingEventCalled);
        //    Assert.IsTrue(_completedEventCalled);
        //}

        ///// <summary>
        ///// Tests to verify if the <see cref="WebBootManager"/> can instantiate the MerchelloContext
        ///// </summary>
        //[Test]
        //public void Web_BootManager_Can_Create_MerchelloContext()
        //{
        //    MerchelloBootstrapper.Init(new WebBootManager(true) { IsUnitTest = true });

        //    var context = MerchelloContext.Current;
        //    Assert.NotNull(context);

        //    var service = context.Services.CustomerService;
        //    Assert.NotNull(service);

        //    Assert.IsTrue(_initEventCalled);
        //    Assert.IsTrue(_startingEventCalled);
        //    Assert.IsTrue(_completedEventCalled);
        //}

        ///// <summary>
        ///// Test verifies that the Merchello configuration returns true
        ///// </summary>
        //[Test]
        //public void MerchelloContext_Returns_IsConfigured_True()
        //{
        //    MerchelloBootstrapper.Init(new WebBootManager(true) { IsUnitTest = true });

        //    var context = MerchelloContext.Current;

        //    Assert.IsTrue(context.IsConfigured);
        //}

    }
}
