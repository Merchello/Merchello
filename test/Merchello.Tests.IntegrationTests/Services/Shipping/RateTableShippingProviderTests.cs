using System.Linq;
using Merchello.Core;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Shipping
{
    [TestFixture]
    [Category("Shipping")]
    public class RateTableShippingProviderTests : ShippingProviderTestBase
    {
        [Test]
        public void Can_Create_And_Persist_A_ShipMethod_With_RateTable()
        {
            //// Arrange            
            // Get the RateTableShippingProvider
            var key = Constants.ProviderKeys.Shipping.RateTableShippingProviderKey;
            var provider = GatewayProviderService.GetGatewayProviderByKey(Constants.ProviderKeys.Shipping.RateTableShippingProviderKey);
            

        }
    }
}