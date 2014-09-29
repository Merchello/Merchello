using Merchello.Plugin.Payments.Chase.Models;
using Merchello.Plugin.Payments.Chase.Provider;
using Merchello.Tests.Base.TestHelpers;

namespace Merchello.Tests.Braintree.Integration.TestHelpers
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Models;

    using NUnit.Framework;             

    public abstract class ChaseTestBase : MerchelloAllInTestBase
    {
        protected ChaseProcessorSettings ChaseProcessorSettings;

        protected ICustomer TestCustomer;

        protected Guid CustomerKey = new Guid("1A6E8170-9CB9-41C0-B944-36F16B97BED2");

        protected ChasePaymentGatewayProvider Gateway;


        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
            TestCustomer = MerchelloContext.Current.Services.CustomerService.CreateCustomerWithKey(
                Guid.NewGuid().ToString(),
                "debug",
                "debug",
                "debug@debug.com");

            ChaseProcessorSettings = TestHelper.GetBraintreeProviderSettings();

            AutoMapperMappings.CreateMappings();

            Gateway = BraintreeProviderSettings.AsBraintreeGateway();

        }
    }
}