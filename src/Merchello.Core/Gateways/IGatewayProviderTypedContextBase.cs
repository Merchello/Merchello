using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines a GatewayContext for a specific provider type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGatewayProviderTypedContextBase<out T> where T : GatewayProviderBase
    {
        /// <summary>
        /// Lists all available <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <returns>A collection of all GatewayProvider of the particular type T</returns>
        IEnumerable<GatewayProviderBase> GetAllActivatedProviders();

        /// <summary>
        /// Lists all available providers.  This list includes providers that are just resolved and not configured
        /// </summary>
        /// <returns>A collection of all Gatewayprovider</returns>
        IEnumerable<GatewayProviderBase> GetAllProviders();

        /// <summary>
        /// Instantiates a GatewayProvider given its registered Key
        /// </summary>
        /// <typeparam name="T">The Type of the GatewayProvider.  Must inherit from GatewayProviderBase</typeparam>
        /// <param name="gatewayProviderKey"></param>
        /// <param name="activatedOnly">Search only activated providers</param>
        /// <returns>An instantiated GatewayProvider</returns>
        T GetProviderByKey(Guid gatewayProviderKey, bool activatedOnly = true);

        /// <summary>
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (Guid) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (Guid) of the <see cref="IGatewayMethod"/></param>
        /// <returns>An instantiated GatewayProvider</returns>
        T GetProviderByMethodKey(Guid gatewayMethodKey);

        /// <summary>
        /// Obsolete method
        /// </summary>
        /// <param name="gatewayProviderKey"></param>
        /// <returns></returns>
        [Obsolete("Use GetProviderByKey instead")]
        T CreateInstance(Guid gatewayProviderKey);

        /// <summary>
        /// Activates a GatewayProvider
        /// </summary>
        /// <param name="provider">The GatewayProvider</param>
        void ActivateProvider(GatewayProviderBase provider);

        /// <summary>
        /// Activates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderSettings">The <see cref="IGatewayProviderSettings"/> to be activated</param>
        void ActivateProvider(IGatewayProviderSettings gatewayProviderSettings);


        /// <summary>
        /// Activates a GatewayProvider
        /// </summary>
        /// <param name="provider">The GatewayProvider</param>
        void DeactivateProvider(GatewayProviderBase provider);

        /// <summary>
        /// Deactivates a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderSettings">The <see cref="IGatewayProviderSettings"/> to be deactivated</param>
        void DeactivateProvider(IGatewayProviderSettings gatewayProviderSettings);

    }
}