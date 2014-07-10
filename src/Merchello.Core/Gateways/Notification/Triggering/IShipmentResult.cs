using Merchello.Core.Models;
using Merchello.Core.Models.MonitorModels;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>
    /// The ShipmentResult interface.
    /// </summary>
    public interface IShipmentResult : INotifyModel
    {
        /// <summary>
        /// Gets the shipment.
        /// </summary>
        /// <value>
        /// The shipment.
        /// </value>
        IShipment Shipment { get; }

        /// <summary>
        /// Gets the invoice.
        /// </summary>
        /// <value>
        /// The invoice.
        /// </value>
        IInvoice Invoice { get; }
    }
}