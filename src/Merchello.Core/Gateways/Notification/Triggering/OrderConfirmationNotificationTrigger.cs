using System;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    [ObservableTriggerFor("OrderConfirmation", ObservableTopic.Notifications)]
    public sealed class OrderConfirmationNotificationTrigger : ObservableTriggerBase<IPaymentResult>
    {
        public override IDisposable Subscribe(IObserver<IPaymentResult> observer)
        {

            return GetUnsubscriber(observer);
        }
    }
}