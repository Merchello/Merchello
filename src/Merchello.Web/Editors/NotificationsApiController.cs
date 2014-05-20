using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Merchello.Core.Builders;
using Merchello.Core.Models;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using Merchello.Web.Models.ContentEditing;
using System.Net;
using System.Net.Http;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class NotificationsApiController : MerchelloApiController
    {
        private readonly NotificationMessageService _notificationMessageService;

        /// <summary>
        /// Constructor
        /// </summary>
        public NotificationsApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public NotificationsApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _notificationMessageService = MerchelloContext.Services.NotificationMessageService as NotificationMessageService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
		internal NotificationsApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
			_notificationMessageService = MerchelloContext.Services.NotificationMessageService as NotificationMessageService;
        }

		/// <summary>
		/// Returns NotificationMessages by key
		/// 
		/// GET /umbraco/Merchello/NotificationsApi/GetNotification/{key}
		/// </summary>
		/// <param name="id">Key of the NotificationMessages</param>
		public NotificationMessageDisplay GetNotification(Guid id)
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
		public IEnumerable<NotificationMessageDisplay> GetNotifications([FromUri]IEnumerable<Guid> ids)
		{
			var notifications = _notificationMessageService.GetByKeys(ids);
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
		/// Adds a NotificationMessages
		///
		/// POST /umbraco/Merchello/NotificationsApi/NewNotification
		/// </summary>
		/// <param name="notificationDisplay">POSTed <see cref="NotificationMessageDisplay"/> object</param>
		/// <remarks>
		/// </remarks>
		[AcceptVerbs("POST", "GET")]
		public NotificationMessageDisplay NewNotification(NotificationMessageDisplay notificationDisplay)
		{
			INotificationMessage notification;

			try
			{
				throw new NotImplementedException();
				notification = notificationDisplay.ToNotificationMessage(notification);
				var existingNotification = _notificationMessageService.GetByKey(notification.Key);
				if (existingNotification != null)
				{
					throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Conflict));
				}

				_notificationMessageService.Save(notification);
			}
			catch (Exception e)
			{
				throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError));
			}

			return notification.ToNotificationMessageDisplay();
		}

		/// <summary>
		/// Updates an existing NotificationMessage
		///
		/// PUT /umbraco/Merchello/notificationsApi/PutNotification
		/// </summary>
		/// <param name="notificationDisplay">NotificationMessage object serialized from WebApi</param>
		[AcceptVerbs("PUT", "POST")]
		public HttpResponseMessage PutNotification(NotificationMessageDisplay notificationDisplay)
		{
			var response = Request.CreateResponse(HttpStatusCode.OK);

			try
			{
				var notification = _notificationMessageService.GetByKey(notificationDisplay.Key);
				notification = notificationDisplay.ToNotificationMessage(notification);

				_notificationMessageService.Save(notification);
			}
			catch (Exception ex)
			{
				response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", ex.Message));
			}

			return response;
		}

	    /// <summary>
	    /// Deletes an existing NotificationMessage by id (Guid)
	    ///
	    /// PUT /umbraco/Merchello/notificationsApi/DeleteNotification
	    /// </summary>
		/// <param name="id">Key of the notification method</param>
	    [AcceptVerbs("PUT", "POST")]
	    public HttpResponseMessage DeletesNotification(Guid id)
	    {
			var notification = _notificationMessageService.GetByKey(id);
			if (notification == null)
		    {
			    return Request.CreateResponse(HttpStatusCode.NotFound);
		    }

			_notificationMessageService.Delete(notification);

		    return Request.CreateResponse(HttpStatusCode.OK);
	    }

    }
}
