using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Services;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.WebApi;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class GatewayProviderApiController : MerchelloApiController
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IGatewayContext _gatewayContext;
        
        public GatewayProviderApiController()
            : this(MerchelloContext.Current)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public GatewayProviderApiController(IMerchelloContext merchelloContext)
            : base((MerchelloContext) merchelloContext)
        {
            _gatewayContext = merchelloContext.Gateways;
            _gatewayProviderService = merchelloContext.Services.GatewayProviderService;
        }


        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal GatewayProviderApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base((MerchelloContext) merchelloContext, umbracoContext)
        {
            _gatewayContext = merchelloContext.Gateways;
            _gatewayProviderService = merchelloContext.Services.GatewayProviderService;
        }

        /// <summary>
        /// Returns an GatewayProvider by id (key)
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetGatewayProvider/{guid}
        /// </summary>
        /// <param name="id"></param>
        public GatewayProviderDisplay GetGatewayProvider(Guid id)
        {
            var provider = _gatewayProviderService.GetGatewayProviderByKey(id) as Core.Models.GatewayProvider;
            if (provider == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return provider.ToGatewayProviderDisplay();
        }

        /// <summary>
        /// Returns all resolved payment gateway providers
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetResolvedPaymentGatewayProviders
        /// </summary>
        public IEnumerable<GatewayProviderDisplay> GetResolvedPaymentGatewayProviders()
        {
            return _gatewayContext.Payment.GetAllProviders().Select(x => x.ToGatewayProviderDisplay());
        }

        /// <summary>
        /// Returns all resolved shipping gateway providers
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetResolvedShippingGatewayProviders
        /// </summary>
        public IEnumerable<GatewayProviderDisplay> GetResolvedShippingGatewayProviders()
        {
            return _gatewayContext.Shipping.GetAllProviders().Select(x => x.ToGatewayProviderDisplay());
        }


        /// <summary>
        /// Returns all resolved taxation gateway providers
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetResolvedTaxationGatewayProviders
        /// </summary>
        public IEnumerable<GatewayProviderDisplay> GetResolvedTaxationGatewayProviders()
        {
            return _gatewayContext.Taxation.GetAllProviders().Select(x => x.ToGatewayProviderDisplay());
        }


        /// <summary>
        /// Adds (Activates) a GatewayProvider
        ///
        /// POST /umbraco/Merchello/GatewayProviderApi/ActivateGatewayProvider
        /// </summary>
        /// <param name="gatewayProvider">POSTed <see cref="GatewayProviderDisplay"/> object</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage ActivateGatewayProvider(GatewayProviderDisplay gatewayProvider)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var gatewayProviderType = EnumTypeFieldConverter.GatewayProvider.GetTypeField(gatewayProvider.ProviderTfKey);
   
                var provider = GetGatewayProviderFromResolver(gatewayProvider, gatewayProviderType);
             
                if (provider == null || provider.Activated)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", "Provider could not be found or has already been activated"));
                }

                ToggleProviderActivation(provider, gatewayProviderType);
                
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Removes (Deactivates) a GatewayProvider
        ///
        /// POST /umbraco/Merchello/GatewayProviderApi/DeactiveGatewayProvider
        /// </summary>
        /// <param name="gatewayProvider">POSTed <see cref="GatewayProviderDisplay"/> object</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage DeactivateGatewayProvider(GatewayProviderDisplay gatewayProvider)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var gatewayProviderType = EnumTypeFieldConverter.GatewayProvider.GetTypeField(gatewayProvider.ProviderTfKey);

                var provider = GetGatewayProviderFromResolver(gatewayProvider, gatewayProviderType);

                if (provider == null || !provider.Activated)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", "Provider could not be found or is already not activated"));
                }

                ToggleProviderActivation(provider, gatewayProviderType);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }


        #region Utility methods        

        // TODO refactor this
        private void ToggleProviderActivation(IGatewayProvider gatewayProvider, GatewayProviderType gatewayProviderType)
        {
            switch (gatewayProviderType)
            {
                case GatewayProviderType.Payment:
                    if (gatewayProvider.Activated)
                        _gatewayContext.Payment.DeactivateProvider(gatewayProvider);
                    else 
                        _gatewayContext.Payment.ActivateProvider(gatewayProvider);
                    break;

                case GatewayProviderType.Shipping:
                    if (gatewayProvider.Activated)
                        _gatewayContext.Shipping.DeactivateProvider(gatewayProvider);
                    else 
                        _gatewayContext.Shipping.ActivateProvider(gatewayProvider);                    
                    break;

                case GatewayProviderType.Taxation:
                    if (gatewayProvider.Activated)
                        _gatewayContext.Taxation.DeactivateProvider(gatewayProvider);
                    else 
                        _gatewayContext.Taxation.ActivateProvider(gatewayProvider);
                    break;
            }
        }

        /// <summary>
        /// Helper method to get get the <see cref="IGatewayProvider"/> from the appropriate resolver
        /// </summary>
        /// <param name="gatewayProvider">The <see cref="GatewayProviderDisplay"/></param>
        /// <param name="gatewayProviderType"></param>
        /// <returns>A <see cref="IGatewayProvider"/> or null</returns>
        // TODO refactor this
        private IGatewayProvider GetGatewayProviderFromResolver(GatewayProviderDisplay gatewayProvider, GatewayProviderType gatewayProviderType)
        {
            // get the type of the provider
            

            IGatewayProvider provider = null;

            switch (gatewayProviderType)
            {
                case GatewayProviderType.Payment:
                    provider = _gatewayContext.Payment.GetAllProviders().FirstOrDefault(x => x.Key == gatewayProvider.Key);
                    break;

                case GatewayProviderType.Shipping:
                    provider = _gatewayContext.Shipping.GetAllProviders().FirstOrDefault(x => x.Key == gatewayProvider.Key);
                    break;

                case GatewayProviderType.Taxation:
                    provider = _gatewayContext.Taxation.GetAllProviders().FirstOrDefault(x => x.Key == gatewayProvider.Key);
                    break;
            }

            return provider;
        }

        #endregion
    }
}