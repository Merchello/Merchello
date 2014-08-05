namespace Merchello.Core.Gateways.Notification.Triggering
{
    using System.Collections.Generic;
    using Models.MonitorModels;
    using Observation;
    using Payment;

    /// <summary>
    /// Represents and OrderConfirmationTrigger
    /// </summary>
    [TriggerFor("OrderConfirmation", Topic.Notifications)]
    public sealed class OrderConfirmationTrigger : NotificationTriggerBase<IPaymentResult, IPaymentResultMonitorModel>
    {
        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="contacts">
        /// An additional list of contacts
        /// </param>
        protected override void Notify(IPaymentResult model, IEnumerable<string> contacts)
        {            
           NotifyMonitors(model.ToOrderConfirmationNotification(contacts));           
        }
    }
}