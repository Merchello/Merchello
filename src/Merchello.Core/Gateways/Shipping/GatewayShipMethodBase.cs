using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines a an abstract gateway ship method
    /// </summary>
    public abstract class GatewayShipMethodBase : IGatewayShipMethod
    {
        private readonly IShipMethod _shipMethod;
        private readonly IGatewayResource _gatewayResource;

        protected GatewayShipMethodBase(IGatewayResource gatewayResource, IShipMethod shipMethod)
        {
            Mandate.ParameterNotNull(gatewayResource, "gatewayResource");
            Mandate.ParameterNotNull(shipMethod, "shipMethod");

            _gatewayResource = gatewayResource;
            _shipMethod = shipMethod;
        }

        /// <summary>
        /// Returns a rate quote for a given shipment
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public abstract decimal QuoteShipment(IShipment shipment);

        /// <summary>
        /// Gets the ship method
        /// </summary>
        public IShipMethod ShipMethod
        {
            get { return _shipMethod; }
        }

        /// <summary>
        /// Gets the gateway resource
        /// </summary>
        public IGatewayResource GatewayResource
        {
            get { return _gatewayResource; }
        }
    }
}