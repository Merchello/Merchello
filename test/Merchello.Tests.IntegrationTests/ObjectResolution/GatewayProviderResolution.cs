using System;
using System.Linq;
using Merchello.Core.Gateways.Payment;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [TestFixture]
    public class GatewayProviderResolution : MerchelloAllInTestBase
    {

        [Test]
        public void Can_Retreive_PaymentGatewayProviders_From_Resolver()
        {
            //// Arrange
            // should be handled by the CoreBootManager in MerchelloAllInTestBase

            //// Act
            var providers = PaymentGatewayProviderResolver.Current.ProviderTypes;

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
