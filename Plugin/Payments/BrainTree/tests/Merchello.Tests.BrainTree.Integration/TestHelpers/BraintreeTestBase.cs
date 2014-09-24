using Merchello.Tests.Base.TestHelpers;

namespace Merchello.Tests.Braintree.Integration.TestHelpers
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Plugin.Payments.Braintree.Models;

    using NUnit.Framework;

    using umbraco.cms.businesslogic;

    public abstract class BraintreeTestBase : MerchelloAllInTestBase
    {
        protected BraintreeProviderSettings BraintreeProviderSettings;

        protected ICustomer TestCustomer;

        protected Guid CustomerKey = new Guid("1A6E8170-9CB9-41C0-B944-36F16B97BED2");

        protected BraintreeGateway Gateway;


        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
            TestCustomer = MerchelloContext.Current.Services.CustomerService.CreateCustomerWithKey(
                Guid.NewGuid().ToString(),
                "debug",
                "debug",
                "debug@debug.com");

            BraintreeProviderSettings = TestHelper.GetBraintreeProviderSettings();

            AutoMapperMappings.CreateMappings();

            Gateway = BraintreeProviderSettings.AsBraintreeGateway();

        }
    }
}