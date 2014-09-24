namespace Merchello.Tests.Braintree.Integration.TestHelpers
{
    using Merchello.Plugin.Payments.Braintree.Provider;

    using NUnit.Framework;

    public abstract class ProviderTestsBase : BraintreeTestBase
    {

        protected BraintreePaymentGatewayProvider Provider;

        
        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            base.TestFixtureSetup();



            // Sets Umbraco SqlSytax and ensure database is setup

        }
    }
}