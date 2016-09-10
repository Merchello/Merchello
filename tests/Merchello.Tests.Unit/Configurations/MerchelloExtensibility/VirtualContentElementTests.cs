namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using NUnit.Framework;

    [TestFixture]
    public class VirtualContentElementTests : MerchelloExtensibilityTests
    {
        [Test]
        public void Routing()
        {
            Assert.NotNull(ExtensibilitySection.Mvc.VirtualContent);
        }
    }
}