namespace Merchello.Tests.Unit.Configurations.MerchelloCountries
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class CountriesElementTests : MerchelloCountriesTests
    {
        [Test]
        public void Countries()
        {
            Assert.NotNull(CountriesSection.Countries);
            Assert.IsTrue(CountriesSection.Countries.Any());
        }

        //// Arrange


        //// Act


        //// Assert
    }
}