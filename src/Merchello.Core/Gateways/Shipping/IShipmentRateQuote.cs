using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines a shipment rate quote
    /// </summary>
    public interface IShipmentRateQuote
    {
        /// <summary>
        /// The <see cref="IShipment"/> associated with this rate quote
        /// </summary>
        IShipment Shipment { get; }

        /// <summary>
        /// The <see cref="IShipMethod"/> used to obtain the quote
        /// </summary>
        IShipMethod ShipMethod { get; }

        /// <summary>
        /// The calculated quoted rate for the shipment
        /// </summary>
        decimal Rate { get; set; }
    }
}