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
using Merchello.Core.Gateways.Shipping;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class CatalogShippingApiController : MerchelloApiController
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IStoreSettingService _storeSettingService;
        private readonly IShipCountryService _shipCountryService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CatalogShippingApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public CatalogShippingApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _gatewayProviderService = MerchelloContext.Services.GatewayProviderService;
            _storeSettingService = MerchelloContext.Services.StoreSettingService;
            _shipCountryService = ((ServiceContext)MerchelloContext.Services).ShipCountryService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal CatalogShippingApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _gatewayProviderService = MerchelloContext.Services.GatewayProviderService;
            _storeSettingService = MerchelloContext.Services.StoreSettingService;
            _shipCountryService = ((ServiceContext)MerchelloContext.Services).ShipCountryService;
        }


        /// <summary>
        /// Returns ShipCountry by id (key)
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetShipCountry/{guid}
        /// </summary>
        /// <param name="id">Key of the ShipCountry to retrieve</param>
        public ShipCountryDisplay GetShipCountry(Guid id)
        {

            var shipCountry = _shipCountryService.GetByKey(id);
            if (shipCountry == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
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
            var countries = _shipCountryService.GetShipCountriesByCatalogKey(id);
            if (countries == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
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
                //ICountry country = _storeSettingService.GetCountryByCode(countryCode);
                //newShipCountry = new ShipCountry(catalogKey, country);
                //_shipCountryService.Save(newShipCountry);
                //newShipCountry = _shipCountryService.GetShipCountryByCountryCode(catalogKey, countryCode) as ShipCountry;
                var attempt = ((ShipCountryService) _shipCountryService).CreateShipCountryWithKey(catalogKey, countryCode);
                if (attempt.Success)
                {
                    newShipCountry = attempt.Result as ShipCountry;
                }
                else
                {
                    throw attempt.Exception;
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }

            return newShipCountry.ToShipCountryDisplay();
        }

        /// <summary>
        /// Deletes an existing ship country
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/DeleteShipCountry/{guid}
        /// </summary>
        /// <param name="id">ShipCountry Key</param>
        [AcceptVerbs("GET")]
        public HttpResponseMessage DeleteShipCountry(Guid id)
        {
            var shipCountryToDelete = _shipCountryService.GetByKey(id);
            if (shipCountryToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _shipCountryService.Delete(shipCountryToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }


        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipGatewayProviders
        /// </summary>
        public IEnumerable<GatewayProviderDisplay> GetAllShipGatewayProviders()
        {
            var providers = MerchelloContext.Gateways.Shipping.GetAllGatewayProviders();
            if( providers.Count() > 0 )
            {
                var rateTableProvider = MerchelloContext.Gateways.Shipping.ResolveByKey(providers.First().Key);
                if (rateTableProvider == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }
            }

            foreach (IGatewayProvider provider in providers)
            {
                yield return provider.ToGatewayProviderDisplay();
            }
        }

        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipGatewayResourcesForProvider
        /// </summary>
        /// <param name="id">GatewayProvider Key</param>
        public IEnumerable<GatewayResourceDisplay> GetAllShipGatewayResourcesForProvider(Guid id)
        {
            var provider = MerchelloContext.Gateways.Shipping.ResolveByKey(id);
            if (provider != null)
            {
                var resources = provider.ListResourcesOffered();
                foreach (IGatewayResource resource in resources)
                {
                    yield return resource.ToGatewayProviderDisplay();
                } 
            }


        }

        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipCountryProviders/{id}
        /// </summary>
        /// <param name="id">ShipCountry Key</param>
        public IEnumerable<ShippingGatewayProviderDisplay> GetAllShipCountryProviders(Guid id)
        {
            var shipCountry = _shipCountryService.GetByKey(id);
            if (shipCountry != null)
            {
                var providers = MerchelloContext.Gateways.Shipping.GetGatewayProvidersByShipCountry(shipCountry);

                foreach (IShippingGatewayProvider provider in providers)
                {
                    if (!Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey.Equals(provider.Key))
                    {
                        yield return provider.ToShipGatewayProviderDisplay();
                    }
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
        }

        /// <summary>
        /// Add an external ShipMethod to the ShipCountry
        /// 
        /// USPS, UPS, etc
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/AddShipMethod
        /// </summary>
        /// <param name="method">POSTed ShipMethodDisplay object</param>
        [AcceptVerbs("POST")]
        public ShipMethodDisplay AddShipMethod(ShipMethodDisplay method)
        {
            return null;
        }

        /// <summary>
        /// Save an external ShipMethod to the ShipCountry
        /// 
        /// USPS, UPS, etc
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/PutShipMethod
        /// </summary>
        /// <param name="method">POSTed ShipMethodDisplay object</param>
        [AcceptVerbs("POST", "PUT")]
        public ShipMethodDisplay PutShipMethod(ShipMethodDisplay method)
        {
            return null;
        }

        /// <summary>
        /// Save an external ShipMethod to the ShipCountry
        /// 
        /// USPS, UPS, etc
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/DeleteShipMethod
        /// </summary>
        /// <param name="method">POSTed ShipMethodDisplay object</param>
        [AcceptVerbs("POST", "PUT")]
        public ShipMethodDisplay DeleteShipMethod(ShipMethodDisplay method)
        {
            return null;
        }

    }
}
