namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Models;
    using Core.Services;
    using Models.ContentEditing;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;
    using WebApi;

    [PluginController("Merchello")]
    public class NotificationsApiController : MerchelloApiController
    {
        private readonly NotificationMessageService _notificationMessageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsApiController"/> class. 
        /// </summary>
        public NotificationsApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsApiController"/> class. 
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        public NotificationsApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _notificationMessageService = ((ServiceContext)MerchelloContext.Services).NotificationMessageService as NotificationMessageService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsApiController"/> class. 
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
		internal NotificationsApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _notificationMessageService = ((ServiceContext)MerchelloContext.Services).NotificationMessageService as NotificationMessageService;
        }

		/// <summary>
		/// Returns NotificationMessages by key
		/// 
		/// GET /umbraco/Merchello/NotificationsApi/GetNotification/{key}
		/// </summary>
		/// <param name="id">
		/// Key of the NotificationMessages
		/// </param>
		/// <returns>
		/// The <see cref="NotificationMessageDisplay"/>.
		/// </returns>
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
		/// <param name="ids">Keys of the NotificationMessages</param>
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
		/// <param name="id">
		/// Key of the notification method
		/// </param>
		/// <returns>
		/// The collection of <see cref="NotificationMessageDisplay"/>
		/// </returns>
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
		/// <param name="notificationDisplay">
		/// POSTed <see cref="NotificationMessageDisplay"/> object
		/// </param>
		/// <returns>
		/// The <see cref="NotificationMessageDisplay"/>.
		/// </returns>
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
		/// <param name="notificationDisplay">
		/// NotificationMessage object serialized from WebApi
		/// </param>
		/// <returns>
		/// The <see cref="HttpResponseMessage"/>.
		/// </returns>
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
				response = Request.CreateResponse(HttpStatusCode.NotFound, string.Format("{0}", ex.Message));
			}

			return response;
		}

	    /// <summary>
	    /// Deletes an existing NotificationMessage by id (Guid)
	    /// 
	    /// PUT /umbraco/Merchello/notificationsApi/DeleteNotification
	    /// </summary>
	    /// <param name="id">
	    /// Key of the notification method
	    /// </param>
	    /// <returns>
	    /// The <see cref="HttpResponseMessage"/>.
	    /// </returns>
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
