using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Notification.Formatters;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Represents a NotificationGatewayMethodBase object
    /// </summary>
    public abstract class NotificationGatewayMethodBase : INotificationGatewayMethod
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly INotificationMethod _notificationMethod;
        
        protected NotificationGatewayMethodBase(IGatewayProviderService gatewayProviderService, INotificationMethod notificationMethod)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(notificationMethod, "notificationMethod");

            _notificationMethod = notificationMethod;
            _gatewayProviderService = gatewayProviderService;
        }

        public INotificationMessage CreateNotificationMessage(string name, string description, string fromAddress, IEnumerable<string> recipients, string bodyText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/> given it's unique Key (Guid)
        /// </summary>
        /// <param name="messageKey">The unique key (Guid) of the <see cref="IFormattedNotificationMessage"/></param>
        public virtual bool Send(Guid messageKey)
        {
            return Send(messageKey, new DefaultNotificationFormatter());
        }

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/> given it's unique Key (Guid)
        /// </summary>
        /// <param name="messageKey">The unique key (Guid) of the <see cref="IFormattedNotificationMessage"/></param>
        /// <param name="formatter">The <see cref="INotificationFormatter"/> to use to format the message</param>
        public virtual bool Send(Guid messageKey, INotificationFormatter formatter)
        {
            var message = _gatewayProviderService.GetNotificationMessageByKey(messageKey);

            return Send(message, formatter);
        }

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        public virtual bool Send(INotificationMessage notificationMessage)
        {
            return Send(notificationMessage, new DefaultNotificationFormatter());
        }

        /// <summary>
        /// Sends a <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        /// <param name="formatter">The <see cref="INotificationFormatter"/> to use to format the message</param>
        public virtual bool Send(INotificationMessage notificationMessage, INotificationFormatter formatter)
        {
            return PerformSend(new FormattedNotificationMessage(notificationMessage, formatter)); 
        }

        /// <summary>
        /// Does the actual work of sending the <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        public abstract bool PerformSend(IFormattedNotificationMessage message);
        

        /// <summary>
        /// Gets the <see cref="INotificationMethod"/>
        /// </summary>
        public INotificationMethod NotificationMethod 
        {
            get { return _notificationMethod; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        protected IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }
    }
}