namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration.BackOffice;

    using NUnit.Framework;

    [TestFixture]
    public class FiltersElementTests : MerchelloSettingsTests
    {

        [Test]
        public void Products()
        {
            //// Arrange
            const int count = 1;
            var key = new Guid("4700456D-A872-4721-8455-1DDAC19F8C16");

            //// Act
            var values = SettingsSection.Filters.Products;

            //// Assert
            Assert.AreEqual(count, values.Count());

            var value = values.First();

            Assert.AreEqual(key, value.Key);
            Assert.IsNotEmpty(value.Title);
            Assert.IsNotEmpty(value.Icon);
            Assert.AreEqual(true, value.Visible);
        }
            
    }
}