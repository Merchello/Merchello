using Merchello.Plugin.Payments.Braintree.Services;

namespace Merchello.Tests.Braintree.Integration.Services
{
    using System;
    using System.Linq;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Tests.Braintree.Integration.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class BraintreeCustomerServiceTests : BraintreeTestBase
    {
        private IBraintreeCustomerApiService _braintreeCustomerApiService;

        private IAddress _address;

        /// <summary>
        /// Remove commented [Test] and run this method to delete all customers from Braintree
        /// </summary>
        //[Test]
        public void DeleteAllCustomers()
        {
            var customers = _braintreeCustomerApiService.GetAll();
            foreach (var customer in customers)
            {
                _braintreeCustomerApiService.Delete(((Customer)customer).Id);
            }
        }

        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            base.TestFixtureSetup();

            _braintreeCustomerApiService = new BraintreeCustomerApiService(MerchelloContext.Current, BraintreeProviderSettings);

            _address = new Core.Models.Address()
            {
                AddressType = AddressType.Billing,
                Organization = "Mindfly",
                Address1 = "114 W. Magnolia St. Suite 300",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US"
            };
        }

        /// <summary>
        /// Test asserts that a customer can be retrieved from the API
        /// </summary>
        [Test]
        public void Can_Get_Customer_From_Service()
        {
            //// Arrange
            
            //// Act
            var customer = _braintreeCustomerApiService.GetBraintreeCustomer(TestCustomer);

            Assert.NotNull(customer);

        }


        /// <summary>
        /// Shows that a client request token can be retrieved for an AnonymousCustomer
        /// </summary>
        [Test]
        public void Can_Generate_A_ClientToken_For_An_AnonymousCustomer()
        {
            var token = _braintreeCustomerApiService.GenerateClientRequestToken();

            Console.Write(token);
            Assert.IsNotNullOrEmpty(token);
        }

        /// <summary>
        /// Shows that a client request token can be retrieved for an existing customer
        /// </summary>
        [Test]
        public void Can_Generate_A_ClientToken_For_A_Customer()
        {
            var token = _braintreeCustomerApiService.GenerateClientRequestToken(TestCustomer);

            Console.Write(token);
            Assert.IsNotNullOrEmpty(token);
        }

        /// <summary>
        /// Shows an address can be saved to a customer
        /// </summary>
        [Test]
        public void Can_Save_A_Customer_Address()
        {
            //// Arrange


            //// Act
            var success = _braintreeCustomerApiService.Create(TestCustomer, TestHelper.PaymentMethodNonce, _address);
            var customer = _braintreeCustomerApiService.GetBraintreeCustomer(TestCustomer);
            var matching = customer.GetMatchingAddress(_address);

            //// Assert
            Assert.IsTrue(success, "Failed to create the customer");
            Assert.IsTrue(customer.Addresses.Any(), "Customer did not have any addresses");
            Assert.IsTrue(customer.PaymentMethods.Any(), "Customer did not have any payment methods");
            Assert.IsTrue(customer.CreditCards.Any(), "Customer did not have any credit cards");
            Assert.NotNull(matching, "A matching address was not found");
        }

        /// <summary>
        /// Test shows it is possible to get a list of all customers from the API
        /// </summary>
        [Test]
        public void Can_Get_A_List_Of_All_Cutomers()
        {
            //// Arrange
            
            //// Act
            var customers = _braintreeCustomerApiService.GetAll();

            //// Assert
            Assert.NotNull(customers);
            Assert.Greater(customers.MaximumCount, 0);
            Console.Write(customers.MaximumCount);
        }

    }
}