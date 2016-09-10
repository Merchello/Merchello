namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using NUnit.Framework;

    [TestFixture]
    public class MvcElementTests : MerchelloExtensibilitySectionTests
    {
        [Test]
        public void Views()
        {
            Assert.NotNull(ExtensibilitySection.Mvc.Views);
        }

        [Test]
        public void VirtualContent()
        {
            Assert.NotNull(ExtensibilitySection.Mvc.VirtualContent);
        }


    }
}