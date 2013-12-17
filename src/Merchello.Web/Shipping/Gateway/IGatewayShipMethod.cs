using Merchello.Core.Models;

namespace Merchello.Web.Shipping.Gateway
{
    public interface IGatewayShipMethod
    {
        /// <summary>
        /// Returns a rate quote for a given shipment
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        decimal QuoteRate(IShipment shipment);
    }
}