using Merchello.Core.Gateways.Notification.Triggering;
using Merchello.Core.Models.MonitorModels;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Monitors
{
    [MonitorFor("5DB575B5-0728-4B31-9B37-E9CF6C12E0AA", typeof(OrderConfirmationTrigger), "Order Confirmation Message (Pattern Replace)")]
    public class OrderConfirmationMonitor : NotificationMonitorBase<IPaymentResultMonitorModel>
    {
        public OrderConfirmationMonitor(INotificationContext notificationContext) 
            : base(notificationContext)
        { }

        public override void OnNext(IPaymentResultMonitorModel value)
        {
            throw new System.NotImplementedException();

        }
    }
}