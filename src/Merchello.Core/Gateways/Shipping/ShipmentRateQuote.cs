using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents a shipment rate quote
    /// </summary>
    public class ShipmentRateQuote : IShipmentRateQuote
    {
        public ShipmentRateQuote(IShipMethod shipMethod)
        {
            ShipMethod = shipMethod;
        }

        public IShipMethod ShipMethod { get; private set; }
        public decimal Rate { get; set; }

    }
}