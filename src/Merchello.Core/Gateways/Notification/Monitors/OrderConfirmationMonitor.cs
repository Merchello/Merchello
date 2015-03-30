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
    [MonitorFor("5DB575B5-0728-4B31-9B37-E9CF6C12E0AA", typeof(OrderConfirmationTrigger), "Order Confirmation Message (Pattern Replace)")]
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
        /// Trigger call to notifify the monitor of a change
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
            formatter.AddOrUpdateReplaceablePattern(value.Invoice.ReplaceablePatterns());

            // get shipping information if any
            var invoice = value.Invoice;
            var shippingLineItems = invoice.ShippingLineItems();
            if (shippingLineItems.Any())
            {
                // just use the first one
                var shipment = shippingLineItems.FirstOrDefault().ExtendedData.GetShipment<InvoiceLineItem>();
                formatter.AddOrUpdateReplaceablePattern(shipment.ReplaceablePatterns());
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