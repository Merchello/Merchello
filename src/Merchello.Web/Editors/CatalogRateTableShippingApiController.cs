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
using Merchello.Core.Gateways.Shipping.RateTable;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class CatalogRateTableShippingApiController : MerchelloApiController
    {
        private readonly IShipCountryService _shipCountryService;
        private readonly RateTableShippingGatewayProvider _rateTableShippingGatewayProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        public CatalogRateTableShippingApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public CatalogRateTableShippingApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _shipCountryService = MerchelloContext.Services.ShipCountryService;
            _rateTableShippingGatewayProvider = (RateTableShippingGatewayProvider)MerchelloContext.Gateways.Shipping.ResolveByKey(Constants.ProviderKeys.Shipping.RateTableShippingProviderKey);
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal CatalogRateTableShippingApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _shipCountryService = MerchelloContext.Services.ShipCountryService;
            _rateTableShippingGatewayProvider = (RateTableShippingGatewayProvider)MerchelloContext.Gateways.Shipping.ResolveByKey(Constants.ProviderKeys.Shipping.RateTableShippingProviderKey);
        }

        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/GetAllShipCountryRateTableProviders/{id}
        /// </summary>
        /// <param name="id">ShipCountry Key</param>
        public ShippingGatewayProviderDisplay GetAllShipCountryRateTableProviders(Guid id)
        {
            var shipCountry = _shipCountryService.GetByKey(id);
            if (shipCountry != null)
            {
                var providers = MerchelloContext.Gateways.Shipping.GetGatewayProvidersByShipCountry(shipCountry);

                var fixedProvider = providers.Where(x => x.Key == Constants.ProviderKeys.Shipping.RateTableShippingProviderKey).FirstOrDefault();

                if (fixedProvider != null)
                {
                    return fixedProvider.ToShipGatewayProviderDisplay();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Add a Fixed Rate Table ShipMethod to the ShipCountry
        ///
        /// GET /umbraco/Merchello/ShippingMethodsApi/AddRateTableShipMethod
        /// </summary>
        /// <param name="method">POSTed RateTableShipMethodDisplay object</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddRateTableShipMethod(RateTableShipMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var shipCountry = _shipCountryService.GetByKey(method.ShipMethod.ShipCountryKey);
                var provider = _rateTableShippingGatewayProvider;

                var merchelloGwShipMethod = (IRateTableShipMethod)provider.CreateShipMethod(method.RateTableType, shipCountry, method.ShipMethod.Name);

                merchelloGwShipMethod = method.ToRateTableShipMethod(merchelloGwShipMethod);

                provider.SaveShipMethod(merchelloGwShipMethod);
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
        /// GET /umbraco/Merchello/ShippingMethodsApi/PutRateTableShipMethod
        /// </summary>
        /// <param name="method">POSTed ShipMethodDisplay object</param>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutRateTableShipMethod(RateTableShipMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var shipCountry = _shipCountryService.GetByKey(method.ShipMethod.ShipCountryKey);
                var provider = _rateTableShippingGatewayProvider;

                var merchelloMethod = (IRateTableShipMethod)provider.GetActiveShipMethods(shipCountry).Where(m => m.ShipMethod.Key == method.ShipMethod.Key).FirstOrDefault();

                if (merchelloMethod != null)
                {
                    merchelloMethod = method.ToRateTableShipMethod(merchelloMethod);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                provider.SaveShipMethod(merchelloMethod);
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
        [AcceptVerbs("POST", "PUT")]
        public ShipMethodDisplay DeleteRateTableShipMethod(ShipMethodDisplay method)
        {
            return null;
        }
    }
}
