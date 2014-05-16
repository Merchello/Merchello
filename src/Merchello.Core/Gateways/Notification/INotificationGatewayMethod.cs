using Merchello.Core.Gateways.Notification.Formatters;

namespace Merchello.Core.Gateways.Notification
{
    public interface INotificationGatewayMethod : IGatewayMethod
    {
        /// <summary>
        /// Sends a <see cref="INotificationGatewayMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationGatewayMessage"/> to be sent</param>
        void Send(INotificationGatewayMessage message);


        void Send(INotificationGatewayMessage message, INotificationFormatter formatter);
    }
}