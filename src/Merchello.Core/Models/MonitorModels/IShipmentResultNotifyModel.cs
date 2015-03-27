namespace Merchello.Core.Models.MonitorModels
{
    using Merchello.Core.Models;

    /// <summary>
    /// The ShipmentResult interface.
    /// </summary>
    public interface IShipmentResultNotifyModel : INotifyModel
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