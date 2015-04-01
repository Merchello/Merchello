namespace Merchello.Core.Models.MonitorModels
{
    /// <summary>
    /// Represents a ShippingResultModel
    /// </summary>
    internal class ShipmentResultNotifyModel : NotifyModelBase, IShipmentResultNotifyModel
    {
        /// <summary>
        /// Gets or sets the shipment.
        /// </summary>
        /// <value>
        /// The shipment.
        /// </value>
        public IShipment Shipment { get; set; }         

        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        /// <value>
        /// The invoice.
        /// </value>
        public IInvoice Invoice { get; set; }

        /// <summary>
        /// Gets or sets the ship method.
        /// </summary>
        public IShipMethod ShipMethod { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }

    }
}
