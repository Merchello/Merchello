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
    using Umbraco.Core.Logging;
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
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

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

            _merchello = new MerchelloHelper(_merchelloContext.Services);

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
        /// The filter option choices.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="productAttributeKey">
        /// The product attribute key
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductOptionDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<ProductOptionDisplay> FilterOptionChoices(Guid productKey, Guid productAttributeKey)
        {
            var product = _merchello.Query.Product.GetByKey(productKey);

            if (product == null)
            {
                var nullReference = new NullReferenceException("Product with key " + productKey + " returned null");
                LogHelper.Error<BazaarSiteApiController>("MerchelloHelper failed to retrieve product.", nullReference);
                throw nullReference;
            }

            // TODO move this to a service
            
            var returnOptions = new List<ProductOptionDisplay>();

            // this is the option the was just used in a selection
            var activeOption = product.ProductOptions.FirstOrDefault(po => po.Choices.Any(c => c.Key == productAttributeKey));

            if (activeOption == null) return returnOptions;

            returnOptions.Add(activeOption);

            var variants = product.ProductVariants.Where(pv => pv.Available && pv.Attributes.Any(att => att.Key == productAttributeKey)).ToArray();

            var otherOptions = product.ProductOptions.Where(x => !x.Key.Equals(activeOption.Key)).ToArray();

            foreach (var option in otherOptions)
            {
                var addOption = new ProductOptionDisplay()
                                    {
                                        SortOrder = option.SortOrder,
                                        Key = option.Key,
                                        Name = option.Name
                                    };

                var optionChoices = new List<ProductAttributeDisplay>();

                foreach (var choice in option.Choices)
                {
                    if (ValidateOptionChoice(variants, choice.Key))
                    {
                        optionChoices.Add(new ProductAttributeDisplay()
                                            {
                                                Key = choice.Key,
                                                Name = choice.Name,
                                                Sku = choice.Sku,
                                                OptionKey = choice.OptionKey, 
                                                SortOrder = choice.SortOrder 
                                            });
                    }
                }

                addOption.Choices = optionChoices;

                returnOptions.Add(addOption);
            }

            return returnOptions;
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
        /// The validate option choice.
        /// </summary>
        /// <param name="variants">
        /// The variants.
        /// </param>
        /// <param name="productAttributeKey">
        /// The product attribute key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ValidateOptionChoice(IEnumerable<ProductVariantDisplay> variants, Guid productAttributeKey)
        {
            return variants.Any(pv => pv.Attributes.Any(pa => pa.Key.Equals(productAttributeKey)) && pv.Available);
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