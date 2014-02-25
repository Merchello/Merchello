namespace Merchello.Core.Gateways.Shipping.FixedRate
{
    /// <summary>
    /// Defines the rate table ship method
    /// </summary>
    public interface IFixedRateShipMethod : IShippingGatewayMethod
    {
        /// <summary>
        /// Gets the <see cref="IShippingFixedRateTable"/> for this ship method
        /// </summary>
        IShippingFixedRateTable RateTable { get; }

        /// <summary>
        /// Gets the <see cref="FixedRateShipMethod.QuoteType"/> for this ship method
        /// </summary>
        FixedRateShipMethod.QuoteType RateTableType { get; }
    }
}