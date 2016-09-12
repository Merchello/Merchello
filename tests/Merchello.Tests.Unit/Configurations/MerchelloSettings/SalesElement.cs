namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using NUnit.Framework;

    [TestFixture]
    public class SalesElementTests : MerchelloSettingsTests
    {
        [Test]
        public void AlwaysApproveOrderCreation()
        {
            //// Arrange
            const bool expected = false;

            //// Act
            var value = SettingsSection.Sales.AlwaysApproveOrderCreation;

            //// Assert
            Assert.AreEqual(expected, value);
        }

    }
}