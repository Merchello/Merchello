using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Taxation;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.UnitTests.ObjectResolution
{
    [TestFixture]
    public class GatewayProviderResolution
    {
        /// <summary>
        /// Tests confirms that a PaymentGatewayProvider can be resolved by type
        /// </summary>
        [Test]
        public void Can_Resolve_PaymentGatewayProviders()
        {
            //// Arrange
            
            //// Act
            var paymentProviders = PluginManager.Current.ResolveTypes<PaymentGatewayProviderBase>();

            //// Assert
            Assert.IsTrue(paymentProviders.Any());
        }

        /// <summary>
        /// Test confirms that all GatewayProviders can be resolved
        /// </summary>
        [Test]
        public void Can_Resolve_All_GatewayProviders()
        {
            //// Arrange

            //// Act
            var providers = PluginManager.Current.ResolveTypes<GatewayProviderBase>();

            //// Assert
            Assert.IsTrue(providers.Any());
        }

        /// <summary>
        /// Test confirms that ITaxationGatewayProvider can be resolved by the interface ITaxationGatewayProvider
        /// </summary>
        [Test]
        public void Can_Resolve_All_ITaxationGatewayProvider()
        {
            //// Arrange

            //// Act
            var providers = PluginManager.Current.ResolveTypes<ITaxationGatewayProvider>();

            //// Assert
            Assert.IsTrue(providers.Any());
        }


        
    }
}