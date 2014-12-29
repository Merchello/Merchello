namespace Merchello.Core.Gateways.Shipping
{
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// Defines a ShippingGatewayMethod.
    /// </summary>
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
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <returns>
        /// The shipment rate quote <see cref="Attempt{IShipmentRateQuote}"/>.
        /// </returns>
        Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment);

    }
}