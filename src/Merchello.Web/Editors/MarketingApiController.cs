namespace Merchello.Web.Editors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.WebApi;

    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// An API controller for the Merchello marketing section.
    /// </summary>
    [PluginController("Merchello")]
    public class MarketingApiController : MerchelloApiController
    {
        /// <summary>
        /// The offer settings service.
        /// </summary>
        private readonly IOfferSettingsService _offerSettingsService;

        /// <summary>
        /// The offer provider resolver.
        /// </summary>
        private IOfferProviderResolver _resolver;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingApiController"/> class.
        /// </summary>
        public MarketingApiController()
            : this(Core.MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public MarketingApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _offerSettingsService = merchelloContext.Services.OfferSettingsService;
            _resolver = OfferProviderResolver.Current;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        internal MarketingApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _offerSettingsService = merchelloContext.Services.OfferSettingsService;

            // TODO - this need to be fixed to make testable
            _resolver = OfferProviderResolver.Current;
        }

        #endregion

        /// <summary>
        /// Gets the collection of all active offer settings.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{OfferSettingsDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<OfferSettingsDisplay> GetOfferSettings()
        {
            var offers = _offerSettingsService.GetAllActive(false);
            return offers.Select(x => x.ToOfferSettingsDisplay());
        }

        /// <summary>
        /// Adds a new offer settings record to the database.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="OfferSettingsDisplay"/>.
        /// </returns>
        [HttpPost]
        public OfferSettingsDisplay PostAddOfferSettings(OfferSettingsDisplay settings)
        {

            var offerSettings = _offerSettingsService.CreateOfferSettings(
                settings.Name,
                settings.OfferCode,
                settings.OfferProviderKey, 
                settings.ComponentDefinitions.AsOfferComponentDefinitionCollection());

            offerSettings.Active = settings.Active;
            // To do validate dates
            offerSettings.OfferStartsDate = settings.OfferStartsDate;
            offerSettings.OfferEndsDate = settings.OfferEndsDate;
           
            _offerSettingsService.Save(offerSettings);

            return offerSettings.ToOfferSettingsDisplay();
        }
    }
}