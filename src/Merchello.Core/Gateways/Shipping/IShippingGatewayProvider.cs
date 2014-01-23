using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines the base shipping gateway
    /// </summary>
    public interface IShippingGatewayProvider
    {
        /// <summary>
        /// Saves a shipmethod
        /// </summary>
        /// <param name="gatewayShipMethod"></param>
        void SaveShipMethod(IGatewayShipMethod gatewayShipMethod);

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayResource> ListResourcesOffered();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayShipMethod> GetActiveShipMethods(IShipCountry shipCountry);

    }
}