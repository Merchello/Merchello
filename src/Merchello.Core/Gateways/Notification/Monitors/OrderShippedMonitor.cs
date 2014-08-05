﻿namespace Merchello.Core.Gateways.Notification.Monitors
{
    using System.Linq;
    using Formatters;    
    using Models;
    using Models.MonitorModels;
    using Observation;
    using Triggering;

    /// <summary>
    /// Represents and order shipped monitor
    /// </summary>
    [MonitorFor("1078FE96-6C73-4CC7-A92D-496AFB2FC3CB", typeof(OrderShippedTrigger), "Order Shipped Message (Pattern Replace)")]
    public class OrderShippedMonitor : NotificationMonitorBase<IShipment>
    {
        public OrderShippedMonitor(INotificationContext notificationContext)
            : base(notificationContext)
        {            
        }

        /// <summary>
        /// Trigger call to notifify the monitor of a change
        /// </summary>
        /// <param name="value">
        /// The model to be used by the monitor
        /// </param>
        public override void OnNext(IShipment value)
        {
            if (!Messages.Any()) return;

            var formatter = PatternReplaceFormatter.GetPatternReplaceFormatter();
                                                                                        
            // Add the replaceable patterns from the invoice
            formatter.AddOrUpdateReplaceablePattern(value.ReplaceablePatterns());
                                                                                
            foreach (var message in Messages)
            {
                if (message.SendToCustomer)
                {
                    // add the additional contacts to the recipients list
                    if (!message.Recipients.EndsWith(";")) 
                        message.Recipients += ";";      

                    if (message.Recipients[0] == ';')
                        message.Recipients = message.Recipients.TrimStart(';');

                    message.Recipients = string.Format("{0}{1}", message.Recipients, string.Join(";", value.Email));
                }            

                // send the message
                NotificationContext.Send(message, formatter);
            }
        }
    }
}
                                                                                  