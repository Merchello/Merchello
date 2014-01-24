using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines a an abstract gateway ship method
    /// </summary>
    public abstract class GatewayShipMethodBase : IGatewayShipMethod
    {
        private readonly IShipMethod _shipMethod;
        private readonly IGatewayResource _gatewayResource;
        private readonly IShipCountry _shipCountry;

        protected GatewayShipMethodBase(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry)
        {
            Mandate.ParameterNotNull(gatewayResource, "gatewayResource");
            Mandate.ParameterNotNull(shipMethod, "shipMethod");
            Mandate.ParameterNotNull(shipCountry, "shipCountry");

            _gatewayResource = gatewayResource;
            _shipMethod = shipMethod;
            _shipCountry = shipCountry;
        }

        /// <summary>
        /// Returns a rate quote for a given shipment
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public abstract Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment);

        /// <summary>
        /// Gets the ship method
        /// </summary>
        public IShipMethod ShipMethod
        {
            get { return _shipMethod; }
        }

        /// <summary>
        /// Gets the ship country
        /// </summary>
        public IShipCountry ShipCountry
        {
            get { return _shipCountry; }
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