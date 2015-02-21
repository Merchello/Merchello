namespace Merchello.Bazaar.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.WebApi;

    /// <summary>
    /// The bazaar site API controller.
    /// </summary>
    [PluginController("Bazaar")]
    [JsonCamelCaseFormatter]
    public class BazaarSiteApiController : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The _store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// The <see cref="ICurrency"/>.
        /// </summary>
        private ICurrency _currency;

        /// <summary>
        /// Initializes a new instance of the <see cref="BazaarSiteApiController"/> class.
        /// </summary>
        public BazaarSiteApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BazaarSiteApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public BazaarSiteApiController(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _merchelloContext = merchelloContext;
            _storeSettingService = _merchelloContext.Services.StoreSettingService;

            this.Initialize();
        }

        /// <summary>
        /// Returns a price for a product variant.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="optionChoiceKeys">
        /// The option choice keys.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation of the variant Price.
        /// </returns>
        [HttpGet]
        public string GetProductVariantPrice(Guid productKey, string optionChoiceKeys)
        {
            var optionsArray = optionChoiceKeys.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => new Guid(x)).ToArray();


            var product = _merchelloContext.Services.ProductService.GetByKey(productKey);
            var variant = _merchelloContext.Services.ProductVariantService.GetProductVariantWithAttributes(product, optionsArray);

            return ModelExtensions.FormatPrice(variant.OnSale ? variant.SalePrice.GetValueOrDefault() : variant.Price, _currency.Symbol);
        }

        /// <summary>
        /// Filters available product variant options.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="optionChoices">
        /// The option choices.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductVariantDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<ProductVariantDisplay> FilterOptionsBySelectedChoices(Guid productKey, string optionChoices)
        {
            var merchello = new MerchelloHelper(_merchelloContext.Services);

            var optionsArray = string.IsNullOrEmpty(optionChoices) ? new string[] { } : optionChoices.Split(',');
            var guidOptionChoices = new List<Guid>();

            foreach (var option in optionsArray)
            {
                if (!string.IsNullOrEmpty(option))
                {
                    guidOptionChoices.Add(new Guid(option));
                }
            }

            var variants = merchello.GetValidProductVariants(productKey, guidOptionChoices.ToArray());
            return variants.Where(x => x.Available);
        }

        /// <summary>
        /// Gets the collection of <see cref="IProvince"/> by country code.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProvince}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<IProvince> GetProvincesForCountry(string countryCode)
        {
            return _storeSettingService.GetCountryByCode(countryCode).Provinces;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            var setting = _storeSettingService.GetByKey(Core.Constants.StoreSettingKeys.CurrencyCodeKey);
            _currency = _storeSettingService.GetCurrencyByCode(setting.Value);
        }
    }
}