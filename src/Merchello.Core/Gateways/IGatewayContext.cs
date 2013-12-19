using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Gateways
{
    public interface IGatewayContext
    {
        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/>s by type
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayProvider> GetGatewayProviders(GatewayProviderType gatewayProviderType);

        /// <summary>
        /// Returns an instantiation of a <see cref="ShippingGatewayBase"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        ShippingGatewayBase InstantiateShippingGateway(IGatewayProvider provider);

        /// <summary>
        /// Refreshes the <see cref="GatewayBase"/> cache
        /// </summary>
        void RefreshCache();
    }
}