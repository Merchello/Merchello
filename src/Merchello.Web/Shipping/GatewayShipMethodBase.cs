using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Shipping
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
    
    }
}