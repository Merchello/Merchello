using Merchello.Core.Gateways.Notification.Monitors;
using Merchello.Core.Observation;

namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Gateways;
    using Core.Gateways.Notification;
    using Core.Models;
    using Core.Services;
    using Models.ContentEditing;
    using Umbraco.Web.Mvc;
    using WebApi;

    /// <summary>
    /// Merchello Back Office Controller Responsible for NotificationGatewayProvider Configurations and Settings
    /// </summary>
    [PluginController("Merchello")]
    public class NotificationGatewayApiController : MerchelloApiController
    {
        /// <summary>
        /// The notification context.
        /// </summary>
        private readonly INotificationContext _notificationContext;

        /// <summary>
        /// The notification message service.
        /// </summary>
        private readonly INotificationMessageService _notificationMessageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGatewayApiController"/> class. 
        /// </summary>
        public NotificationGatewayApiController()
            : this(Core.MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGatewayApiController"/> class. 
        /// </summary>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        public NotificationGatewayApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _notificationContext = ((GatewayContext)MerchelloContext.Gateways).Notification;
            _notificationMessageService = ((ServiceContext)MerchelloContext.Services).NotificationMessageService;
        }

        /// <summary>
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetGatewayResources/{id}
        /// 
        /// </summary>
        /// <param name="id">
        /// The key of the PaymentGatewayProvider
        /// </param>
        /// <returns>
        /// A collection for <see cref="GatewayResourceDisplay"/>
        /// </returns>
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
        /// <returns>
        /// A collection for <see cref="GatewayResourceDisplay"/>
        /// </returns>
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
        /// Returns a list of all of Notification monitors of NotificationMonitorDisplay
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetAllNotificationMonitors/
        /// </summary>
        /// <returns>
        /// A collection of <see cref="NotificationMonitorDisplay"/>
        /// </returns>
        public IEnumerable<NotificationMonitorDisplay> GetAllNotificationMonitors()
        {
            var monitors = MonitorResolver.Current.GetAllMonitors();

            return monitors.Select(x => new NotificationMonitorDisplay()
            {
                MonitorKey = x.MonitorFor().Key,
                Name = x.MonitorFor().Name,
                Alias = x.MonitorFor().ObservableTrigger.ToString()
            });
        }

        /// <summary>
        /// Get all <see cref="NotificationMethodDisplay"/> for a notification provider
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/GetNotificationProviderNotificationMethods/{id}
        /// </summary>
        /// <param name="id">The key (guid) of the NotificationGatewayProvider</param>
        /// <returns>A collection of <see cref="NotificationMethodDisplay"/></returns>
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
        /// <param name="method">
        /// POSTed <see cref="NotificationMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
                    var notificationGatewayMethod = provider.CreateNotificationMethod(gatewayResource, method.Name, method.ServiceCode);

                    provider.SaveNotificationMethod(notificationGatewayMethod);
                }
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Save a <see cref="INotificationMethod"/>
        /// 
        /// PUT /umbraco/Merchello/NotificationGatewayApi/PutNotificationMethod
        /// </summary>
        /// <param name="method">
        /// POSTed <see cref="NotificationMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Delete a <see cref="INotificationMethod"/>
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/DeletNotificationMethod
        /// </summary>
        /// <param name="id">
        /// <see cref="NotificationMethodDisplay"/> key to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Returns a  <see cref="NotificationMessageDisplay"/> by method key
        /// 
        /// GET /umbraco/Merchello/NotificationsApi/GetNotificationsByMethod/{key}
        /// </summary>
        /// <param name="id">
        /// Key of the notification method
        /// </param>
        /// <returns>
        /// The <see cref="NotificationMessageDisplay"/>.
        /// </returns>
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
        /// <param name="message">
        /// POSTed <see cref="NotificationMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Adds a <see cref="INotificationMessage"/>
        /// 
        /// POST /umbraco/Merchello/NotificationGatewayApi/SaveNotificationMessage
        /// </summary>
        /// <param name="message">
        /// POSTed <see cref="NotificationMethodDisplay"/> object
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [AcceptVerbs("POST")]
        public string PutNotificationMessage(NotificationMessageDisplay message)
        {
            try
            {
                var provider = _notificationContext.GetProviderByMethodKey(message.MethodKey);

                var method = provider.GetNotificationGatewayMethodByKey(message.MethodKey);

                var notificationMessage = new NotificationMessage(message.MethodKey, message.Name, message.FromAddress);

                method.SaveNotificationMessage(message.ToNotificationMessage(notificationMessage));
                
                return notificationMessage.Key.ToString();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message)));
            }            
        }

        /// <summary>
        /// Delete a <see cref="INotificationMessage"/>
        /// 
        /// GET /umbraco/Merchello/NotificationGatewayApi/DeleteNotificationMessage
        /// </summary>
        /// <param name="id">
        /// <see cref="NotificationMessageDisplay"/> key to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
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
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("{0}", ex.Message));
            }           
           
            return response;
        }
    }
}