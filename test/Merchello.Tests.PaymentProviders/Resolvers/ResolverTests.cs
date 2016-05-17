namespace Merchello.Tests.PaymentProviders.Resolvers
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Providers.Payment.PayPal.Provider;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    using Umbraco.Core;

    [TestFixture]
    public class ResolverTests : MerchelloAllInTestBase
    {
        [Test]
        public void Can_Resolve_Types()
        {
            var resolved = PluginManager.Current.ResolveTypesWithAttribute<GatewayProviderBase, GatewayProviderActivationAttribute>(false);


            Assert.IsTrue(resolved.Any());
        }

        [Test]
        public void Can_Resolve_Payment_Providers()
        {
            //// Arrange
            var expected = 3;

            //// Act
            var providers = MerchelloContext.Current.Gateways.Payment.GetAllProviders();

            //// Assert
            Assert.AreEqual(expected, providers.Count());
        }
    }
}