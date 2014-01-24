using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines a shipment rate quote
    /// </summary>
    public interface IShipmentRateQuote
    {
        IShipMethod ShipMethod { get; }
        decimal Rate { get; set; }
    }
}