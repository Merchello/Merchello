namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using AutoMapper.Mappers;

    using Core;
    using Core.Gateways;
    using Core.Gateways.Shipping;
    using Core.Models;
    using Core.Services;
    using Models.ContentEditing;    
    using Umbraco.Web.Mvc;
    using WebApi;

    /// <summary>
    /// The shipping gateway api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class ShippingGatewayApiController : MerchelloApiController
    {
        #region Fields

        /// <summary>
        /// The shipping context.
        /// </summary>
        private readonly IShippingContext _shippingContext;

        /// <summary>
        /// The ship country service.
        /// </summary>
        private readonly IShipCountryService _shipCountryService;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingGatewayApiController"/> class.
        /// </summary>
        public ShippingGatewayApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingGatewayApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ShippingGatewayApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _shippingContext = MerchelloContext.Gateways.Shipping;

            _shipCountryService = ((ServiceContext)MerchelloContext.Services).ShipCountryService;
        }


        /// <summary>
        /// Returns ShipCountry by id (key)
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetShipCountry/{guid}
        /// </summary>
        /// <param name="id">
        /// Key of the ShipCountry to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="ShipCountryDisplay"/>.
        /// </returns>
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
        /// <param name="id">
        /// CatalogKey Guid to get countries for
        /// </param>
        /// <returns>
        /// The collection of <see cref="ShipCountryDisplay"/>.
        /// </returns>
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
        /// GET /umbraco/Merchello/ShippingMethodsApi/NewShipCountry?catalogKey={guid}&amp;countryCode={string}
        /// </summary>
        /// <param name="catalogKey">
        /// CatalogKey Guid
        /// </param>
        /// <param name="countryCode">
        /// Country code string
        /// </param>
        /// <returns>
        /// The <see cref="ShipCountryDisplay"/>.
        /// </returns>        
        [AcceptVerbs("GET", "POST")]
        public ShipCountryDisplay NewShipCountry(Guid catalogKey, string countryCode)
        {
            ShipCountry newShipCountry = null;

            try
            {
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
        /// <param name="id">
        /// ShipCountry Key
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
        /// <returns>
        /// The collection of all <see cref="GatewayProviderDisplay"/>.
        /// </returns>
        public IEnumerable<GatewayProviderDisplay> GetAllShipGatewayProviders()
        {
            var providers = MerchelloContext.Gateways.Shipping.GetAllActivatedProviders().ToArray();

            return providers.Select(provider => provider.GatewayProviderSettings.ToGatewayProviderDisplay());
        }

        /// <summary>
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipGatewayResourcesForProvider
        /// 
        /// </summary>
        /// <param name="id">
        /// GatewayProvider Key
        /// </param>
        /// <returns>
        /// The collection of <see cref="GatewayResourceDisplay"/>.
        /// </returns>
        public IEnumerable<GatewayResourceDisplay> GetAllShipGatewayResourcesForProvider(Guid id)
        {
            var provider = MerchelloContext.Gateways.Shipping.GetProviderByKey(id);

            if (provider == null) yield break;
            
            var resources = provider.ListResourcesOffered();

            foreach (IGatewayResource resource in resources)
            {
                yield return resource.ToGatewayResourceDisplay();
            }
        }

        /// <summary>
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipCountryProviders/{id}
        /// 
        /// </summary>
        /// <param name="id">
        /// ShipCountry Key
        /// </param>
        /// <returns>
        /// The collection of <see cref="ShippingGatewayProviderDisplay"/>.
        /// </returns>
        public IEnumerable<ShippingGatewayProviderDisplay> GetAllShipCountryProviders(Guid id)
        {
            var shipCountry = _shipCountryService.GetByKey(id);

            if (shipCountry != null)
            {
                var providers = MerchelloContext.Gateways.Shipping.GetGatewayProvidersByShipCountry(shipCountry);

                foreach (var provider in providers)
                {                  
                    yield return provider.ToShipGatewayProviderDisplay();                    
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
        /// <param name="id">
        /// The key of the ShippingGatewayProvider
        /// </param>
        /// <param name="shipCountryId">
        /// ShipCountry Key
        /// </param>
        /// <returns>
        /// The collection of <see cref="ShipMethodDisplay"/>.
        /// </returns>
        public IEnumerable<ShipMethodDisplay> GetShippingProviderShipMethods(Guid id, Guid shipCountryId)
        {
            var provider = _shippingContext.GetProviderByKey(id);

            if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return
                provider.ShipMethods.Select(
                    method => 
                        provider.GetShippingGatewayMethod(method.Key, method.ShipCountryKey).ToShipMethodDisplay());                  
        }

        /// <summary>
        /// Get <see cref="IShipMethod"/> for a shipping provider by country
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetShippingProviderShipMethodsByCountry/{id}
        /// </summary>
        /// <param name="id">
        /// The key of the ShippingGatewayProvider
        /// </param>
        /// <param name="shipCountryId">
        /// ShipCountry Key
        /// </param>
        /// <returns>
        /// The collection of <see cref="ShipMethodDisplay"/>.
        /// </returns>
        public IEnumerable<ShipMethodDisplay> GetShippingProviderShipMethodsByCountry(Guid id, Guid shipCountryId)
        {
            var provider = _shippingContext.GetProviderByKey(id);

            var shipCountry = _shipCountryService.GetByKey(shipCountryId);
            
            if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            
            if (shipCountry == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            var methods = provider.GetAllShippingGatewayMethodsForShipCountry(shipCountryId);

            return methods.Select(method => method.ToShipMethodDisplay());
        }

        /// <summary>
        /// Add an external ShipMethod to the ShipCountry
        /// 
        /// USPS, UPS, etc
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/AddShipMethod
        /// </summary>
        /// <param name="method">
        /// POSTed ShipMethodDisplay object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST")]
        public ShipMethodDisplay AddShipMethod(ShipMethodDisplay method)
        {
            ////var response = Request.CreateResponse(HttpStatusCode.OK);

            var provider = _shippingContext.GetProviderByKey(method.ProviderKey);

            var gatewayResource =
                provider.ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == method.ServiceCode);

            var shipCountry = _shipCountryService.GetByKey(method.ShipCountryKey);

            var shippingGatewayMethod = provider.CreateShippingGatewayMethod(
                gatewayResource,
                shipCountry,
                method.Name);

            provider.SaveShippingGatewayMethod(shippingGatewayMethod);

            return shippingGatewayMethod.ToShipMethodDisplay();
        }

        /// <summary>
        /// Save an external ShipMethod to the ShipCountry
        /// 
        /// USPS, UPS, etc
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/PutShipMethod
        /// </summary>
        /// <param name="method">
        /// POSTed ShipMethodDisplay object
        /// </param>
        /// <returns>
        /// The <see cref="ShipMethodDisplay"/>.
        /// </returns>
        [AcceptVerbs("POST", "PUT")]
        public ShipMethodDisplay PutShipMethod(ShipMethodDisplay method)
        {            
            var provider = _shippingContext.GetProviderByKey(method.ProviderKey);

            var shippingMethod = provider.ShipMethods.FirstOrDefault(x => x.Key == method.Key);

            shippingMethod = method.ToShipMethod(shippingMethod);

            provider.GatewayProviderService.Save(shippingMethod);

            return shippingMethod.ToShipMethodDisplay();
        }

        /// <summary>
        /// Save an external ShipMethod to the ShipCountry
        /// 
        /// USPS, UPS, etc
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/DeleteShipMethod
        /// </summary>
        /// <param name="method">
        /// <see cref="ShipMethodDisplay"/> key to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
