namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.WebApi;

    using Umbraco.Core.Persistence;
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
        private readonly IOfferProviderResolver _providerResolver;

        /// <summary>
        /// The offer component resolver.
        /// </summary>
        private readonly IOfferComponentResolver _componentResolver;

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
            this._providerResolver = OfferProviderResolver.Current;

            this._componentResolver = OfferComponentResolver.Current;
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
            this._providerResolver = OfferProviderResolver.Current;
        }

        #endregion

        /// <summary>
        /// Gets a collection of all offer providers.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="OfferProviderDisplay"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<OfferProviderDisplay> GetOfferProviders()
        {
            return
                OfferProviderResolver.Current.GetOfferProviders()
                    .Select(x => x.ToOfferProviderDisplay())
                    .OrderBy(x => x.BackOfficeTree.SortOrder);
        }

        /// <summary>
        /// Gets an offer settings by it's unique key
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="OfferSettingsDisplay"/>.
        /// </returns>
        [HttpGet]
        public OfferSettingsDisplay GetOfferSettings(Guid id)
        {
            return _offerSettingsService.GetByKey(id).ToOfferSettingsDisplay();
        }

        /// <summary>
        /// The get the available <see cref="OfferComponentDefinitionDisplay"/>s for a given offer provider key.
        /// </summary>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{OfferComponentDefinitionDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<OfferComponentDefinitionDisplay> GetAvailableOfferComponents(Guid offerProviderKey)
        {
            return
                _componentResolver.GetOfferComponentsByProviderKey(offerProviderKey)
                    .Select(x => x.ToOfferComponentDefinitionDisplay()).OrderBy(x => x.Name);
        }

        /// <summary>
        /// Returns a paged set of offers.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay SearchOffers(QueryDisplay query)
        {
            var term = query.Parameters.FirstOrDefault(x => x.FieldName == "term");
            
            var hasSearchTerm = term != null && !string.IsNullOrEmpty(term.Value);

            var page = hasSearchTerm ? 
                _offerSettingsService.GetPage(term.Value, query.CurrentPage + 1, query.ItemsPerPage, query.SortBy, query.SortDirection) : 
                _offerSettingsService.GetPage(query.CurrentPage + 1, query.ItemsPerPage, query.SortBy, query.SortDirection);

            return this.GetQueryResultDisplay(page);
        }

            /// <summary>
        /// Gets the collection of all active offer settings.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{OfferSettingsDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<OfferSettingsDisplay> GetAllOfferSettings()
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

            offerSettings.ApplySafeDates(settings);
                       
            _offerSettingsService.Save(offerSettings);

            return offerSettings.ToOfferSettingsDisplay();
        }

        /// <summary>
        /// Saves the offer settings
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="OfferSettingsDisplay"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the offer setting can not be found in the database.
        /// </exception>
        [HttpPost, HttpPut]
        public OfferSettingsDisplay PutUpdateOfferSettings(OfferSettingsDisplay settings)
        {
            var offerSettings = _offerSettingsService.GetByKey(settings.Key);
            if (offerSettings == null)
            {
                throw new NullReferenceException("OfferSettings was not found");
            }

            offerSettings = settings.ToOfferSettings(offerSettings);

            _offerSettingsService.Save(offerSettings);

            return offerSettings.ToOfferSettingsDisplay();
        }

        /// <summary>
        /// The delete offer settings.
        /// 
        /// DELETE /umbraco/Merchello/MarketingApi/{guid}
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>       
        [HttpGet, HttpDelete, HttpPost]
        public HttpResponseMessage DeleteOfferSettings(Guid id)
        {
            var offerSettings = _offerSettingsService.GetByKey(id);
            if (offerSettings == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            _offerSettingsService.Delete(offerSettings);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Checks if the offer code is unique.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// A valid indicating whether or not the offer code is unique.
        /// </returns>
        [HttpGet]
        public bool OfferCodeIsUnique(string offerCode = "")
        {
            if (string.IsNullOrEmpty(offerCode)) return false;
            return _offerSettingsService.OfferCodeIsUnique(offerCode);
        }

        /// <summary>
        /// Maps a <see cref="Page{IOfferSetting}" /> to a <see cref="QueryResultDisplay"/>.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        private QueryResultDisplay GetQueryResultDisplay(Page<IOfferSettings> page)
        {
            return new QueryResultDisplay()
                       {
                           CurrentPage = page.CurrentPage - 1,
                           ItemsPerPage = page.ItemsPerPage,
                           TotalItems = page.TotalItems,
                           TotalPages = page.TotalPages,
                           Items = page.Items.Select(x => x.ToOfferSettingsDisplay())
                       };
        }
    }
}