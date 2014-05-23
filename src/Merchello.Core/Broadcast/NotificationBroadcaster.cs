using System;
using System.Collections.Generic;
using Merchello.Core.Events;

namespace Merchello.Core.Broadcast
{
    [BroadcastObserverFor("99AB816F-C1F5-4921-B0B5-0290C5E86498", "New Invoice", "NewInvoice")]
    [BroadcastObserverFor("A0AA97CB-02C1-48F1-82BF-EFEAA17B2F80", "Order Confirmation", "OrderConfirmation")]
    [BroadcastObserverFor("C02DC640-9A6C-4BBC-AF5A-2EB355BEE41E", "Shipment Shipped", "ShipmentShipped")]
    [BroadcastObserverFor("4DF58706-F569-4A29-ADC2-9BDA0442306A", "Payment Received", "PaymentReceived")]
    [BroadcastObserverFor("15E7D98A-55B9-48D2-93C0-38857630BBCE", "Order Canceled", "OrderCanceled")]
    public class NotificationBroadcaster : BroadcasterBase<NotificationEventArgs>, IBroadcaster<object>
    {
        public void Broadcast(Guid triggerKey, object model)
        {
            Broadcast(triggerKey, model, new List<string>());
        }

        public void Broadcast(Guid triggerKey, object model, IEnumerable<string> contacts)
        {
            OnBroadcasting( new NotificationEventArgs(triggerKey, model, contacts));
        }    
    }
}