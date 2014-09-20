using Merchello.Plugin.Payments.Braintree;
using Merchello.Plugin.Payments.Braintree.Models;
using Merchello.Tests.Braintree.Integration.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.Braintree.Integration.Api
{
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
            var gateway = BraintreeProviderSettings.AsBraintreeGateway();

            //// Assert
            Assert.NotNull(gateway);
            Assert.AreEqual(BraintreeProviderSettings.Environment, gateway.Environment);
            Assert.AreEqual(BraintreeProviderSettings.PublicKey, gateway.PublicKey);
            Assert.AreEqual(BraintreeProviderSettings.PrivateKey, gateway.PrivateKey);
            Assert.AreEqual(BraintreeProviderSettings.MerchantId, gateway.MerchantId);

            Assert.AreEqual(TransactionOption.SubmitForSettlement, BraintreeProviderSettings.DefaultTransactionOption);
        }

        /// <summary>
        /// Verifies that the MerchantDescriptor can be mapped to a DescriptorRequest
        /// </summary>
        [Test]
        public void Can_Create_A_DescriptorRequest()
        {
            //// Arrange

            //// Act
            var descriptorRequest = BraintreeProviderSettings.MerchantDescriptor.AsDescriptorRequest();

            //// Assert
            Assert.NotNull(descriptorRequest);
            Assert.AreEqual(BraintreeProviderSettings.MerchantDescriptor.Name, descriptorRequest.Name);
            Assert.AreEqual(BraintreeProviderSettings.MerchantDescriptor.Url, descriptorRequest.Url);
            Assert.AreEqual(BraintreeProviderSettings.MerchantDescriptor.Phone, descriptorRequest.Phone);
        }

    }
}