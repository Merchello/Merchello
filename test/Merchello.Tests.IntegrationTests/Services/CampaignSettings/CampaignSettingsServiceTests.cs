namespace Merchello.Tests.IntegrationTests.Services.CampaignSettings
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class CampaignSettingsServiceTests : MerchelloAllInTestBase
    {
        private ICampaignSettingsService _campaignSettingsService;

        [SetUp]
        public void Init()
        {
            _campaignSettingsService = MerchelloContext.Current.Services.CampaignSettingsService;
            var allSettings = _campaignSettingsService.GetAll();
            _campaignSettingsService.Delete(allSettings);
        }

        /// <summary>
        /// Test validates a campaign setting can be added to the database
        /// </summary>
        [Test]
        public void Can_Add_A_CampaignSetting()
        {
            //// Arrange
            
            //// Act
            var settings = _campaignSettingsService.CreateCampaignSettingsWithKey("Test campaign", "test");

            //// Assert
            Assert.NotNull(settings);
            Assert.IsTrue(settings.HasIdentity);
            Assert.AreEqual("Test campaign", settings.Name);
            Assert.AreEqual("test", settings.Alias);
            Assert.IsTrue(settings.Active);
            Assert.IsFalse(settings.ActivitySettings.Any());
        }

        /// <summary>
        /// Test validates a campaign setting can be retrieved from the database by it's unique key.
        /// </summary>
        [Test]
        public void Can_Get_A_CampaignSettings_By_Key()
        {
            //// Arrange
            var expected = _campaignSettingsService.CreateCampaignSettingsWithKey("Test campaign", "test");
            var key = expected.Key;

            //// Act
            var settings = _campaignSettingsService.GetByKey(key);

            //// Assert
            Assert.NotNull(settings);
            Assert.AreEqual(expected.Name, settings.Name);
            Assert.AreEqual(expected.Alias, settings.Alias);
            Assert.AreEqual(expected.Active, settings.Active);
            Assert.AreEqual(expected.ActivitySettings.Count(), settings.ActivitySettings.Count());
        }
    }
}