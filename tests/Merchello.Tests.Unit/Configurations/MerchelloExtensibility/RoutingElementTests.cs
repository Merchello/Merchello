namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class RoutingElementTests : MerchelloExtensibilityTests
    {
        [Test]
        public void ProductSlugRoutes()
        {
            var routes = ExtensibilitySection.Mvc.VirtualContent.Routing.ProductSlugRoutes;

            Assert.NotNull(routes);
            if (!TestingDefaults)
            {
                Assert.IsTrue(routes.Any());
            }
        }
    }
}