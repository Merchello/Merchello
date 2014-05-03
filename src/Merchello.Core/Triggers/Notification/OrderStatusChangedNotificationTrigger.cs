using System;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Logging;

namespace Merchello.Core.Triggers.Notification
{
    [TriggerAction("Notification", typeof(OrderService), "StatusChanged")]
    internal class OrderStatusChangedNotificationTrigger : NotificationTriggerBase
    {
        public OrderStatusChangedNotificationTrigger(IMerchelloContext merchelloContext) 
            : base(merchelloContext)
        { }

        public override void Invoke(object sender, EventArgs e)
        {
            try
            {
                var statusChangedArgs = e as StatusChangeEventArgs<IOrder>;
            }
            catch (Exception ex)
            {                
                LogHelper.Error<OrderStatusChangedNotificationTrigger>("Invoke failed", ex);
            }            
        }
    }
}