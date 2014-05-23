using Merchello.Core.Gateways.Payment;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>
    /// Represents and OrderConfirmationNotificationTrigger
    /// </summary>
    [ObservableTriggerFor("OrderConfirmation", ObservableTopic.Notifications)]
    public sealed class OrderConfirmationNotificationTrigger : ObservableTriggerBase<IPaymentResult>
    {        
        protected override void Notify(IPaymentResult value)
        {
            
        }

    }
}