namespace Merchello.Web.Editors
{
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

    /// <summary>
    /// The gateway provider API controller.
    /// </summary>
    [PluginController("Merchello")]
    public class GatewayProviderApiController : MerchelloApiController
    {
        /// <summary>
        /// The gateway provider service.
        /// </summary>
        private readonly IGatewayProviderService _gatewayProviderService;

        /// <summary>
        /// The gateway context.
        /// </summary>
        private readonly IGatewayContext _gatewayContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayProviderApiController"/> class.
        /// </summary>
        public GatewayProviderApiController()
            : this(Core.MerchelloContext.Current)
        {       
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayProviderApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public GatewayProviderApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext) 
        {
            _gatewayContext = merchelloContext.Gateways;
            _gatewayProviderService = merchelloContext.Services.GatewayProviderService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayProviderApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        internal GatewayProviderApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _gatewayContext = merchelloContext.Gateways;
            _gatewayProviderService = merchelloContext.Services.GatewayProviderService;
        }

        /// <summary>
        /// Returns an GatewayProvider by id (key)
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetGatewayProvider/{guid}
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="GatewayProviderDisplay"/>.
        /// </returns>
        public GatewayProviderDisplay GetGatewayProvider(Guid id)
        {
            var provider = _gatewayProviderService.GetGatewayProviderByKey(id) as Core.Models.GatewayProviderSettings;
            if (provider == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return provider.ToGatewayProviderDisplay();
        }

        /// <summary>
        /// Returns all resolved notification gateway providers
        /// 
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetResolvedNotificationGatewayProviders
        /// </summary>
        /// <returns>
        /// The collection of <see cref="GatewayProviderDisplay"/>.
        /// </returns>
        public IEnumerable<GatewayProviderDisplay> GetResolvedNotificationGatewayProviders()
        {
            return _gatewayContext.Notification.GetAllProviders().Select(x => x.GatewayProviderSettings.ToGatewayProviderDisplay());
        }

        /// <summary>
        /// Returns all resolved payment gateway providers
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetResolvedPaymentGatewayProviders
        /// </summary>
        /// <returns>
        /// The collection of <see cref="GatewayProviderDisplay"/>.
        /// </returns>
        public IEnumerable<GatewayProviderDisplay> GetResolvedPaymentGatewayProviders()
        {
            return _gatewayContext.Payment.GetAllProviders().Select(x => x.GatewayProviderSettings.ToGatewayProviderDisplay());
        }

        /// <summary>
        /// Returns all resolved shipping gateway providers
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetResolvedShippingGatewayProviders
        /// </summary>
        /// <returns>
        /// The collection of <see cref="GatewayProviderDisplay"/>.
        /// </returns>
        public IEnumerable<GatewayProviderDisplay> GetResolvedShippingGatewayProviders()
        {
            return _gatewayContext.Shipping.GetAllProviders().Select(x => x.GatewayProviderSettings.ToGatewayProviderDisplay());
        }


        /// <summary>
        /// Returns all resolved taxation gateway providers
        /// 
        /// GET /umbraco/Merchello/GatewayProviderApi/GetResolvedTaxationGatewayProviders
        /// </summary>
        /// <returns>
        /// The collection of <see cref="GatewayProviderDisplay"/>.
        /// </returns>
        public IEnumerable<GatewayProviderDisplay> GetResolvedTaxationGatewayProviders()
        {
            return _gatewayContext.Taxation.GetAllProviders().Select(x => x.GatewayProviderSettings.ToGatewayProviderDisplay());
        }


        /// <summary>
        /// Adds (Activates) a GatewayProvider
        /// 
        /// POST /umbraco/Merchello/GatewayProviderApi/ActivateGatewayProvider
        /// </summary>
        /// <param name="gatewayProvider">
        /// POSTed <see cref="GatewayProviderDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
                    return Request.CreateResponse(HttpStatusCode.NotFound, string.Format("{0}", "Provider could not be found or has already been activated"));
                }

                ToggleProviderActivation(provider, gatewayProviderType);
                
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Removes (Deactivates) a GatewayProvider
        /// 
        /// POST /umbraco/Merchello/GatewayProviderApi/DeactiveGatewayProvider
        /// </summary>
        /// <param name="gatewayProvider">
        /// POSTed <see cref="GatewayProviderDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
                    return Request.CreateResponse(HttpStatusCode.NotFound, string.Format("{0}", "Provider could not be found or is already not activated"));
                }

                ToggleProviderActivation(provider, gatewayProviderType);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Saves a GatewayProvider
        /// 
        /// 
        /// POST /umbraco/Merchello/GatewayProviderApi/PutGatewayProvider
        /// </summary>
        /// <param name="gatewayProviderDisplay">
        /// POSTed <see cref="GatewayProviderDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutGatewayProvider(GatewayProviderDisplay gatewayProviderDisplay)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _gatewayProviderService.GetGatewayProviderByKey(gatewayProviderDisplay.Key);
                _gatewayProviderService.Save(gatewayProviderDisplay.ToGatewayProvider(provider));

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, string.Format("{0}", ex.Message));
            }

            return response;

        }


        #region Utility methods        

        // TODO refactor this
        /// <summary>
        /// The toggle provider activation.
        /// </summary>
        /// <param name="gatewayProviderSettings">
        /// The gateway provider settings.
        /// </param>
        /// <param name="gatewayProviderType">
        /// The gateway provider type.
        /// </param>
        private void ToggleProviderActivation(IGatewayProviderSettings gatewayProviderSettings, GatewayProviderType gatewayProviderType)
        {
            switch (gatewayProviderType)
            {
               case GatewayProviderType.Notification:
                    if(gatewayProviderSettings.Activated)
                        _gatewayContext.Notification.DeactivateProvider(gatewayProviderSettings);
                    else
                        _gatewayContext.Notification.ActivateProvider(gatewayProviderSettings);
                    break;

                case GatewayProviderType.Payment:
                    if (gatewayProviderSettings.Activated)
                        _gatewayContext.Payment.DeactivateProvider(gatewayProviderSettings);
                    else 
                        _gatewayContext.Payment.ActivateProvider(gatewayProviderSettings);
                    break;

                case GatewayProviderType.Shipping:
                    if (gatewayProviderSettings.Activated)
                        _gatewayContext.Shipping.DeactivateProvider(gatewayProviderSettings);
                    else 
                        _gatewayContext.Shipping.ActivateProvider(gatewayProviderSettings);                    
                    break;

                case GatewayProviderType.Taxation:
                    if (gatewayProviderSettings.Activated)
                        _gatewayContext.Taxation.DeactivateProvider(gatewayProviderSettings);
                    else 
                        _gatewayContext.Taxation.ActivateProvider(gatewayProviderSettings);
                    break;
            }
        }

        /// <summary>
        /// Helper method to get get the <see cref="IGatewayProviderSettings"/> from the appropriate resolver
        /// </summary>
        /// <param name="gatewayProvider">The <see cref="GatewayProviderDisplay"/></param>
        /// <param name="gatewayProviderType">The gateway provider type</param>
        /// <returns>A <see cref="IGatewayProviderSettings"/> or null</returns>
        private IGatewayProviderSettings GetGatewayProviderFromResolver(GatewayProviderDisplay gatewayProvider, GatewayProviderType gatewayProviderType)
        {
            GatewayProviderBase provider = null;

            switch (gatewayProviderType)
            {
                case GatewayProviderType.Notification:
                    provider = _gatewayContext.Notification.GetProviderByKey(gatewayProvider.Key, false);
                    break;

                case GatewayProviderType.Payment:
                    provider = _gatewayContext.Payment.GetProviderByKey(gatewayProvider.Key, false);
                    break;

                case GatewayProviderType.Shipping:
                    provider = _gatewayContext.Shipping.GetProviderByKey(gatewayProvider.Key, false);
                    break;

                case GatewayProviderType.Taxation:
                    provider = _gatewayContext.Taxation.GetProviderByKey(gatewayProvider.Key, false);
                    break;
            }

            return provider != null ? provider.GatewayProviderSettings : null;
        }

        #endregion
    }
}