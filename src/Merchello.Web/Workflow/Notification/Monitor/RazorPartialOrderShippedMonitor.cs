namespace Merchello.Web.Workflow.Notification.Monitor
{
    using Merchello.Core.Gateways.Notification;
    using Merchello.Core.Gateways.Notification.Triggering;
    using Merchello.Core.Models.MonitorModels;
    using Merchello.Core.Observation;

    /// <summary>
    /// The razor partial order shipped monitor.
    /// </summary>
    [MonitorFor("9927CB8B-00AF-4DDB-BB68-987EF89C60EF", typeof(PartialOrderShippedTrigger), "Partial Order Shipped (Razor)", true)]
    public class RazorPartialOrderShippedMonitor : RazorMonitorBase<IShipmentResultNotifyModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RazorPartialOrderShippedMonitor"/> class.
        /// </summary>
        /// <param name="notificationContext">
        /// The <see cref="NotificationContext"/>.
        /// </param>
        public RazorPartialOrderShippedMonitor(INotificationContext notificationContext)
            : base(notificationContext)
        {
        }
    }
}