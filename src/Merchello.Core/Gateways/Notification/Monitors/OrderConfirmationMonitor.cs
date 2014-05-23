using Merchello.Core.Gateways.Notification.Formatters;
using Merchello.Core.Gateways.Notification.Triggering;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Monitors
{
    [ObserverMonitorFor(typeof(OrderConfirmationNotificationTrigger), "Order Confirmation Message (Pattern Replace)")]
    public class OrderConfirmationMonitor : NotificationMonitorBase<IPaymentResult>
    {
        public override void OnNext(IPaymentResult value)
        {
            throw new System.NotImplementedException();
        }
    }
}