using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Notification.Monitors.Models;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>
    /// Represents and OrderConfirmationNotificationTrigger
    /// </summary>
    [ObservableTriggerFor("OrderConfirmation", ObservableTopic.Notifications)]
    public sealed class OrderConfirmationNotificationTrigger : NotificationTriggerBase<IPaymentResult, IPaymentResultNotifyModel>
    {
        public override void Update(IPaymentResult model, IEnumerable<string> contacts)
        {
            foreach (var o in Observers)
            {
                try
                {

                    o.OnNext(model.ToOrderConfirmationNotification());
                }
                catch (Exception ex)
                {
                    o.OnError(ex);
                }
            }
        }
    }
}