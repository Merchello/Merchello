using System;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    [ObservableTriggerFor("ACFB0BA8-9407-4438-AA37-973E7A700368", "Order Confirmation", "OrderConfirmation", ObservableTopic.Notifications)]
    public sealed class OrderConfirmationNotificationTrigger : ObservableTriggerBase<IPaymentResult>
    {
        public override IDisposable Subscribe(IObserver<IPaymentResult> observer)
        {

            return GetUnsubscriber(observer);
        }
    }
}