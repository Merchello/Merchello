using System;
using System.Linq;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [TestFixture]
    public class GatewayProviderResolution : MerchelloAllInTestBase
    {
        /// <summary>
        /// Test verifies that PaymentGatewayProviders can be resolved
        /// </summary>
        [Test]
        public void Can_Retreive_PaymentGatewayProviders_From_Resolver()
        {
            //// Arrange
            // Handled in base class instantiation

            //// Act
            var providers = PaymentGatewayProviderResolver.Current.ProviderTypes;

            //// Assert
            Assert.IsTrue(providers.Any());
        }

        /// <summary>
        /// Test verifies that ShippingGatewayProviders can be resolved
        /// </summary>
        [Test]
        public void Can_Retreive_ShippingGatewayProviders_From_Resolver()
        {
            //// Arrange
            // Handled in base class instantiation

            //// Act
            var providers = ShippingGatewayProviderResolver.Current.ProviderTypes;

            //// Assert
            Assert.IsTrue(providers.Any());
        }

        /// <summary>
        /// Test verifies that TaxationGatewayProviders can be resolved
        /// </summary>
        [Test]
        public void Can_Retreive_TaxationGatewayProviders_From_Resolver()
        {
            //// Arrange
            // Handled in base class instantiation

            //// Act
            var providers = TaxationGatewayProviderResolver.Current.ProviderTypes;

            //// Assert
            Assert.IsTrue(providers.Any());
        }

        [Test]
        public void Can_Retrieve_A_List_Of_Unresolved_Providers()
        {
            var typeNames = PaymentGatewayProviderResolver.Current.ProviderTypes.Select(x => x.AssemblyQualifiedName);

            foreach (var type in typeNames)
            {
                var parts = type.Split(',');
                Console.WriteLine(parts[0].Trim() + ", " + parts[1].Trim());
            }
        }
    }
}
