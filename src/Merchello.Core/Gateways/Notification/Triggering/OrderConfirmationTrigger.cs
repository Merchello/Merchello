using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Notification.Monitors.Models;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>
    /// Represents and OrderConfirmationTrigger
    /// </summary>
    [TriggerFor("OrderConfirmation", Topic.Notifications)]
    public sealed class OrderConfirmationTrigger : NotificationTriggerBase<IPaymentResult, IPaymentResultNotifyModel>
    {
        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        public override void Notify(IPaymentResult model, IEnumerable<string> contacts)
        {
            var confirmation = model.ToOrderConfirmationNotification(contacts);

            foreach (var o in Observers)
            {
                try
                {                  
                    o.OnNext(confirmation);
                }
                catch (Exception ex)
                {
                    o.OnError(ex);
                }
            }
        }

    }
}