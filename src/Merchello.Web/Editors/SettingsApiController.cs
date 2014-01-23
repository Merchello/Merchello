using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using Merchello.Web.Models.ContentEditing;
using System.Net;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class SettingsApiController : MerchelloApiController
    {
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public SettingsApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _storeSettingService = MerchelloContext.Services.StoreSettingService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal SettingsApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _storeSettingService = MerchelloContext.Services.StoreSettingService;
        }

        /// <summary>
        /// Returns Country for the countryCode passed in
        /// 
        /// GET /umbraco/Merchello/SettingsApi/GetCountry/{countryCode}
        /// </summary>
        /// <param name="id">Country code to get</param>
        public CountryDisplay GetCountry(string id)
        {
            ICountry country = _storeSettingService.GetCountryByCode(id);
            if (country == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return country.ToCountryDisplay();
        }

        /// <summary>
        /// Returns All Countries
        /// 
        /// GET /umbraco/Merchello/SettingsApi/GetAllCountries
        /// </summary>
        public IEnumerable<CountryDisplay> GetAllCountries()
        {
            var countries = _storeSettingService.GetAllCountries();
            if (countries == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            foreach (ICountry country in countries)
            {
                yield return country.ToCountryDisplay();
            }
        }

        /// <summary>
        /// Returns All Countries with a list of country codes to exclude
        /// 
        /// GET /umbraco/Merchello/SettingsApi/GetAllCountriesExcludeCodes?codes={string}&codes={string}
        /// </summary>
        /// <param name="codes">Country codes to exclude</param>
        public IEnumerable<CountryDisplay> GetAllCountriesExcludeCodes([FromUri]string[] codes)
        {
            var countries = _storeSettingService.GetAllCountries(codes);
            if (countries == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            foreach (ICountry country in countries)
            {
                yield return country.ToCountryDisplay();
            }
        }

        /// <summary>
        /// Returns All Tax Provinces
        /// 
        /// GET /umbraco/Merchello/SettingsApi/GetAllTaxProvinces
        /// </summary>
        public IEnumerable<TaxProvinceDisplay> GetAllTaxProvinces()
        {
            // TODO: replace with call to service
            var taxProvinces = new List<TaxProvince>();

            var oregon = new TaxProvince("OR", "Oregon");
            oregon.PercentAdjustment = 0.01M;
            taxProvinces.Add(oregon);

            var washington = new TaxProvince("WA", "Washington");
            washington.PercentAdjustment = 0.09M;
            taxProvinces.Add(washington);

            // END TEST DATA

            foreach (TaxProvince taxProvince in taxProvinces)
            {
                yield return taxProvince.ToTaxProvinceDisplay();
            }
        }
    }
}
