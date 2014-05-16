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
        private IGatewayProviderService _gatewayProviderService;
        private readonly INotificationMethod _notificationMethod;
        
        protected NotificationGatewayMethodBase(IGatewayProviderService gatewayProviderService, INotificationMethod notificationMethod)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(notificationMethod, "notificationMethod");

            _notificationMethod = notificationMethod;
            _gatewayProviderService = gatewayProviderService;
        }

        public virtual void Send(INotificationGatewayMessage message)
        {
            Send(message, new DefaultNotificationFormatter());
        }

        public abstract void Send(INotificationGatewayMessage message, INotificationFormatter formatter);


        /// <summary>
        /// Gets the <see cref="INotificationMethod"/>
        /// </summary>
        protected INotificationMethod NotificationMethod 
        {
            get { return _notificationMethod; }
        }
    }
}