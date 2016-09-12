namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using NUnit.Framework;

    [TestFixture]
    public class ViewsElementTests : MerchelloExtensibilityTests
    {
        [Test]
        public void DefaultPath()
        {
            const string expected = "~/Views/Merchello/";

            Assert.NotNull(ExtensibilitySection.Mvc.Views.DefaultPath);
            Assert.AreEqual(expected, ExtensibilitySection.Mvc.Views.DefaultPath);

        }

        [Test]
        public void Notifications()
        {
            const string expected = "~/Views/Merchello/Notifications/";

            Assert.NotNull(ExtensibilitySection.Mvc.Views.Notifications);
            Assert.AreEqual(expected, ExtensibilitySection.Mvc.Views.Notifications);
        }

    }
}