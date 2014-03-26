using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Cache;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Taxation;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [TestFixture]
    public class GatewayProviderResolution : DatabaseIntegrationTestBase
    {

        [Test]
        public void Can_Resolve_GatewayProvidersByType()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(PreTestDataWorker.GatewayProviderService, new NullCacheProvider());

            //// Act
            var types = resolver.GetActiveProviders<TaxationGatewayProviderBase>();

            //// Assert
            Assert.IsTrue(types.Any());
        }

    }
}
