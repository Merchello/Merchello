namespace Merchello.Web.UI.Tests
{
    using System;
    using Merchello.Core;
    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Umbraco.Core;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Summary description for ShipmentNotificationsHandler
    /// </summary>
    public class ShipmentNotificationsHandler : ApplicationEventHandler
    {
                /// <summary>
        /// The Umbraco Application Starting event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            ShipmentService.StatusChanged += ShipmentServiceOnStatusChanged;
        }

        private void ShipmentServiceOnStatusChanged(IShipmentService sender, StatusChangeEventArgs<IShipment> statusChangeEventArgs)
        {
            foreach (var shipment in statusChangeEventArgs.StatusChangedEntities)
            {
                Notification.Trigger("OrderShipped", shipment, new[] { "noreply@merchello.com" });
            }
        }
    }
}
