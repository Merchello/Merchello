namespace Merchello.Core.Gateways.Notification
{
    using System.Collections.Generic;
    using Formatters;
    using Models;
    using Services;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a NotificationGatewayMethodBase object
    /// </summary>
    public abstract class NotificationGatewayMethodBase : INotificationGatewayMethod
    {
        private readonly IGatewayProviderService _gatewayProviderService;

        private readonly INotificationMethod _notificationMethod;

        private IEnumerable<INotificationMessage> _notificationMessages;

        protected NotificationGatewayMethodBase(IGatewayProviderService gatewayProviderService, INotificationMethod notificationMethod)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(notificationMethod, "notificationMethod");

            _notificationMethod = notificationMethod;
            _gatewayProviderService = gatewayProviderService;
        }

        /// <summary>
        /// Gets the <see cref="INotificationMethod"/>
        /// </summary>
        public INotificationMethod NotificationMethod
        {
            get { return _notificationMethod; }
        }
       
        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s associated with this NotificationMethod
        /// </summary>
        public IEnumerable<INotificationMessage> NotificationMessages
        {
            get
            {
                return _notificationMessages ??
                       (_notificationMessages =
                           GatewayProviderService.GetNotificationMessagesByMethodKey(_notificationMethod.Key));
            }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        protected IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }

        /// <summary>
        /// Creates a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="name">A name for the message (used in Back Office UI)</param>
        /// <param name="description">A description for the message (used in Back Office UI)</param>
        /// <param name="fromAddress">The senders or "From Address"</param>
        /// <param name="recipients">A collection of recipients</param>
        /// <param name="bodyText">The body text for the message</param>
        /// <returns>A <see cref="INotificationMessage"/></returns>
        public INotificationMessage CreateNotificationMessage(string name, string description, string fromAddress, IEnumerable<string> recipients, string bodyText)
        {
            var attempt = GatewayProviderService.CreateNotificationMessageWithKey(_notificationMethod.Key, name,description, fromAddress, recipients, bodyText);

            if (attempt.Success)
            {
                _notificationMessages = null;

                return attempt.Result;
            }
            
            LogHelper.Error<NotificationGatewayMethodBase>("Failed to create and save a notification message", attempt.Exception);

            throw attempt.Exception;
        }

        /// <summary>
        /// Saves a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationMessage"/> to be saved</param>
        public void SaveNotificationMessage(INotificationMessage message)
        {
            GatewayProviderService.Save(message);

            _notificationMessages = null;
        }

        /// <summary>
        /// Deletes a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationMessage"/> to be deleted</param>
        public void DeleteNotificationMessage(INotificationMessage message)
        {
            GatewayProviderService.Delete(message);

            _notificationMessages = null;
        }

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        public virtual void Send(INotificationMessage notificationMessage)
        {
            Send(notificationMessage, new DefaultFormatter());
        }

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        /// <param name="formatter">The <see cref="IFormatter"/> to use to format the message</param>
        public virtual void Send(INotificationMessage notificationMessage, IFormatter formatter)
        {
            PerformSend(new FormattedNotificationMessage(notificationMessage, formatter)); 
        }

        /// <summary>
        /// Does the actual work of sending the <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        public abstract void PerformSend(IFormattedNotificationMessage message);        
    }
}