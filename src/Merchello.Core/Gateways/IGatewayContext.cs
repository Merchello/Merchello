using System;
using System.Collections.Generic;
using Merchello.Core.Models;

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
        /// Gets a collection of instantiated gateway providers
        /// </summary>
        /// <param name="gatewayProviderType"></param>
        /// <returns></returns>
        IEnumerable<GatewayProviderBase> ResolveByGatewayProviderType(GatewayProviderType gatewayProviderType);
            
        /// <summary>
        /// Returns an instantiation of a <see cref="T"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        T ResolveByGatewayProvider<T>(IGatewayProvider provider) where T : GatewayProviderBase;

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <returns>An instantiated GatewayProvider</returns>
        T ResolveByKey<T>(Guid gatewayProviderKey) where T : GatewayProviderBase;

        /// <summary>
        /// Refreshes the <see cref="GatewayProviderBase"/> cache
        /// </summary>
        void RefreshCache();
    }
}