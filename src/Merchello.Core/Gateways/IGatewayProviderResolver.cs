using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways
{
    public interface IGatewayProviderResolver
    {
        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/>s by type
        /// </summary>
        IEnumerable<IGatewayProvider> GetActiveProviders<T>() where T : GatewayProviderBase;

        /// <summary>
        /// Gets a collection of instantiated gateway providers
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> CreateInstances<T>(GatewayProviderType gatewayProviderType) where T : GatewayProviderBase;

        /// <summary>
        /// Returns an instantiation of a <see cref="T"/>
        /// </summary>
        /// <param name="provider"><see cref="IGatewayProvider"/></param>
        /// <returns></returns>
        T CreateInstance<T>(IGatewayProvider provider) where T : GatewayProviderBase;

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <returns>An instantiated GatewayProvider</returns>
        T CreateInstance<T>(Guid gatewayProviderKey) where T : GatewayProviderBase;
    }
}