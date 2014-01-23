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
        private readonly IShippingService _shippingService;
        private readonly IStoreSettingService _storeSettingService;

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
            _shippingService = MerchelloContext.Services.ShippingService;
            _storeSettingService = MerchelloContext.Services.StoreSettingService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ShippingMethodsApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _shippingService = MerchelloContext.Services.ShippingService;
            _storeSettingService = MerchelloContext.Services.StoreSettingService;
        }

        /// <summary>
        /// Returns ShipCountry by id (key)
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetShipCountry/{guid}
        /// </summary>
        /// <param name="id">Key of the ShipCountry to retrieve</param>
        public ShipCountryDisplay GetShipCountry(Guid id)
        {

            var shipCountry = _shippingService.GetShipCountryByKey(id);
            if (shipCountry == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return shipCountry.ToShipCountryDisplay();

        }

        /// <summary>
        /// Returns ShipCountry by id (key)
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetShipCountryByCode/?catalogKey={guid}&countryCode={string}
        /// </summary>
        /// <param name="catalogKey">CatalogKey Guid to get countries for</param>
        /// <param name="countryCode">Country code to retrieve</param>
        public ShipCountryDisplay GetShipCountryByCode(Guid catalogKey, string countryCode)
        {

            var shipCountry = _shippingService.GetShipCountryByCountryCode(catalogKey, countryCode);
            if (shipCountry == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return shipCountry.ToShipCountryDisplay();

        }

        /// <summary>
        /// Returns All ShipCountries with ShipMethods in them
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipCountries/{guid}
        /// </summary>
        /// <param name="id">CatalogKey Guid to get countries for</param>
        public IEnumerable<ShipCountryDisplay> GetAllShipCountries(Guid id)
        {
            var countries = _shippingService.GetShipCountriesByCatalogKey(id);
            if (countries == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            foreach (IShipCountry country in countries)
            {
                yield return country.ToShipCountryDisplay();
            }
        }

        /// <summary>
        /// Creates a ship country
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/NewShipCountry?catalogKey={guid}&countryCode={string}
        /// </summary>
        /// <param name="catalogKey">CatalogKey Guid</param>
        /// <param name="countryCode">Country code string</param>
        [AcceptVerbs("GET", "POST")]
        public ShipCountryDisplay NewShipCountry(Guid catalogKey, string countryCode)
        {
            ShipCountry newShipCountry = null;

            try
            {
                ICountry country = _storeSettingService.GetCountryByCode(countryCode);
                newShipCountry = new ShipCountry(catalogKey, country);
                _shippingService.Save(newShipCountry);
                newShipCountry = _shippingService.GetShipCountryByCountryCode(catalogKey, countryCode) as ShipCountry;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return newShipCountry.ToShipCountryDisplay();
        }

        /// <summary>
        /// Deletes an existing ship country
        ///
        /// DELETE /umbraco/Merchello/ShippingMethodsApi/{guid}
        /// </summary>
        /// <param name="id"></param>
        public HttpResponseMessage Delete(Guid id)
        {
            var shipCountryToDelete = _shippingService.GetShipCountryByKey(id);
            if (shipCountryToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _shippingService.Delete(shipCountryToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes an existing ship country by catalog and country code
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/DeleteByCountryCode?catalogKey={guid}&countryCode={string}
        /// </summary>
        /// <param name="catalogKey">CatalogKey Guid</param>
        /// <param name="countryCode">Country code string</param>
        public HttpResponseMessage DeleteByCountryCode(Guid catalogKey, string countryCode)
        {
            var shipCountryToDelete = _shippingService.GetShipCountryByCountryCode(catalogKey, countryCode);
            if (shipCountryToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _shippingService.Delete(shipCountryToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
