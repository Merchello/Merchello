namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.WebApi;

    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The MarketingCampaignApiController.
    /// </summary>
    [PluginController("Merchello")]
    public class MarketingCampaignApiController : MerchelloApiController
    {
        /// <summary>
        /// The <see cref="CampaignSettingsService"/>.
        /// </summary>
        private readonly ICampaignSettingsService _campaignSettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingCampaignApiController"/> class.
        /// </summary>
        public MarketingCampaignApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingCampaignApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public MarketingCampaignApiController(IMerchelloContext merchelloContext)
            : this(merchelloContext, UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingCampaignApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        internal MarketingCampaignApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _campaignSettingsService = merchelloContext.Services.CampaignSettingsService;
        }

        /// <summary>
        /// The get active campaigns.
        /// </summary>
        /// <returns>
        /// The <see cref="CampaignSettingsDisplay"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<CampaignSettingsDisplay> GetActiveCampaigns()
        {
            return _campaignSettingsService.GetActive().Select(x => x.ToCampaignSettingsDisplay());
        }

        /// <summary>
        /// The get all campaigns.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CampainSettingsDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<CampaignSettingsDisplay> GetAllCampaigns()
        {
            return _campaignSettingsService.GetAll().Select(x => x.ToCampaignSettingsDisplay());
        }

        /// <summary>
        /// Gets a <see cref="CampaignSettingsDisplay"/> by it's unique key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CampaignSettingsDisplay"/>.
        /// </returns>
        [HttpGet]
        public CampaignSettingsDisplay GetCampaignSettingsByKey(Guid key)
        {
            return _campaignSettingsService.GetByKey(key).ToCampaignSettingsDisplay();
        }

        /// <summary>
        /// Saves a campaign setting.
        /// </summary>
        /// <param name="campaign">
        /// The campaign.
        /// </param>
        /// <returns>
        /// The <see cref="CampaignSettingsDisplay"/>.
        /// </returns>
        [HttpPost]
        public CampaignSettingsDisplay PostAddCampaignSettings(CampaignSettingsDisplay campaign)
        {
            return _campaignSettingsService.CreateCampaignSettingsWithKey(campaign.Name, campaign.Alias).ToCampaignSettingsDisplay();
        }

        /// <summary>
        /// The post update campaign setting.
        /// </summary>
        /// <param name="campaign">
        /// The campaign.
        /// </param>
        /// <returns>
        /// The <see cref="CampaignActivitySettings"/>.
        /// </returns>
        [HttpPost]
        public CampaignSettingsDisplay PostUpdateCampaignSetting(CampaignSettingsDisplay campaign)
        {
            var destination = _campaignSettingsService.GetByKey(campaign.Key);

            destination = campaign.ToCampaignSettings(destination);
            _campaignSettingsService.Save(destination);

            return destination.ToCampaignSettingsDisplay();
        }
    }
}