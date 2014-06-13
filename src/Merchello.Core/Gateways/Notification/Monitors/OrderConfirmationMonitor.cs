using System.Linq;
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
            if (!value.PaymentSuccess) return;
            if (!Messages.Any()) return;

            foreach (var message in Messages)
            {
                if (value.Contacts.Any())
                {
                    // add the additional contacts to the recipients list
                    if (!message.Recipients.EndsWith(";")) message.Recipients += ";";
                    message.Recipients = string.Format("{0}{1}", message.Recipients, string.Join(";", value.Contacts));

                }
                
            }
        }
    }
}