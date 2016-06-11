namespace Merchello.Tests.PaymentProviders.Braintree.Api
{
    using Merchello.Providers.Payment.Braintree;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Tests.PaymentProviders.Braintree.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class MappingAndFactoryTests : BraintreeTestBase
    {
        /// <summary>
        /// Verifies that the BraintreeProviderSettings can be mapped to a BraintreeGateway
        /// </summary>
        [Test]
        public void Can_Map_BrainTreeProviderSettings_To_BrainTreeGateway()
        {
            //// Arrange

            //// Act
            var gateway = this.BraintreeProviderSettings.AsBraintreeGateway();

            //// Assert
            Assert.NotNull(gateway);
            Assert.AreEqual(this.BraintreeProviderSettings.EnvironmentType.ToString().ToLower(), gateway.Environment.EnvironmentName);
            Assert.AreEqual(this.BraintreeProviderSettings.PublicKey, gateway.PublicKey);
            Assert.AreEqual(this.BraintreeProviderSettings.PrivateKey, gateway.PrivateKey);
            Assert.AreEqual(this.BraintreeProviderSettings.MerchantId, gateway.MerchantId);

            Assert.AreEqual(TransactionOption.SubmitForSettlement, this.BraintreeProviderSettings.DefaultTransactionOption);
        }

        /// <summary>
        /// Verifies that the MerchantDescriptor can be mapped to a DescriptorRequest
        /// </summary>
        [Test]
        public void Can_Create_A_DescriptorRequest()
        {
            //// Arrange

            //// Act
            var descriptorRequest = this.BraintreeProviderSettings.MerchantDescriptor.AsDescriptorRequest();

            //// Assert
            Assert.NotNull(descriptorRequest);
            Assert.AreEqual(this.BraintreeProviderSettings.MerchantDescriptor.Name, descriptorRequest.Name);
            Assert.AreEqual(this.BraintreeProviderSettings.MerchantDescriptor.Url, descriptorRequest.Url);
            Assert.AreEqual(this.BraintreeProviderSettings.MerchantDescriptor.Phone, descriptorRequest.Phone);
        }

    }
}