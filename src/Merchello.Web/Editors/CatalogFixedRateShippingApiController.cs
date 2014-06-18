namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Gateways.Shipping;
    using Core.Gateways.Shipping.FixedRate;
    using Core.Services;
    using Models.ContentEditing;    
    using Umbraco.Web;
    using Umbraco.Web.Mvc;
    using WebApi;

    /// <summary>
    /// The catalog fixed rate shipping api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class CatalogFixedRateShippingApiController : MerchelloApiController
    {
        private readonly IShipCountryService _shipCountryService;
        private readonly FixedRateShippingGatewayProvider _fixedRateShippingGatewayProvider;
        private readonly IShippingContext _shippingContext;

        /// <summary>
        /// Constructor
        /// </summary>
        public CatalogFixedRateShippingApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public CatalogFixedRateShippingApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _shipCountryService = ((ServiceContext) MerchelloContext.Services).ShipCountryService;
            _fixedRateShippingGatewayProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Gateways.Shipping.GetProviderByKey(Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey);
            _shippingContext = MerchelloContext.Gateways.Shipping;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal CatalogFixedRateShippingApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _shipCountryService = ((ServiceContext)MerchelloContext.Services).ShipCountryService;
            _fixedRateShippingGatewayProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Gateways.Shipping.GetProviderByKey(Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey);
        }

        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllFixedRateGatewayResources/
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public IEnumerable<GatewayResourceDisplay> GetAllFixedRateGatewayResources()
        {
            var provider = (FixedRateShippingGatewayProvider)_shippingContext.GetProviderByKey(Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey);

            var resources = provider.ListResourcesOffered();

            return resources.Select(resource => resource.ToGatewayResourceDisplay());
        }

        /// <summary>
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipCountryFixedRateProviders/{id}
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="id">ShipCountry Key</param>
        public IEnumerable<ShippingGatewayProviderDisplay> GetAllShipCountryFixedRateProviders(Guid id)
        {
            var shipCountry = _shipCountryService.GetByKey(id);
            if (shipCountry != null)
            {
                var providers = _shippingContext.GetGatewayProvidersByShipCountry(shipCountry);

                var fixedProviders = providers.Where(x => x.Key == Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey);

                foreach (var provider in fixedProviders)
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
        /// 
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllFixedRateProviderMethods/{id}
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="id">ShipCountry Key</param>
        public IEnumerable<FixedRateShipMethodDisplay> GetAllFixedRateProviderMethods(Guid id)
        {
            var shipCountry = _shipCountryService.GetByKey(id);
            if (shipCountry != null)
            {
                var providers = _shippingContext.GetGatewayProvidersByShipCountry(shipCountry).ToArray();

                if (providers.Any())
                {
                    var fixedProvider = providers.FirstOrDefault(x => x.Key == Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey);

                    if (fixedProvider != null)
                    {
                        foreach (var method in fixedProvider.GetAllShippingGatewayMethods(shipCountry))
                        {
                            var fixedRateShippingGatewayMethod = method as IFixedRateShippingGatewayMethod;
                            yield return fixedRateShippingGatewayMethod.ToFixedRateShipMethodDisplay();
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
        }


        /// <summary>
        /// Add a Fixed Rate Table ShipMethod to the ShipCountry
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/AddFixedRateShipMethod
        /// </summary>
        /// <param name="method">POSTed FixedRateShipMethodDisplay object</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddFixedRateShipMethod(FixedRateShipMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var shipCountry = _shipCountryService.GetByKey(method.ShipMethod.ShipCountryKey);
                var provider = _fixedRateShippingGatewayProvider;

                var rateTableType = FixedRateShippingGatewayMethod.QuoteType.VaryByWeight;
                if (method.GatewayResource.ServiceCode == "VBP")
                {
                    rateTableType = FixedRateShippingGatewayMethod.QuoteType.VaryByPrice;
                }
                var merchelloGwShipMethod = (IFixedRateShippingGatewayMethod)provider.CreateShipMethod(rateTableType, shipCountry, method.ShipMethod.Name);

                merchelloGwShipMethod = method.ToFixedRateShipMethod(merchelloGwShipMethod);

                provider.SaveShippingGatewayMethod(merchelloGwShipMethod);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save a Fixed Rate Table ShipMethod to the ShipCountry
        /// 
        /// GET /umbraco/Merchello/ShippingMethodsApi/PutFixedRateShipMethod
        /// </summary>
        /// <param name="method">POSTed ShipMethodDisplay object</param>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutFixedRateShipMethod(FixedRateShipMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var shipCountry = _shipCountryService.GetByKey(method.ShipMethod.ShipCountryKey);
                var provider = _fixedRateShippingGatewayProvider;

                var merchelloMethod = (IFixedRateShippingGatewayMethod)provider.GetAllShippingGatewayMethods(shipCountry).FirstOrDefault(m => m.ShipMethod.Key == method.ShipMethod.Key);

                if (merchelloMethod != null)
                {
                    merchelloMethod = method.ToFixedRateShipMethod(merchelloMethod);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                provider.SaveShippingGatewayMethod(merchelloMethod);
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
        /// GET /umbraco/Merchello/ShippingMethodsApi/DeleteRateTableShipMethod
        /// </summary>
        /// <param name="method">POSTed ShipMethodDisplay object</param>
        [AcceptVerbs("POST", "DELETE")]
        public HttpResponseMessage DeleteRateTableShipMethod(FixedRateShipMethodDisplay method)
        {
            var shipmethodService = ((ServiceContext) MerchelloContext.Services).ShipMethodService;
            var methodToDelete = shipmethodService.GetByKey(method.ShipMethod.Key);

            if (methodToDelete == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            shipmethodService.Delete(methodToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
