namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents a shipment rate quote
    /// </summary>
    public class ShipmentRateQuote : IShipmentRateQuote
    {
        public decimal Rate { get; set; }

    }
}