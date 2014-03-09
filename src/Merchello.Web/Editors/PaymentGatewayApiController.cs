using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.WebApi;
using Umbraco.Web.Mvc;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class PaymentGatewayApiController : MerchelloApiController 
    {
        private readonly IPaymentContext _paymentContext;

        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentGatewayApiController()
            :this(MerchelloContext.Current)
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        public PaymentGatewayApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _paymentContext = MerchelloContext.Gateways.Payment;
        }

        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/PaymentGatewayApi/GetGatewayResources/{id}
        /// </summary>
        /// <param name="id">The key of the PaymentGatewayProvider</param>
        public IEnumerable<GatewayResourceDisplay> GetGatewayResources(Guid id)
        {
            try
            {
                var provider = _paymentContext.ResolveByKey(id);

                var resources = provider.ListResourcesOffered();

                return resources.Select(resource => resource.ToGatewayResourceDisplay());
            }
            catch (Exception)
            {

                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            
        }

        /// <summary>
        /// Returns a list of all of GatewayProviders of GatewayProviderType Payment
        ///
        /// GET /umbraco/Merchello/PaymentGatewayApi/GetAllGatewayProviders/
        /// </summary>        
        public IEnumerable<GatewayProviderDisplay> GetAllGatewayProviders()
        {
            var providers = _paymentContext.GetAllGatewayProviders();
            if (providers != null)
            {
                foreach (var provider in providers)
                {
                    yield return provider.ToGatewayProviderDisplay();
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
        /// GET /umbraco/Merchello/PaymentGatewayApi/GetPaymentProviderPaymentMethods/{id}
        /// </summary>
        /// <param name="id">The key of the PaymentGatewayProvider</param>
        /// <remarks>
        /// 
        /// </remarks>
        public IEnumerable<PaymentMethodDisplay> GetPaymentProviderPaymentMethods(Guid id)
        {
            var provider = _paymentContext.ResolveByKey(id);
            if (provider != null)
            {
                foreach (var method in provider.PaymentMethods)
                {
                    yield return method.ToPaymentMethodDisplay();
                }
            }

            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Adds a PaymentMethod
        ///
        /// POST /umbraco/Merchello/PaymentGatewayApi/AddPaymentMethod
        /// </summary>
        /// <param name="method">POSTed <see cref="PaymentMethodDisplay"/> object</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddPaymentMethod(PaymentMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _paymentContext.ResolveByKey(method.ProviderKey);

                var paymentGatewayMethod = provider.CreatePaymentMethod(method.Name, method.Description);

               provider.SavePaymentMethod(paymentGatewayMethod);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save a TaxMethod
        /// 
        /// PUT /umbraco/Merchello/PaymentGatewayApi/PutPaymentMethod
        /// </summary>
        /// <param name="method">POSTed <see cref="PaymentMethodDisplay"/> object</param>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutTaxMethod(PaymentMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _paymentContext.ResolveByKey(method.ProviderKey);

                var paymentMethod = provider.PaymentMethods.FirstOrDefault(x => x.Key == method.Key);

                paymentMethod = method.ToPaymentMethod(paymentMethod);

                provider.GatewayProviderService.Save(paymentMethod);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Delete a <see cref="IPaymentMethod"/>
        /// 
        /// GET /umbraco/Merchello/PaymentGatewayApi/DeletePaymentMethod
        /// </summary>
        /// <param name="method">POSTed PaymentMethodDisplay object</param>
        [AcceptVerbs("GET", "DELETE")]
        public HttpResponseMessage DeletePaymentMethod(PaymentMethodDisplay method)
        {
            var paymentMethodService = ((ServiceContext)MerchelloContext.Services).PaymentMethodService;
            var methodToDelete = paymentMethodService.GetByKey(method.Key);

            if (methodToDelete == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            paymentMethodService.Delete(methodToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
         
    }
}