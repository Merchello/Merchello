namespace Merchello.Core.Gateways.Shipping.RateTable
{
    /// <summary>
    /// Defines the rate table ship method
    /// </summary>
    public interface IRateTableShipMethod : IGatewayShipMethod
    {
        /// <summary>
        /// Gets the <see cref="IShipRateTable"/> for this ship method
        /// </summary>
        IShipRateTable RateTable { get; }
    }
}