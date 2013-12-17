using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models;
using IGatewayMethod = Merchello.Core.Models.IGatewayMethod;

namespace Merchello.Web.Shipping.Gateway
{
    public interface IShippingGatewayProvider<out T>
        where T : IGatewayShipMethod
    {

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayMethod> ListAvailableMethods();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetActiveShipMethods(IShipCountry shipCountry);

    }
}