namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using System.Configuration;
    using System.IO;

    using Merchello.Core.Configuration.Sections;
    using Merchello.Tests.Unit.TestHelpers;

    using NUnit.Framework;

    public class MerchelloExtensibilityTests
    {
        protected virtual bool TestingDefaults
        {
            get { return false; }
        }

        [OneTimeSetUp]
        public void Init()
        {
            var config = new FileInfo(TestHelper.MapPathForTest("~/Configurations/MerchelloExtensibility/web.config"));

            var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = config.FullName };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (TestingDefaults)
            {
                this.ExtensibilitySection = configuration.GetSection("merchello/defaultExtensibilitySettings") as MerchelloExtensibilitySection;
            }
            else
            {
                this.ExtensibilitySection = configuration.GetSection("merchello/merchelloExtensibility") as MerchelloExtensibilitySection;
            }



            Assert.IsNotNull(this.ExtensibilitySection, "Settings section was null");
        }

        protected IMerchelloExtensibilitySection ExtensibilitySection { get; private set; }
    }
}