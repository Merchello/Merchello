using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models.MonitorModels;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>
    /// Represents and OrderConfirmationTrigger
    /// </summary>
    [TriggerFor("OrderConfirmation", Topic.Notifications)]
    public sealed class OrderConfirmationTrigger : NotificationTriggerBase<IPaymentResult, IPaymentResultMonitorModel>
    {
        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        protected override void Notify(IPaymentResult model, IEnumerable<string> contacts)
        {            
           NotifyMonitors(model.ToOrderConfirmationNotification(contacts));           
        }

    }
}