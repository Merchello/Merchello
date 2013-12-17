using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Shipping.Gateway
{
    public abstract class GatewayShipMethodBase : IGatewayShipMethod
    {
        private readonly IShipMethod _shipMethod;

        protected GatewayShipMethodBase(IShipMethod shipMethod)
        {
            Mandate.ParameterNotNull(shipMethod, "shipMethod");
            _shipMethod = shipMethod;
        }

        public abstract decimal QuoteRate(IShipment shipment);

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IGatewayMethod> ListAvailableMethods();
    
    }
}