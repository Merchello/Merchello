using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Merchello.Core.Gateways;
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
    public class ShippingGatewayApiController : MerchelloApiController
    {
        private readonly IShippingContext _shippingContext;
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IStoreSettingService _storeSettingService;
        private readonly IShipCountryService _shipCountryService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ShippingGatewayApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public ShippingGatewayApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _shippingContext = MerchelloContext.Gateways.Shipping;


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

            return countries.Select(country => country.ToShipCountryDisplay());
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
            var providers = MerchelloContext.Gateways.Shipping.GetAllActivatedProviders().ToArray();
            if( providers.Any() )
            {
                var rateTableProvider = MerchelloContext.Gateways.Shipping.CreateInstance(providers.First().Key);
                if (rateTableProvider == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }
            }

            return providers.Select(provider => provider.GatewayProviderSettings.ToGatewayProviderDisplay());
        }

        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipGatewayResourcesForProvider
        /// </summary>
        /// <param name="id">GatewayProvider Key</param>
        public IEnumerable<GatewayResourceDisplay> GetAllShipGatewayResourcesForProvider(Guid id)
        {
            var provider = MerchelloContext.Gateways.Shipping.CreateInstance(id);
            if (provider != null)
            {
                var resources = provider.ListResourcesOffered();
                foreach (IGatewayResource resource in resources)
                {
                    yield return resource.ToGatewayResourceDisplay();
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

                foreach (var provider in providers)
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
        /// Get all <see cref="IShipMethod"/> for a shipping provider
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetShippingProviderShipMethods/{id}
        /// </summary>
        /// <param name="id">The key of the ShippingGatewayProvider</param>
        /// <param name="shipCountryId">ShipCountry Key</param>
        /// <remarks>
        /// 
        /// </remarks>
        public IEnumerable<ShipMethodDisplay> GetShippingProviderShipMethods(Guid id, Guid shipCountryId)
        {
            var provider = _shippingContext.CreateInstance(id);
            if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return
                provider.ShipMethods.Select(
                    method => 
                        provider.GetShippingGatewayMethod(method.Key, method.ShipCountryKey).ToShipMethodDisplay()
                    );
                  
        }

        /// <summary>
        /// Get <see cref="IShipMethod"/> for a shipping provider by country
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetShippingProviderShipMethodsByCountry/{id}
        /// </summary>
        /// <param name="id">The key of the ShippingGatewayProvider</param>
        /// <param name="shipCountryId">ShipCountry Key</param>
        /// <remarks>
        /// 
        /// </remarks>
        public IEnumerable<ShipMethodDisplay> GetShippingProviderShipMethodsByCountry(Guid id, Guid shipCountryId)
        {
            var provider = _shippingContext.CreateInstance(id);
            var shipCountry = _shipCountryService.GetByKey(shipCountryId);
            if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            if (shipCountry == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            if (!Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey.Equals(provider.Key))
            {
                var methods = provider.GetAllShippingGatewayMethodsForShipCountry(shipCountryId);
                return methods.Select(method => method.ToShipMethodDisplay());
            }

            return new List<ShipMethodDisplay>();
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
        public HttpResponseMessage AddShipMethod(ShipMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _shippingContext.CreateInstance(method.ProviderKey);

                var gatewayResource =
                    provider.ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == method.ServiceCode);

                var shipCountry = _shipCountryService.GetByKey(method.ShipCountryKey);

                var shippingGatewayMethod = provider.CreateShippingGatewayMethod(gatewayResource, shipCountry, method.Name);

                provider.SaveShippingGatewayMethod(shippingGatewayMethod);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
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
        public HttpResponseMessage PutShipMethod(ShipMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _shippingContext.CreateInstance(method.ProviderKey);

                var shippingMethod = provider.ShipMethods.FirstOrDefault(x => x.Key == method.Key);

                shippingMethod = method.ToShipMethod(shippingMethod);

                provider.GatewayProviderService.Save(shippingMethod);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save an external ShipMethod to the ShipCountry
        /// 
        /// USPS, UPS, etc
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/DeleteShipMethod
        /// </summary>
        /// <param name="method"><see cref="ShipMethodDisplay"/> key to delete</param>
        [AcceptVerbs("POST", "DELETE")]
        public HttpResponseMessage DeleteShipMethod(ShipMethodDisplay method)
        {
            var shippingMethodService = ((ServiceContext)MerchelloContext.Services).ShipMethodService;
            var methodToDelete = shippingMethodService.GetByKey(method.Key);

            if (methodToDelete == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            shippingMethodService.Delete(methodToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
