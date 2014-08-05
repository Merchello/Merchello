namespace Merchello.Core.Gateways
{
    using System;
    using System.Collections.Generic;
    using Models;

    /// <summary>
    /// Defines the GatewayProviderResolver.
    /// </summary>
    public interface IGatewayProviderResolver
    {

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProviderSettings"/>s by type
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="GatewayProviderBase"/>
        /// </typeparam>
        /// <returns>
        /// The collection of <see cref="IGatewayProviderSettings"/>s
        /// </returns>
        IEnumerable<GatewayProviderBase> GetActivatedProviders<T>() where T : GatewayProviderBase;

        /// <summary>
        /// Gets a collection of all "activated" providers regardless of type
        /// </summary>
        /// <returns>
        /// The collection of all "activated" providers.
        /// </returns>
        IEnumerable<GatewayProviderBase> GetActivatedProviders();

        /// <summary>
        /// Gets a collection of all providers regardless of type
        /// </summary>
        /// <returns>
        /// The collection of all providers.
        /// </returns>
        IEnumerable<GatewayProviderBase> GetAllProviders();
            
        /// <summary>
        /// Gets a collection of inactive (not saved) <see cref="IGatewayProviderSettings"/> by type
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="GatewayProviderBase"/>
        /// </typeparam>
        /// <returns>
        /// The collection of inactive (not saved) <see cref="IGatewayProviderSettings"/>.
        /// </returns>
        IEnumerable<GatewayProviderBase> GetAllProviders<T>() where T : GatewayProviderBase;

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey">The gateway provider key</param>
        /// <param name="activatedOnly">Search only activated providers</param>
        /// <returns>An instantiated GatewayProvider</returns>
        T GetProviderByKey<T>(Guid gatewayProviderKey, bool activatedOnly = true) where T : GatewayProviderBase;

        /// <summary>
        /// Refreshes the <see cref="GatewayProviderBase"/> cache
        /// </summary>
        void RefreshCache(); 
    }
}