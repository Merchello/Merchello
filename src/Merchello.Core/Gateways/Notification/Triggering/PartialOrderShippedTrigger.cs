namespace Merchello.Core.Gateways.Notification.Triggering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.MonitorModels;
    using Merchello.Core.Observation;

    /// <summary>
    /// The partial order shipped trigger.
    /// </summary>
    [TriggerFor("PartialOrderShipped", Topic.Notifications)]
    public class PartialOrderShippedTrigger : NotificationTriggerBase<IShipment, IShipmentResultNotifyModel>
    {
        /// <summary>
        /// The <see cref="ShipmentResultNotifyModelFactory"/>.
        /// </summary>
        private readonly Lazy<ShipmentResultNotifyModelFactory> _factory = new Lazy<ShipmentResultNotifyModelFactory>(() => new ShipmentResultNotifyModelFactory()); 


        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="contacts">
        /// An additional list of contacts
        /// </param>
        protected override void Notify(IShipment model, IEnumerable<string> contacts)
        {
            if (model == null || !model.Items.Any()) return;

            var notifyModel = _factory.Value.Build(model);

            NotifyMonitors(notifyModel);
        }
    }
}
