namespace Merchello.Core.Gateways.Notification.Monitors
{
    using System.Linq;
    using Formatters;    
    using Models;
    using Models.MonitorModels;
    using Observation;
    using Triggering;

    /// <summary>
    /// Represents and order confirmation monitor
    /// </summary>
    [MonitorFor("5DB575B5-0728-4B31-9B37-E9CF6C12E0AA", typeof(OrderConfirmationTrigger), "Order Confirmation (Legacy)")]
    public class OrderConfirmationMonitor : NotificationMonitorBase<IPaymentResultMonitorModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderConfirmationMonitor"/> class.
        /// </summary>
        /// <param name="notificationContext">
        /// The notification context.
        /// </param>
        public OrderConfirmationMonitor(INotificationContext notificationContext)
            : base(notificationContext)
        {            
        }

        /// <summary>
        /// Trigger call to notify the monitor of a change
        /// </summary>
        /// <param name="value">
        /// The model to be used by the monitor
        /// </param>
        public override void OnNext(IPaymentResultMonitorModel value)
        {
            if (!value.PaymentSuccess) return;

            if (!Messages.Any()) return;

            var formatter = PatternReplaceFormatter.GetPatternReplaceFormatter();

            // Add the replaceable patterns from the invoice
            formatter.AddOrUpdateReplaceablePattern(value.Invoice.ReplaceablePatterns(value.CurrencySymbol));

            if (value.Payment != null)
            {
                formatter.AddOrUpdateReplaceablePattern(value.Payment.ReplaceablePatterns(value.CurrencySymbol));
            }

            if (value.Shipment != null)
            {
                formatter.AddOrUpdateReplaceablePattern(value.Shipment.ReplaceablePatterns(value.CurrencySymbol));
            }

            if (value.ShipMethod != null)
            {
                formatter.AddOrUpdateReplaceablePattern(value.ShipMethod.ReplaceablePatterns());
            }

            foreach (var message in Messages)
            {
                if (value.Contacts.Any() && message.SendToCustomer)
                {
                    // add the additional contacts to the recipients list
                    if (!message.Recipients.EndsWith(";")) 
                        message.Recipients += ";";
                                                                               
                    if (message.Recipients[0] == ';')
                        message.Recipients = message.Recipients.TrimStart(';');

                    message.Recipients = string.Format("{0}{1}", message.Recipients, string.Join(";", value.Contacts));
                }            

                // send the message
                NotificationContext.Send(message, formatter);
            }
        }
    }
}