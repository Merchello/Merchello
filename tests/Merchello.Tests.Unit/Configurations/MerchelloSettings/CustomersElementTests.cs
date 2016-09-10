namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class CustomersElementTests : MerchelloSettingsTests
    {
        [Test]
        public void AnonymousCustomersMaxDays()
        {
            //// Arrange
            const int expected = 30;

            //// Act
            var value = SettingsSection.Customers.AnonymousCustomersMaxDays;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void MemberTypes()
        {
            //// Arrange
            var compare = new[] { "customer", "Customer", "merchelloCustomer", "MerchelloCustomer" };
            var count = 4;

            //// Act
            var values = SettingsSection.Customers.MemberTypes.ToArray();


            //// Assert
            Assert.AreEqual(count, values.Count());

            var nonMatching = values.Where(x => compare.All(y => y != x));

            Assert.AreEqual(0, nonMatching.Count());
        }
    }
}