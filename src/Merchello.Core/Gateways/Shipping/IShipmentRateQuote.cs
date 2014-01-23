namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines a shipment rate quote
    /// </summary>
    public interface IShipmentRateQuote
    {
        decimal Rate { get; set; }
    }
}