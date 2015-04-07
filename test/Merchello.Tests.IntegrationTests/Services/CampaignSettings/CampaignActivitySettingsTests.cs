namespace Merchello.Tests.IntegrationTests.Services.CampaignSettings
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class CampaignActivitySettingsTests : MerchelloAllInTestBase
    {
        private ICampaignSettingsService _campaignSettingsService;

        private ICampaignSettings _campaignSettings;


        [TestFixtureSetUp]
        public override void FixtureSetup()
        { 
            base.FixtureSetup();

            _campaignSettingsService = MerchelloContext.Current.Services.CampaignSettingsService;

            var allSettings = _campaignSettingsService.GetAll();
            _campaignSettingsService.Delete(allSettings);

            _campaignSettings = _campaignSettingsService.CreateCampaignSettingsWithKey("Campaign 1", "campaign1");

        }

        /// <summary>
        /// Test verifies a campaign activity setting can be added to a campaign
        /// </summary>
        [Test]
        public void Can_Add_A_CampaignActivitySettings_To_A_CampaignSetting()
        {
            //// Arrage
            
            //// Act
            var activity = _campaignSettingsService.CreateCampaignActivitySettingsWithKey(
                _campaignSettings.Key,
                "Activity 1",
                "activity1",
                EnumTypeFieldConverter.CampaignActivity.Discount.TypeKey,
                DateTime.Now,
                DateTime.Now.AddDays(7));

            //// Assert
            Assert.NotNull(activity);
            Assert.IsTrue(activity.HasIdentity);
        }
    }
}