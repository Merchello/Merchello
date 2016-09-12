namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using NUnit.Framework;

    [TestFixture]
    public class MigrationsElementTests : MerchelloSettingsTests
    {
        [Test]
        public void AutoUpdateDbSchema()
        {
            //// Arrange
            const bool expected = true;

            //// Act
            var value = SettingsSection.Migrations.AutoUpdateDbSchema;

            //// Assert
            Assert.AreEqual(expected, value);
        }
    }
}