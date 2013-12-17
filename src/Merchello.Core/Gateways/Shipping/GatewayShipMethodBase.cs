using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines a an abstract gateway ship method
    /// </summary>
    public abstract class GatewayShipMethodBase : IGatewayShipMethod
    {
        private readonly IShipMethod _shipMethod;

        protected GatewayShipMethodBase(IShipMethod shipMethod)
        {
            Mandate.ParameterNotNull(shipMethod, "shipMethod");
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
        protected IShipMethod ShipMethod
        {
            get { return _shipMethod; }
        }

    }
}