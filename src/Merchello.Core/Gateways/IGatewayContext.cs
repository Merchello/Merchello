using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;

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
        /// Returns an instantiation of a <see cref="ShippingGatewayProvider"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        ShippingGatewayProvider GetShippingGatewayProvider(IGatewayProvider provider);

        /// <summary>
        /// Refreshes the <see cref="GatewayProviderBase"/> cache
        /// </summary>
        void RefreshCache();
    }
}