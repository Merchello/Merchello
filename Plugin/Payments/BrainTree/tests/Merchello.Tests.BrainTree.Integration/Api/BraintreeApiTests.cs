using Merchello.Plugin.Payments.Braintree.Services;

namespace Merchello.Tests.Braintree.Integration.Api
{
    using System;

    using global::Braintree;

    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Tests.Braintree.Integration.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class BraintreeApiTests : BraintreeTestBase
    {

        [SetUp]
        public void Setup()
        {
            try
            {
                var customer = Gateway.Customer.Find(CustomerKey.ToString());

                Gateway.Customer.Delete(CustomerKey.ToString());
            }
            catch (Exception)
            {
                // skip it

            }
        }

        
        /// <summary>
        /// Verifies that a transaction request can be created
        /// </summary>
        [Test]
        public void Can_Create_A_Transaction_Request()
        {
            //// Arrange
            var gateway = BraintreeProviderSettings.AsBraintreeGateway();

            var request = new TransactionRequest()
                              {
                                  Amount = 100M,
                                  PaymentMethodNonce = TestHelper.PaymentMethodNonce
                              };

            //// Act
            var result = gateway.Transaction.Sale(request);

            //// Assert
            Assert.IsTrue(result.IsSuccess());
        }

        [Test]
        public void Can_Create_A_Customer()
        {
            //// Arrange
            var gateway = BraintreeProviderSettings.AsBraintreeGateway();

            var customerRequest = new CustomerRequest()
                                      {
                                          CustomerId = CustomerKey.ToString(),
                                          FirstName = "Rusty",
                                          LastName = "Swayne",
                                          Company = "Mindfly",
                                          Email = "rusty@mindfly.com",
                                          Website = "http://www.mindfly.com"
                                      };

            //// Act
            var result = gateway.Customer.Create(customerRequest);

            //// Assert
            Assert.IsTrue(result.IsSuccess());
        }

        [Test]
        public void Can_Generate_A_ClientTokenRequest_And_GetToken()
        {
            //// Arrange
            var factory = new BraintreeApiRequestFactory(BraintreeProviderSettings);

            var request = factory.CreateClientTokenRequest(Guid.Empty);

            //// Act
            var token = Gateway.ClientToken.generate(request);

            //// Assert
            Assert.IsNotNullOrEmpty(token);
            Console.Write(token);
        }

        [Test]
        public void Can_Generate_A_ClientTokenRequest_For_A_Customer_And_GetToken()
        {
            //// Arrange
            var factory = new BraintreeApiRequestFactory(BraintreeProviderSettings);

            var request = factory.CreateClientTokenRequest(TestCustomer.Key);

            //// Act
            var token = Gateway.ClientToken.generate(request);

            //// Assert
            Assert.IsNotNullOrEmpty(token);
            Console.Write(token);
        }
    }
}