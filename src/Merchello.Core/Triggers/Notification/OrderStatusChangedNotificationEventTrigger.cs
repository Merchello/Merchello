using System;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Logging;

namespace Merchello.Core.Triggers.Notification
{
    [EventTrigger("Notification", typeof(OrderService), "StatusChanged")]
    internal class OrderStatusChangedNotificationEventTrigger : NotificationEventTriggerBase
    {
        public override void Invoke(Object sender, EventArgs e)
        {
            try
            {
                var args = e as StatusChangeEventArgs<IOrder>;
            }
            catch (Exception ex)
            {                
                LogHelper.Error<OrderStatusChangedNotificationEventTrigger>("Invoke failed", ex);
            }            
        }
    }
}