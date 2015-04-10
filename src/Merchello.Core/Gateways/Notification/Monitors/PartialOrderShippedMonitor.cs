namespace Merchello.Core.Gateways.Notification.Monitors
{
    using System.Linq;
    using Formatters;
    using Models;
    using Models.MonitorModels;
    using Observation;
    using Triggering;

    /// <summary>
    /// Represents and order shipped monitor
    /// </summary>
    [MonitorFor("45016334-AB36-4496-BFC4-CD860F2A7EFF", typeof(PartialOrderShippedTrigger), "Partial Order Shipped Message (Pattern Replace)")]
    public class PartialOrderShippedMonitor : NotificationMonitorBase<IShipmentResultNotifyModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialOrderShippedMonitor"/> class.
        /// </summary>
        /// <param name="notificationContext">
        /// The notification context.
        /// </param>
        public PartialOrderShippedMonitor(INotificationContext notificationContext)
            : base(notificationContext)
        {
        }

        /// <summary>
        /// Trigger call to notifify the monitor of a change
        /// </summary>
        /// <param name="value">
        /// The model to be used by the monitor
        /// </param>
        public override void OnNext(IShipmentResultNotifyModel value)
        {
            if (!Messages.Any()) return;

            var formatter = PatternReplaceFormatter.GetPatternReplaceFormatter();

            // Add the replaceable patterns from the invoice
            formatter.AddOrUpdateReplaceablePattern(value.ReplaceablePatterns());

            foreach (var message in Messages)
            {
                if (message.SendToCustomer)
                {
                    // add the additional contacts to the recipients list
                    if (!message.Recipients.EndsWith(";"))
                        message.Recipients += ";";

                    if (message.Recipients[0] == ';')
                        message.Recipients = message.Recipients.TrimStart(';');

                    var email = value.Shipment.Email ?? value.Invoice.BillToEmail;
                    message.Recipients = string.Format("{0}{1}", message.Recipients, string.Join(";", email));
                }

                // send the message
                NotificationContext.Send(message, formatter);
            }
        }
    }
}
