using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Web.Shipping
{
    public interface IShippingGatewayProvider : IGatewayProviderBase
    {
        /// <summary>
        /// Returns a collection of all possible ship methods associated with this provider
        /// </summary>
        /// <returns></returns>
        IDictionary<string, string> GetAllShipMethods();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayShipMethod> GetActiveShipMethods(IShipCountry shipCountry);

    }
}