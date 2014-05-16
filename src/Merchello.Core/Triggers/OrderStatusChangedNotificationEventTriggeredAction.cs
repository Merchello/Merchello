using System;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Logging;

namespace Merchello.Core.Triggers
{
    [EventTriggeredActionFor("Notification", typeof(OrderService), "StatusChanged")]
    internal class OrderStatusChangedNotificationEventTriggeredAction : EventTriggeredActionBase
    {
        public override void Invoke(Object sender, EventArgs e)
        {
            try
            {
                var args = e as StatusChangeEventArgs<IOrder>;
            }
            catch (Exception ex)
            {                
                LogHelper.Error<OrderStatusChangedNotificationEventTriggeredAction>("Invoke failed", ex);
            }            
        }
    }
}