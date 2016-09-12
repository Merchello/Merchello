namespace Merchello.Tests.Unit.Configurations.MerchelloCountries
{
    using System.Configuration;
    using System.IO;

    using Merchello.Core.Configuration.Sections;
    using Merchello.Tests.Unit.TestHelpers;

    using NUnit.Framework;

    public class MerchelloCountriesTests
    {

        [OneTimeSetUp]
        public void Init()
        {
            var config = new FileInfo(TestHelper.MapPathForTest("~/Configurations/MerchelloCountries/web.config"));

            var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = config.FullName };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


            this.CountriesSection = configuration.GetSection("merchello/merchelloCountries") as MerchelloCountriesSection;

            Assert.That(this.CountriesSection, Is.Not.Null, "Countries section was null");
        }

        protected IMerchelloCountriesSection CountriesSection { get; private set; }
    }
}