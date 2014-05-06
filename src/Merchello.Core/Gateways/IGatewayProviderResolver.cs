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
        IEnumerable<GatewayProviderBase> GetActivatedProviders<T>() where T : GatewayProviderBase;

        /// <summary>
        /// Gets a collection of all "activated" providers regardless of type
        /// </summary>
        IEnumerable<GatewayProviderBase> GetActivatedProviders();
            
        /// <summary>
        /// Gets a collection of inactive (not saved) <see cref="IGatewayProvider"/> by type
        /// </summary>
        IEnumerable<GatewayProviderBase> GetAllProviders<T>() where T : GatewayProviderBase;

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <param name="activatedOnly">Search only activated providers</param>
        /// <returns>An instantiated GatewayProvider</returns>
        T GetProviderByKey<T>(Guid gatewayProviderKey, bool activatedOnly = true) where T : GatewayProviderBase;

        /// <summary>
        /// Refreshes the <see cref="GatewayProviderBase"/> cache
        /// </summary>
        void RefreshCache(); 
    }
}