using Merchello.Core.Models;
using Merchello.Core.Services;
using System.Linq;
using Umbraco.Core;
using Merchello.Core.Events;
using Umbraco.Core.Logging;
using log4net;
using System.Reflection;
using System.Collections.Generic;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents the ShippingContext
    /// </summary>
    public class ShippingEvents : ApplicationEventHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Trigger the SMTP Email Notification for OrderShipped when a Shipment has changed its status to Shipped and/or Delivered.
        /// This will normally result in 2 emails - 1 when the item is shipped, and 1 when it is subsequently delivered.
        /// Note: A CHANGE to the status is required - so the initial ShipmentStatus must NOT be in one of these states.
        /// The OrderShipped.cshtml file examines the shipping state and informs the recipient if the item has been shipped or delivered.
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);
            Log.Info("Setting up Shipment Service, Status Changed processing.");
            ShipmentService.StatusChanged += ShipmentServiceStatusChanged;
        }

        private void ShipmentServiceStatusChanged(IShipmentService sender, StatusChangeEventArgs<IShipment> e)
        {

            var validKeys = new[]
                            {
                            Merchello.Core.Constants.ShipmentStatus.Delivered,
                            Merchello.Core.Constants.ShipmentStatus.Shipped
                        };

            foreach (var shipment in e.StatusChangedEntities)
            {

                if (!validKeys.Contains(shipment.ShipmentStatus.Key)) continue;

                Log.Info(string.Format("Raising 'OrderShipped' notification trigger for shipment no. {0} to {1}", shipment.ShipmentNumber, shipment.Email));

                var contact = new List<string>() { shipment.Email };

                Merchello.Core.Notification.Trigger("OrderShipped", shipment, contact, Merchello.Core.Observation.Topic.Notifications);
            }
        }
    }

    
    
}