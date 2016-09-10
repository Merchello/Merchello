namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using System.Configuration;
    using System.IO;
    using System.Net.Configuration;

    using Merchello.Core.Configuration.Sections;
    using Merchello.Tests.Unit.TestHelpers;

    using NUnit.Framework;

    public abstract class MerchelloSettingsTests
    {
        protected virtual bool TestingDefaults
        {
            get { return false; }
        }

        [OneTimeSetUp]
        public void Init()
        {
            var config = new FileInfo(TestHelper.MapPathForTest("~/Configurations/MerchelloSettings/web.config"));

            var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = config.FullName };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (TestingDefaults)
            {
                SettingsSection = configuration.GetSection("merchello/defaultSettings") as MerchelloSettingsSection;
            }
            else
            {
                SettingsSection = configuration.GetSection("merchello/settings") as MerchelloSettingsSection;
            }



            //Assert.IsNotNull(SettingsSection);
        }

        protected IMerchelloSettingsSection SettingsSection { get; private set; }
    }
}