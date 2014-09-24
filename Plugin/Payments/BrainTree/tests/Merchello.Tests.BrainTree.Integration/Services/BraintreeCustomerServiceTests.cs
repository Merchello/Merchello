namespace Merchello.Tests.Braintree.Integration.Services
{
    using System;

    using Merchello.Core;
    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Plugin.Payments.Braintree.Api;
    using Merchello.Tests.Braintree.Integration.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class BraintreeCustomerServiceTests : BraintreeTestBase
    {
        private IBraintreeCustomerApiProvider _braintreeCustomerApiProvider;

        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            base.TestFixtureSetup();

            this._braintreeCustomerApiProvider = new BraintreeCustomerApiProvider(
                MerchelloContext.Current,
                BraintreeProviderSettings.AsBraintreeGateway());   
        }

        [Test]
        public void Can_Get_Customer_From_Service()
        {
            //// Arrange
            
            //// Act
            var customer = this._braintreeCustomerApiProvider.GetBraintreeCustomer(TestCustomer);

            Assert.NotNull(customer);

        }

        [Test]
        public void Can_Generate_A_ClientToken_For_An_AnonymousCustomer()
        {
            var token = this._braintreeCustomerApiProvider.GenerateClientRequestToken();

            Console.Write(token);
            Assert.IsNotNullOrEmpty(token);
        }

        [Test]
        public void Can_Generate_A_ClientToken_For_A_Customer()
        {
            var token = this._braintreeCustomerApiProvider.GenerateClientRequestToken(TestCustomer);

            Console.Write(token);
            Assert.IsNotNullOrEmpty(token);
        }


    }
}