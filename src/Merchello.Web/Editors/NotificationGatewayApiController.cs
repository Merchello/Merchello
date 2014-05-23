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
                new NotificationTriggerDisplay() { TriggerKey = new Guid("ADCFB6DA-2A59-4E02-8473-EE063E8D2F3F"), MonitorKey = new Guid("AD7CBFE9-6E6A-4A3D-94AD-2340982B5B37"), Name = "Order Confirmation (Pattern Replace)", Alias = "orderConfirmationPatternReplace" },
                new NotificationTriggerDisplay() { TriggerKey = new Guid("ADCFB6DA-2A59-4E02-8473-EE063E8D2F3F"), MonitorKey = new Guid("E9D4699B-D78A-4ADE-8491-13C03D34D2DF"), Name = "Order Confirmation (Twitter Formatter)", Alias = "orderConfirmation TwitterFormatter" },
                new NotificationTriggerDisplay() { TriggerKey = new Guid("ADCFB6DA-2A59-4E02-8473-EE063E8D2F3F"), MonitorKey = new Guid("CBB7A5DB-0A12-4A5C-9D23-47C28C2BFE2C"), Name = "Order Confirmation (No Replacement)", Alias = "orderConfirmationNoReplacement" },
                new NotificationTriggerDisplay() { TriggerKey = new Guid("C02DC640-9A6C-4BBC-AF5A-2EB355BEE41E"), MonitorKey = new Guid("D9D5D22C-4321-4C12-8314-FBBCB480B7E5"), Name = "Order Shipped", Alias = "orderShipped" },
                new NotificationTriggerDisplay() { TriggerKey = new Guid("BEEBD6BB-81DE-4799-BED3-C5DD43B295A0"), MonitorKey = new Guid("0C1AB386-1ACE-4E3C-AB4A-BB3C115BF69C"), Name = "Problems with Payment Auth", Alias = "problemsWithPaymentAuth" },
                new NotificationTriggerDisplay() { TriggerKey = new Guid("4DF58706-F569-4A29-ADC2-9BDA0442306A"), MonitorKey = new Guid("38A0346A-515F-4D87-A4A0-0F3CA77DF126"), Name = "Payment Received", Alias = "paymentReceived" },
                new NotificationTriggerDisplay() { TriggerKey = new Guid("15E7D98A-55B9-48D2-93C0-38857630BBCE"), MonitorKey = new Guid("06EF7EB3-8287-4698-8630-B901FA9418F5"), Name = "Order Canceled", Alias = "orderCanceled" }
            };
        }

        /// <summary>
        /// Get all <see cref="NotificationMethodDisplay"/> for a notification provider
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetNotificationProviderNotificationMethods/{id}
        /// </summary>
        /// <param name="id">The key (guid) of teh NotificationGatewayProvider</param>
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
        /// Returns NotificationMessages by key
        /// 
        /// GET /umbraco/Merchello/NotificationsApi/GetNotification/{key}
        /// </summary>
        /// <param name="id">Key of the NotificationMessages</param>
        public NotificationMessageDisplay GetNotification([FromUri]Guid id)
        {
            var notifications = _notificationMessageService.GetByKey(id);
            if (notifications == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return notifications.ToNotificationMessageDisplay();
        }

        /// <summary>
        /// Returns a collection of NotificationMessages by unique keys (Guid)
        /// 
        /// GET /umbraco/Merchello/NotificationsApi/GetNotifications?ids={guid}&ids={guid}
        /// </summary>
        /// <param name="id">Keys of the NotificationMessages</param>
        [AcceptVerbs("Get")]
        public IEnumerable<NotificationMessageDisplay> GetNotificationMessages(Guid methodKey)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            var messages = new List<NotificationMessageDisplay>();
            try
            {
                var provider = _notificationContext.GetProviderByMethodKey(methodKey);

                var method = provider.GetNotificationGatewayMethodByKey(methodKey);
               // var m = method.GetNotificationMessagesByMethod(methodKey);
               // messages.AddRange(m.Select(AutoMapper.Mapper.Map<INotificationMessage, NotificationMessageDisplay>));
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("{0}", ex.Message));
            }
            return messages;
        }

        /// <summary>
        /// Returns NotificationMessages by method key
        /// 
        /// GET /umbraco/Merchello/NotificationsApi/GetNotificationsByMethod/{key}
        /// </summary>
        /// <param name="id">Key of the notification method</param>
        public IEnumerable<NotificationMessageDisplay> GetNotificationsByMethod(Guid id)
        {
            var notifications = _notificationMessageService.GetNotificationMessagesByMethodKey(id);
            if (notifications == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return notifications.Select(x => x.ToNotificationMessageDisplay());
        }

        /// <summary>
        /// Returns NotificationMessages by method key
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
        /// Adds a <see cref="INotificationMessage"/>
        ///
        /// POST /umbraco/Merchello/NotificationGatewayApi/SaveNotificationMessage
        /// </summary>
        /// <param name="message">POSTed <see cref="NotificationMethodDisplay"/> object</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddNotificationMessage(NotificationMessageDisplay message)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                message.FromAddress = "wesley@proworks.com";

                var provider = _notificationContext.GetProviderByMethodKey(message.MethodKey);

                var method = provider.GetNotificationGatewayMethodByKey(message.MethodKey);

                var notificationMessage = new NotificationMessage(message.MethodKey, message.Name, message.FromAddress);

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
        public HttpResponseMessage PutNotificationMessage(NotificationMessageDisplay message)
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
        public HttpResponseMessage UpdateNotificationMessage(NotificationMessageDisplay message)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                message.Key = Guid.NewGuid();
                message.MethodKey = new Guid("b4d81816-f1b6-4532-9000-edba70295809");
                message.FromAddress = "wesley@proworks.com";

                var provider = _notificationContext.GetProviderByMethodKey(message.MethodKey);

                var method = provider.GetNotificationGatewayMethodByKey(message.MethodKey);

                var notificationMessage = new NotificationMessage(message.MethodKey, message.Name, message.FromAddress);

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
        /// Delete a <see cref="INotificationMethod"/>
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