using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Web;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.WebApi;
using Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Controllers
{
    /// <summary>
    /// Utility controller - assists with little lookups
    /// </summary>
    [PluginController("RosettaStone")]
    [JsonCamelCaseFormatter]
    public class SiteApiController : UmbracoApiController
    {
        private readonly IMerchelloContext _merchelloContext;
        
        public SiteApiController()
            : this(MerchelloContext.Current)
        { }

        public SiteApiController(IMerchelloContext merchelloContext)
        {
            _merchelloContext = merchelloContext;
        }

        /// <summary>
        /// Utility method to change pricing on a Product template when a customer changes an option in the drop downs
        /// </summary>
        /// <param name="productKey">The <see cref="ProductDisplay"/> key</param>
        /// <param name="optionChoiceKeys">A collection of option choice keys from the drop down(s)</param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        public string GetProductVariantPrice(Guid productKey, string optionChoiceKeys)
        {
            var optionsArray = optionChoiceKeys.Split(',');

            var guidOptionChoiceKeys = new List<Guid>();
            foreach (var option in optionsArray)
            {
                if (!String.IsNullOrEmpty(option))
                {
                    guidOptionChoiceKeys.Add(new Guid(option));
                }
            }

            var product = _merchelloContext.Services.ProductService.GetByKey(productKey); 
            var variant = _merchelloContext.Services.ProductVariantService.GetProductVariantWithAttributes(product, guidOptionChoiceKeys.ToArray());

            return variant.Price.ToString("C");
        }

        /// <summary>
        /// Returns a collection of valid (remaining) product variant possiblities based on the currently selected choices
        /// </summary>
        /// <param name="productKey">The unique 'key' (Guid) for the <see cref="ProductDisplay"/></param>
        /// <param name="optionChoices">An array of value choices</param>
        [AcceptVerbs("GET")]
        public IEnumerable<ProductVariantDisplay> FilterOptionsBySelectedChoices(Guid productKey, string optionChoices)
        {
            var merchello = new MerchelloHelper();

            var optionsArray = string.IsNullOrEmpty(optionChoices) ? new string[] { } : optionChoices.Split(',');
            var guidOptionChoices = new List<Guid>();
            foreach (var option in optionsArray)
            {
                if (!String.IsNullOrEmpty(option))
                {
                    guidOptionChoices.Add(new Guid(option));
                }
            }

            var variants = merchello.GetValidProductVariants(productKey, guidOptionChoices.ToArray());
            return variants;
        }

        /// <summary>
        /// Returns a list of all countries Merchello can ship to - for the drop down in the merchCheckoutPage view
        /// </summary>
        [AcceptVerbs("GET")]
        public IEnumerable<CountryModel> GetCountries()
        {
            return AllowableShipCounties
                .OrderBy(x => x.Name).Select(x => new CountryModel() { CountryCode = x.CountryCode, Name = x.Name});
        }

        /// <summary>
        /// Returns a list of all countries
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        public IEnumerable<CountryModel> GetAllCountries()
        {
            return
                AllCountries.Select(x => new CountryModel() {CountryCode = x.CountryCode, Name = x.Name});
        }

        /// <summary>
        /// Gets a collection of all provinces for the shipping drop down in the merchCheckoutPage view
        /// </summary>
        [AcceptVerbs("GET")]
        public IEnumerable<ProvinceModel> GetProvinces()
        {
            return BuildProvinceCollection(AllowableShipCounties.Where(x => x.Provinces.Any()));      
        }

        /// <summary>
        /// Gets a collection of all provinces for the payment drop down
        /// </summary>
        [AcceptVerbs("GET")]
        public IEnumerable<ProvinceModel> GetAllProvinces()
        {
            return BuildProvinceCollection(AllCountries.Where(x => x.Provinces.Any()));
        }


       
        /// <summary>
        /// Gets the payment methods available
        /// </summary>
        [AcceptVerbs("GET")]
        public IEnumerable<object> GetPaymentMethods()
        {
            return _merchelloContext.Gateways.Payment.GetPaymentGatewayMethods().Select(x => new
            {
                x.PaymentMethod.Key, x.PaymentMethod.Name
            });
        }

        

        #region Lookups


        private IEnumerable<ICountry> _allowableCountries; 
        private IEnumerable<ICountry> AllowableShipCounties
        {
            get {
                return _allowableCountries ??
                       (_allowableCountries = _merchelloContext.Gateways.Shipping.GetAllowedShipmentDestinationCountries().OrderBy(x => x.Name));
            }
        }


        private IEnumerable<ICountry> _allCountries;
        private IEnumerable<ICountry> AllCountries
        {
            get
            {
                return _allCountries ??
                       (_allCountries = _merchelloContext.Services.StoreSettingService.GetAllCountries().OrderBy(x => x.Name));
            }
        }



        private static IEnumerable<ProvinceModel> BuildProvinceCollection(IEnumerable<ICountry> countries)
        {
            var models = new List<ProvinceModel>();
            foreach (var country in countries)
            {
                models.AddRange(country.Provinces.Select(p => new ProvinceModel() { ProvinceCode = p.Code, Name = p.Name }));
            }
            return models;
        }

        #endregion
    }

    



}
