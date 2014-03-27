using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
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

        /// <summary>
        /// Test verifies that both "Actived" and "Inactive"  Taxation GatewayProviders can be resolved and returned
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_AllTaxationProviders()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache);

            //// Act
            var providers = resolver.GetAllProviders<TaxationGatewayProviderBase>().ToArray();

            //// Assert
            Assert.IsTrue(providers.Any());
            Assert.IsTrue(providers.Any(x => x.Activated));
            Assert.IsTrue(providers.Any(x => !x.Activated));
        }

        /// <summary>
        /// Test verifies that both "Actived" and "Inactive" Shipping GatewayProviders can be resolved and returned
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_AllShippingProviders()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache);

            //// Act
            var providers = resolver.GetAllProviders<ShippingGatewayProviderBase>().ToArray();

            //// Assert
            Assert.IsTrue(providers.Any());
            Assert.IsTrue(providers.Any(x => x.Activated));
            Assert.IsTrue(providers.Any(x => !x.Activated));
        }

        /// <summary>
        /// Test verifies that both "Actived" and "Inactive" Payment GatewayProviders can be resolved and returned
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_AllPaymentProviders()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache);

            //// Act
            var providers = resolver.GetAllProviders<PaymentGatewayProviderBase>().ToArray();

            //// Assert
            Assert.IsTrue(providers.Any());
            Assert.IsTrue(providers.Any(x => x.Activated));
            Assert.IsTrue(providers.Any(x => !x.Activated));
        }
    }
}
