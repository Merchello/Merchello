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
        private readonly INotificationMessageService _notificationMessageService;
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
            _notificationMessageService = ((ServiceContext)MerchelloContext.Services).NotificationMessageService;
        }

        /// <summary>
        /// 
        ///
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetGatewayResources/{id}
        /// </summary>
        /// <param name="id">The key of the PaymentGatewayProvider</param>
        [AcceptVerbs("GET")]
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

        /// <summary>
        /// Returns a list of all of Notification Triggers of NotificationTriggerDisplay
        ///
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetAllNotificationTriggers/
        /// </summary>    
        public IEnumerable<NotificationTriggerDisplay> GetAllNotificationTriggers()
        {
            return new List<NotificationTriggerDisplay>()
            {
                new NotificationTriggerDisplay() { MonitorKey = new Guid("AD7CBFE9-6E6A-4A3D-94AD-2340982B5B37"), Name = "Order Confirmation (Pattern Replace)", Alias = "orderConfirmationPatternReplace" },
                new NotificationTriggerDisplay() { MonitorKey = new Guid("E9D4699B-D78A-4ADE-8491-13C03D34D2DF"), Name = "Order Confirmation (Twitter Formatter)", Alias = "orderConfirmation TwitterFormatter" },
                new NotificationTriggerDisplay() { MonitorKey = new Guid("CBB7A5DB-0A12-4A5C-9D23-47C28C2BFE2C"), Name = "Order Confirmation (No Replacement)", Alias = "orderConfirmationNoReplacement" },
                new NotificationTriggerDisplay() { MonitorKey = new Guid("D9D5D22C-4321-4C12-8314-FBBCB480B7E5"), Name = "Order Shipped", Alias = "orderShipped" },
                new NotificationTriggerDisplay() { MonitorKey = new Guid("0C1AB386-1ACE-4E3C-AB4A-BB3C115BF69C"), Name = "Problems with Payment Auth", Alias = "problemsWithPaymentAuth" },
                new NotificationTriggerDisplay() { MonitorKey = new Guid("38A0346A-515F-4D87-A4A0-0F3CA77DF126"), Name = "Payment Received", Alias = "paymentReceived" },
                new NotificationTriggerDisplay() { MonitorKey = new Guid("06EF7EB3-8287-4698-8630-B901FA9418F5"), Name = "Order Canceled", Alias = "orderCanceled" }
            };
        }

        /// <summary>
        /// Get all <see cref="NotificationMethodDisplay"/> for a notification provider
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetNotificationProviderNotificationMethods/{id}
        /// </summary>
        /// <param name="id">The key (guid) of the NotificationGatewayProvider</param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        public IEnumerable<NotificationMethodDisplay> GetNotificationProviderNotificationMethods(Guid id)
        {
            // limit only to active providers
            var provider = _notificationContext.GetProviderByKey(id);

            if (provider == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            var methods = provider.NotificationMethods.Select(method => provider.GetNotificationGatewayMethodByKey(method.Key).ToNotificationMethodDisplay());
            return methods;

        }

        /// <summary>
        /// Adds a <see cref="INotificationMethod"/>
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

                var newMethod = true;
                foreach (var nm in provider.NotificationMethods.Where(nm => nm.ServiceCode == gatewayResource.ServiceCode))
                {
                    newMethod = false;
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Method for this resource already exists.");
                }
                if (newMethod)
                {
                    var notificationGatewayMethod = provider.CreateNotificationMethod(gatewayResource, method.Name,
                        method.ServiceCode);

                    provider.SaveNotificationMethod(notificationGatewayMethod);
                }


            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save a <see cref="INotificationMethod"/>
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

                var notificationMethod = provider.NotificationMethods.FirstOrDefault(x => x.Key == method.Key);

                notificationMethod = method.ToNotificationMethod(notificationMethod);

                provider.GatewayProviderService.Save(notificationMethod);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Delete a <see cref="INotificationMethod"/>
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/DeletNotificationMethod
        /// </summary>
        /// <param name="id"><see cref="NotificationMethodDisplay"/> key to delete</param>
        [AcceptVerbs("GET", "DELETE")]
        public HttpResponseMessage DeleteNotificationMethod(Guid id)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var provider = _notificationContext.GetProviderByMethodKey(id);
                var method = provider.GetNotificationGatewayMethodByKey(id);
                provider.DeleteNotificationMethod(method);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Returns a  <see cref="NotificationMessageDisplay"/> by method key
        /// 
        /// GET /umbraco/Merchello/NotificationsApi/GetNotificationsByMethod/{key}
        /// </summary>
        /// <param name="id">Key of the notification method</param>
        public NotificationMessageDisplay GetNotificationMessagesByKey(Guid id)
        {
            var message = _notificationMessageService.GetByKey(id);

            if (message == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return message.ToNotificationMessageDisplay();
        }

        /// <summary>
        /// Updates a <see cref="INotificationMessage"/>
        ///
        /// POST /umbraco/Merchello/NotificationGatewayApi/SaveNotificationMessage
        /// </summary>
        /// <param name="message">POSTed <see cref="NotificationMethodDisplay"/> object</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateNotificationMessage(NotificationMessageDisplay message)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                message.FromAddress = "wesley@proworks.com";

                var provider = _notificationContext.GetProviderByMethodKey(message.MethodKey);
                
                var method = provider.GetNotificationGatewayMethodByKey(message.MethodKey);

                var notificationMessage = method.NotificationMessages.FirstOrDefault(x => x.Key == message.Key);
                if (notificationMessage == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Notification Message Not Found");
                }
                method.SaveNotificationMessage(message.ToNotificationMessage(notificationMessage));
                
                return response;
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Adds a <see cref="INotificationMessage"/>
        ///
        /// POST /umbraco/Merchello/NotificationGatewayApi/SaveNotificationMessage
        /// </summary>
        /// <param name="message">POSTed <see cref="NotificationMethodDisplay"/> object</param>
        [AcceptVerbs("POST")]
        public string PutNotificationMessage(NotificationMessageDisplay message)
        {
            try
            {
                message.FromAddress = "wesley@proworks.com";
                message.TriggerKey = Guid.NewGuid();
                var provider = _notificationContext.GetProviderByMethodKey(message.MethodKey);

                var method = provider.GetNotificationGatewayMethodByKey(message.MethodKey);

                var notificationMessage = new NotificationMessage(message.MethodKey, message.Name, message.FromAddress);

                method.SaveNotificationMessage(message.ToNotificationMessage(notificationMessage));
                
                return notificationMessage.Key.ToString();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message)));
            }
            
        }
        /// <summary>
        /// Delete a <see cref="INotificationMessage"/>
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/DeleteNotificationMessage
        /// </summary>
        /// <param name="id"><see cref="NotificationMessageDisplay"/> key to delete</param>
        [AcceptVerbs("GET", "DELETE")]
        public HttpResponseMessage DeleteNotificationMessage(Guid id)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            var notificationMessageService = ((ServiceContext)MerchelloContext.Services).NotificationMessageService;
            var messageToDelete = notificationMessageService.GetByKey(id);

            if (messageToDelete == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            try
            {
                var provider = _notificationContext.GetProviderByMethodKey(messageToDelete.MethodKey);
                var method = provider.GetNotificationGatewayMethodByKey(messageToDelete.MethodKey);
                method.DeleteNotificationMessage(messageToDelete);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }           
           
            return response;
        }
    }
}