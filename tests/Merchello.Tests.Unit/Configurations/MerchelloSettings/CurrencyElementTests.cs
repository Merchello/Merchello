namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class CurrencyElementTests : MerchelloSettingsTests
    {
        [Test]
        public void CurrencyFormats()
        {
            //// Arrange
            const int expected = 1;

            //// Act
            var value = SettingsSection.CurrencyFormats;

            //// Assert
            Assert.AreEqual(expected, value.Count());
        }
    }
}