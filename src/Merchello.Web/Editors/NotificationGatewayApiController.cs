using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Models;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.WebApi;
using Umbraco.Web.Mvc;

namespace Merchello.Web.Editors
{
    /// <summary>
    /// Merchello Back Office Controller Responsible for NotificationGatewayProvider Configurations and Settings
    /// </summary>
    [PluginController("Merchello")]
    public class NotificationGatewayApiController : MerchelloApiController
    {

        private readonly INotificationContext _notificationContext;

                /// <summary>
        /// Constructor
        /// </summary>
        public NotificationGatewayApiController()
            :this(MerchelloContext.Current)
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        public NotificationGatewayApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _notificationContext = ((GatewayContext)MerchelloContext.Gateways).Notification;
        }

        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetGatewayResources/{id}
        /// </summary>
        /// <param name="id">The key of the PaymentGatewayProvider</param>
        public IEnumerable<GatewayResourceDisplay> GetGatewayResources(Guid id)
        {
            try
            {
                var provider = _notificationContext.GetProviderByKey(id);

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
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetAllGatewayProviders/
        /// </summary>        
        public IEnumerable<GatewayProviderDisplay> GetAllGatewayProviders()
        {
            var providers = _notificationContext.GetAllActivatedProviders();
            if (providers == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return providers.Select(provider => provider.GatewayProviderSetting.ToGatewayProviderDisplay());
        }



        ///// <summary>
        ///// Get all <see cref="INotificationMethod"/> for a payment provider
        /////
        ///// GET /umbraco/Merchello/NotificationGatewayApi/GetPaymentProviderPaymentMethods/{id}
        ///// </summary>
        ///// <param name="id">The key of the PaymentGatewayProvider</param>
        ///// <remarks>
        ///// 
        ///// </remarks>
        //public IEnumerable<NotiMethodDisplay> GetPaymentProviderPaymentMethods(Guid id)
        //{
        //    var provider = _paymentContext.CreateInstance(id);
        //    if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

        //    foreach (var method in provider.PaymentMethods)
        //    {
        //        // we need the actual PaymentGatewayProvider so we can grab the if present
        //        yield return provider.GetPaymentGatewayMethodByKey(method.Key).ToPaymentMethodDisplay();
        //    }
        //}
    }
}