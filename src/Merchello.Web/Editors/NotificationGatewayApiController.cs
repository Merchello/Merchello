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
using Merchello.Core.Services;
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

            return providers.Select(provider => provider.GatewayProviderSettings.ToGatewayProviderDisplay());
        }


        public IEnumerable<NotificationTriggerDisplay> GetAllNotificationTriggers()
        {
            return new List<NotificationTriggerDisplay>()
            {
                new NotificationTriggerDisplay() { Key = new Guid("4B7FD17D-39C8-4D35-BE06-F3BDDE7F3EEB"), Name = "Order Confirmation", SortOrder = 0 },
                new NotificationTriggerDisplay() { Key = new Guid("C02DC640-9A6C-4BBC-AF5A-2EB355BEE41E"), Name = "Order Shipped", SortOrder = 1 },
                new NotificationTriggerDisplay() { Key = new Guid("BEEBD6BB-81DE-4799-BED3-C5DD43B295A0"), Name = "Problems with Payment Auth", SortOrder = 2 },
                new NotificationTriggerDisplay() { Key = new Guid("4DF58706-F569-4A29-ADC2-9BDA0442306A"), Name = "Payment Received", SortOrder = 3 },
                new NotificationTriggerDisplay() { Key = new Guid("15E7D98A-55B9-48D2-93C0-38857630BBCE"), Name = "Order Canceled", SortOrder = 4 }
            };
        }

        /// <summary>
        /// Get all <see cref="NotificationMethodDisplay"/> for a notification provider
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetNotificationProviderNotificationMethods/{id}
        /// </summary>
        /// <param name="id">The key (guid) of teh NotificationGatewayProvider</param>
        /// <returns></returns>
        public IEnumerable<NotificationMethodDisplay> GetNotificationProviderNotificationMethods(Guid id)
        {
            // limit only to active providers
            var provider = _notificationContext.GetProviderByKey(id);

            if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return provider.NotificationMethods.Select(x => x.ToNotificationMethodDisplay());
        }


        /// <summary>
        /// Adds a <see cref="IPaymentMethod"/>
        ///
        /// POST /umbraco/Merchello/NotificationGatewayApi/AddNotificationMethod
        /// </summary>
        /// <param name="method">POSTed <see cref="NotificationMethodDisplay"/> object</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddNotificationMethod(NotificationMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _notificationContext.GetProviderByKey(method.ProviderKey);

                var gatewayResource =
                    provider.ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == method.ServiceCode);

                var notificationGatewayMethod = provider.CreateNotificationMethod(gatewayResource, method.Name, method.Description);

                provider.SaveNotificationMethod(notificationGatewayMethod);

            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save a <see cref="IPaymentMethod"/>
        /// 
        /// PUT /umbraco/Merchello/NotificationGatewayApi/PutNotificationMethod
        /// </summary>
        /// <param name="method">POSTed <see cref="NotificationMethodDisplay"/> object</param>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutNotificationMethod(NotificationMethodDisplay method)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _notificationContext.GetProviderByKey(method.ProviderKey);

                var paymentMethod = provider.NotificationMethods.FirstOrDefault(x => x.Key == method.Key);

                paymentMethod = method.ToNotificationMethod(paymentMethod);

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
        /// GET /umbraco/Merchello/NotificationGatewayApi/DeletNotificationMethod
        /// </summary>
        /// <param name="id"><see cref="NotificationMethodDisplay"/> key to delete</param>
        [AcceptVerbs("GET", "DELETE")]
        public HttpResponseMessage DeletePaymentMethod(Guid id)
        {
            var notificationMethodService = ((ServiceContext)MerchelloContext.Services).NotificationMethodService;
            var methodToDelete = notificationMethodService.GetByKey(id);

            if (methodToDelete == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            notificationMethodService.Delete(methodToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}