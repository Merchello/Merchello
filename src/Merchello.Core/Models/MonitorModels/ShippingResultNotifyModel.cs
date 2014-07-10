using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Gateways.Notification.Triggering;

namespace Merchello.Core.Models.MonitorModels
{
    /// <summary>
    /// Represents a ShippingResultModel
    /// </summary>
    internal class ShippingResultNotifyModel : NotifyModelBase, IShipmentResult
    {
        /// <summary>
        /// Gets the shipment.
        /// </summary>
        /// <value>
        /// The shipment.
        /// </value>
        public IShipment Shipment { get; set; }         

        /// <summary>
        /// Gets the invoice.
        /// </summary>
        /// <value>
        /// The invoice.
        /// </value>
        public IInvoice Invoice { get; set; }
    }
}
