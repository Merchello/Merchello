namespace Merchello.Tests.IntegrationTests.Services.CampaignSettings
{
    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class CampaignSettingsServiceTests : MerchelloAllInTestBase
    {
        private ICampaignSettingsService _campaignSettingsService;

        public void Init()
        {
            _campaignSettingsService = MerchelloContext.Current.Services.CampaignSettingsService;
        }
    }
}