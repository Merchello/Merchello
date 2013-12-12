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
using System.Net.Http;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class ShippingMethodsApiController : MerchelloApiController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ShippingMethodsApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public ShippingMethodsApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ShippingMethodsApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
        }


        /// <summary>
        /// Returns All Countries with Shipment Methods in them
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllCountries
        /// </summary>
        //public IEnumerable<CountryDisplay> GetAllCountries()
        //{
        //    var countries = _settingsService.GetAllCountries();
        //    if (countries == null)
        //    {
        //        throw new HttpResponseException(HttpStatusCode.NotFound);
        //    }

        //    foreach (ICountry country in countries)
        //    {
        //        yield return country.ToCountryDisplay();
        //    }
        //}
    }
}
