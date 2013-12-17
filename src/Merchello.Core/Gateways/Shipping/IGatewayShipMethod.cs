using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    public interface IGatewayShipMethod
    {
        /// <summary>
        /// Returns a rate quote for a given shipment
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        decimal QuoteShipment(IShipment shipment);
    }
}