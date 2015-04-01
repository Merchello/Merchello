namespace Merchello.Core.Models.MonitorModels
{
    using Merchello.Core.Models;

    /// <summary>
    /// The ShipmentResult interface.
    /// </summary>
    public interface IShipmentResultNotifyModel : INotifyModel
    {
        /// <summary>
        /// Gets or sets the shipment.
        /// </summary>
        /// <value>
        /// The shipment.
        /// </value>
        IShipment Shipment { get; set;  }

        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        /// <value>
        /// The invoice.
        /// </value>
        IInvoice Invoice { get; set; }

        /// <summary>
        /// Gets or sets the ship method.
        /// </summary>
        IShipMethod ShipMethod { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        string CurrencySymbol { get; set; }
    }
}