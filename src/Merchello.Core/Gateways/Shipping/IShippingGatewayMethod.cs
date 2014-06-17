namespace Merchello.Core.Gateways.Shipping
{
    using Models;
    using Umbraco.Core;

    public interface IShippingGatewayMethod : IGatewayMethod
    {
        /// <summary>
        /// Gets the <see cref="IShipMethod"/>
        /// </summary>
        IShipMethod ShipMethod { get; }

        /// <summary>
        /// Gets the <see cref="IGatewayResource"/>
        /// </summary>
        IGatewayResource GatewayResource { get; }

        /// <summary>
        /// Returns a rate quote for a given shipment
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment);

    }
}