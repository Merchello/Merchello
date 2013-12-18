using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    public interface IShippingGatewayBase<out T>
        where T : IGatewayShipMethod
    {

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayResource> ListAvailableMethods();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> ActiveShipMethods(IShipCountry shipCountry);

    }
}