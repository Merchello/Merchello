using System;
using System.Collections.Generic;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Marker interface for Providers 
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// The unique Key (Guid) for the gateway.  
        /// Used by Merchello in the GatewayProvider's installation/configuration
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayResource> ListResourcesOffered();
    }
}