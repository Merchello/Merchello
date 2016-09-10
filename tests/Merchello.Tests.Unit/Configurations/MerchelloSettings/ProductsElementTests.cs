namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using NUnit.Framework;

    [TestFixture]
    public class ProductsElementTests : MerchelloSettingsTests
    {
        [Test]
        public void DefaultSkuSeparator()
        {
            //// Arrange
            const string expected = "-";

            //// Act
            var value = SettingsSection.Products.DefaultSkuSeparator;

            //// Assert
            Assert.Equals(expected, value);
        }
    }
}