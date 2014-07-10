using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Models.MonitorModels;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>            
    /// Represents and OrdeShippedTrigger
    /// </summary>
    [TriggerFor("OrderShipped", Topic.Notifications)]
    public class OrderShippedTrigger : NotificationTriggerBase<IShipment, IShipment>
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
        protected override void Notify(IShipment model, IEnumerable<string> contacts)
        {
            NotifyMonitors(model);
        }
    }
}
