namespace Merchello.Tests.PaymentProviders.Braintree.Api
{
    using System;

    using global::Braintree;
    using Merchello.Providers.Payment.Braintree;
    using Merchello.Providers.Payment.Braintree.Services;
    using Merchello.Tests.PaymentProviders.Braintree.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class BraintreeApiTests : BraintreeTestBase
    {

        [SetUp]
        public void Setup()
        {
            try
            {
                var customer = this.Gateway.Customer.Find(this.CustomerKey.ToString());

                this.Gateway.Customer.Delete(this.CustomerKey.ToString());
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
            var gateway = this.BraintreeProviderSettings.AsBraintreeGateway();

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

        /// <summary>
        /// Verifies that a transaction request can be created. Does not touch 
        /// Umbraco/Merchello database. 
        /// </summary>
        [Test]
        public void Can_Find_A_Transaction()
        {
            //// Arrange
            var service = new BraintreeTransactionApiService(this.BraintreeProviderSettings);

            // set to sandbox transaction id
            string transactionId = "86xqrd";

            //// Act
            var attempt = service.Find(transactionId);

            //// Assert
            Assert.IsTrue(attempt.Success);
        }

        [Test]
        public void Can_Create_A_Customer()
        {
            //// Arrange
            var gateway = this.BraintreeProviderSettings.AsBraintreeGateway();

            var customerRequest = new CustomerRequest()
                                      {
                                          CustomerId = this.CustomerKey.ToString(),
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
            var factory = new BraintreeApiRequestFactory(this.BraintreeProviderSettings);

            var request = factory.CreateClientTokenRequest(Guid.Empty);

            //// Act
            var token = this.Gateway.ClientToken.generate(request);

            //// Assert
            Assert.IsNotNullOrEmpty(token);
            Console.Write(token);
        }

        [Test]
        public void Can_Generate_A_ClientTokenRequest_For_A_Customer_And_GetToken()
        {
            //// Arrange
            var factory = new BraintreeApiRequestFactory(this.BraintreeProviderSettings);
            this.BraintreeApiService.Customer.Create(this.TestCustomer);
            var request = factory.CreateClientTokenRequest(this.TestCustomer.Key);

            //// Act
            var token = this.Gateway.ClientToken.generate(request);

            //// Assert
            Assert.IsNotNullOrEmpty(token);
            Console.Write(token);
        }
    }
}