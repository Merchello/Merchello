namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using System.Configuration;
    using System.IO;
    using System.Net.Configuration;

    using NUnit.Framework;

    public abstract class MerchelloSettingsTests
    {
        [SetUp]
        public void Init()
        {
            //var config = new FileInfo(TestHelper.MapPathForTest("~/Configurations/UmbracoSettings/web.config"));

            //var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = config.FullName };
            //var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            //if (TestingDefaults)
            //{
            //    SettingsSection = configuration.GetSection("umbracoConfiguration/defaultSettings") as UmbracoSettingsSection;
            //}
            //else
            //{
            //    SettingsSection = configuration.GetSection("umbracoConfiguration/settings") as UmbracoSettingsSection;
            //}



            //Assert.IsNotNull(SettingsSection);
        }

    }
}