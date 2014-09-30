using Merchello.Plugin.Payments.Braintree.Services;

namespace Merchello.Tests.Braintree.Integration.Services
{
    using System;
    using System.Linq;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Tests.Braintree.Integration.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class BraintreeCustomerServiceTests : BraintreeTestBase
    {
        private IBraintreeCustomerApiService _braintreeCustomerApiService;

        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            base.TestFixtureSetup();

            this._braintreeCustomerApiService = new BraintreeCustomerApiService(MerchelloContext.Current, BraintreeProviderSettings);   
        }

        /// <summary>
        /// Test asserts that a customer can be retrieved from the API
        /// </summary>
        [Test]
        public void Can_Get_Customer_From_Service()
        {
            //// Arrange
            
            //// Act
            var customer = this._braintreeCustomerApiService.GetBraintreeCustomer(TestCustomer);

            Assert.NotNull(customer);

        }


        /// <summary>
        /// Shows that a client request token can be retrieved for an AnonymousCustomer
        /// </summary>
        [Test]
        public void Can_Generate_A_ClientToken_For_An_AnonymousCustomer()
        {
            var token = this._braintreeCustomerApiService.GenerateClientRequestToken();

            Console.Write(token);
            Assert.IsNotNullOrEmpty(token);
        }

        /// <summary>
        /// Shows that a client request token can be retrieved for an existing customer
        /// </summary>
        [Test]
        public void Can_Generate_A_ClientToken_For_A_Customer()
        {
            var token = this._braintreeCustomerApiService.GenerateClientRequestToken(TestCustomer);

            Console.Write(token);
            Assert.IsNotNullOrEmpty(token);
        }

        ///// <summary>
        ///// Shows an address can be saved to a customer
        ///// </summary>
        //[Test]
        //public void Can_Save_A_Customer_Address()
        //{
        //    //// Arrange
        //    var address = new Core.Models.Address()
        //                      {
        //                          AddressType = AddressType.Billing,
        //                          Organization = "Mindfly",
        //                          Address1 = "114 W. Magnolia St. Suite 300",
        //                          Locality = "Bellingham",
        //                          Region = "WA",
        //                          PostalCode = "98225"
        //                      };

        //    //// Act
        //    var success = _braintreeCustomerApiProvider.Create(TestCustomer, TestHelper.PaymentMethodNonce, address);
        //    var customer = _braintreeCustomerApiProvider.GetBraintreeCustomer(TestCustomer);
        //    //// Assert
        //    Assert.IsTrue(success);
        //    Assert.IsTrue(customer.Addresses.Any());
        //    Assert.IsTrue(customer.PaymentMethods.Any());
        //    Assert.IsTrue(customer.CreditCards.Any());
        //}


    }
}