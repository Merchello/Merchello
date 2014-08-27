namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Services;
    using Models.ContentEditing;
    using Models.Payments;
    using WebApi;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Represents the PaymentGatewayApiController
    /// </summary>
    [PluginController("Merchello")]
    public class PaymentGatewayApiController : MerchelloApiController 
    {
        /// <summary>
        /// The payment context.
        /// </summary>
        private readonly IPaymentContext _paymentContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayApiController"/> class.
        /// </summary>
        public PaymentGatewayApiController()
            : this(Core.MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        public PaymentGatewayApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _paymentContext = MerchelloContext.Gateways.Payment;
        }

        /// <summary>
        /// 
        /// 
        /// GET /umbraco/Merchello/PaymentGatewayApi/GetGatewayResources/{id}
        /// </summary>
        /// <param name="id">
        /// The key of the PaymentGatewayProvider
        /// </param>
        /// <returns>
        /// A collection of <see cref="GatewayResourceDisplay"/>.
        /// </returns>
        public IEnumerable<GatewayResourceDisplay> GetGatewayResources(Guid id)
        {
            try
            {
                var provider = _paymentContext.GetProviderByKey(id);

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
        /// <returns>
        /// A collection of all payment <see cref="GatewayProviderDisplay"/>
        /// </returns>
        public IEnumerable<GatewayProviderDisplay> GetAllGatewayProviders()
        {
            var providers = _paymentContext.GetAllActivatedProviders();

            if (providers == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return providers.Select(provider => provider.GatewayProviderSettings.ToGatewayProviderDisplay());
        }

        /// <summary>
        /// Get all <see cref="IPaymentMethod"/> for a payment provider
        /// 
        /// GET /umbraco/Merchello/PaymentGatewayApi/GetPaymentProviderPaymentMethods/{id}
        /// </summary>
        /// <param name="id">
        /// The key of the PaymentGatewayProvider
        /// </param>
        /// <returns>
        /// A collection of <see cref="PaymentMethodDisplay"/>
        /// </returns>
        public IEnumerable<PaymentMethodDisplay> GetPaymentProviderPaymentMethods(Guid id)
        {
            var provider = _paymentContext.GetProviderByKey(id);
            if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            foreach (var method in provider.PaymentMethods)
            {
                // we need the actual PaymentGatewayProvider so we can grab the if present
                yield return provider.GetPaymentGatewayMethodByKey(method.Key).ToPaymentMethodDisplay();
            }
        }

        /// <summary>
        /// Adds a <see cref="IPaymentMethod"/>
        /// 
        /// POST /umbraco/Merchello/PaymentGatewayApi/AddPaymentMethod
        /// </summary>
        /// <param name="method">
        /// POSTed <see cref="PaymentMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddPaymentMethod(PaymentMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _paymentContext.GetProviderByKey(method.ProviderKey);

                var gatewayResource =
                    provider.ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == method.PaymentCode);

                var paymentGatewayMethod = provider.CreatePaymentMethod(gatewayResource, method.Name, method.Description);

                provider.SavePaymentMethod(paymentGatewayMethod);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save a <see cref="IPaymentMethod"/>
        /// 
        /// PUT /umbraco/Merchello/PaymentGatewayApi/PutPaymentMethod
        /// </summary>
        /// <param name="method">
        /// POSTed <see cref="PaymentMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutPaymentMethod(PaymentMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _paymentContext.GetProviderByKey(method.ProviderKey);

                var paymentMethod = provider.PaymentMethods.FirstOrDefault(x => x.Key == method.Key);

                paymentMethod = method.ToPaymentMethod(paymentMethod);

                provider.GatewayProviderService.Save(paymentMethod);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Delete a <see cref="IPaymentMethod"/>
        /// 
        /// GET /umbraco/Merchello/PaymentGatewayApi/DeletePaymentMethod
        /// </summary>
        /// <param name="id">
        /// <see cref="PaymentMethodDisplay"/> key to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("GET", "DELETE")]
        public HttpResponseMessage DeletePaymentMethod(Guid id)
        {
            var paymentMethodService = ((ServiceContext)MerchelloContext.Services).PaymentMethodService;
            var methodToDelete = paymentMethodService.GetByKey(id);

            if (methodToDelete == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            paymentMethodService.Delete(methodToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
         
    }
}