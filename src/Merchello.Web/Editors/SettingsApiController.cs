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
        private readonly ISettingsService _settingsService;

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
            _settingsService = MerchelloContext.Services.SettingsService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal SettingsApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _settingsService = MerchelloContext.Services.SettingsService;
        }

        /// <summary>
        /// Returns Country for the countryCode passed in
        /// 
        /// GET /umbraco/Merchello/SettingsApi/GetCountry/{countryCode}
        /// </summary>
        /// <param name="id">Country code to get</param>
        public CountryDisplay GetCountry(string id)
        {
            ICountry country = _settingsService.GetCountryByCode(id);
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
            var countries = _settingsService.GetAllCountries();
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
            var countries = _settingsService.GetAllCountries(codes);
            if (countries == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            foreach (ICountry country in countries)
            {
                yield return country.ToCountryDisplay();
            }
        }
    }
}
